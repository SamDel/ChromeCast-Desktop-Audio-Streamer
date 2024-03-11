﻿using System;
using System.Linq;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using System.Threading.Tasks;
using ChromeCast.Desktop.AudioStreamer.Application;
using System.Threading;
using System.Text.Json;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceCommunication : IDeviceCommunication
    {
        private IDevice device;
        private Action<byte[]> sendMessage;
        private Func<bool> isDeviceConnected;
        private readonly IApplicationLogic applicationLogic;
        private readonly ILogger logger;
        private readonly IChromeCastMessages chromeCastMessages;
        private string chromeCastDestination;
        private readonly string chromeCastSource;
        private string chromeCastApplicationSessionNr;
        private int chromeCastMediaSessionId;
        private int requestId;
        private VolumeSetItem lastVolumeSetItem;
        private VolumeSetItem nextVolumeSetItem;
        private bool Connected = false;
        private bool IsDisposed = false;
        private UserMode userMode = UserMode.Stopped;
        private bool pendingStatusMessage = false;
        private DateTime lastReceivedMessage = DateTime.MinValue;
        private string statusText;

        public DeviceCommunication(IApplicationLogic applicationLogicIn, ILogger loggerIn)
        {
            applicationLogic = applicationLogicIn;
            logger = loggerIn;
            chromeCastMessages = new ChromeCastMessages();
            chromeCastDestination = string.Empty;
            chromeCastSource = string.Format("client-8{0}", new Random().Next(10000, 99999));
            requestId = 0;
        }

        /// <summary>
        /// Launch the device, media is loaded when the device responded.
        /// </summary>
        public void LaunchAndLoadMedia()
        {
            if (device == null || IsDisposed)
                return;

            // Check to make sure the status of the device is received before streaming is started.
            if (device.GetDeviceState() == DeviceState.Undefined)
            {
                GetStatus();
                WaitDeviceStatusReceived(20);
            }

            pendingStatusMessage = false;
            device.SetDeviceState(DeviceState.LaunchingApplication, null);
            Connect();

            WaitDeviceConnected(Launch);
        }

        /// <summary>
        /// Wait till the status has been received.
        /// </summary>
        private bool WaitDeviceStatusReceived(int nrWaitMsec = 5)
        {
            var attempt = 0;
            while (pendingStatusMessage && attempt++ < nrWaitMsec)
            {
                Task.Delay(100).Wait();
            }

            if (pendingStatusMessage)
                return false;

            return true;
        }

        /// <summary>
        /// Wait till the connection is established.
        /// </summary>
        private void WaitDeviceConnected(Action callback, int nrWaitMsec = 5)
        {
            var attempt = 0;
            while (!isDeviceConnected() && attempt++ < nrWaitMsec)
            {
                Task.Delay(100).Wait();
            }

            if (isDeviceConnected())
                callback();
        }

        /// <summary>
        /// Send a connect message
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="destinationId"></param>
        public void Connect(string sourceId = null, string destinationId = null)
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            SendMessage(chromeCastMessages.GetConnectMessage(sourceId, destinationId));
        }

        /// <summary>
        /// Send a launch message.
        /// </summary>
        public void Launch()
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            SendMessage(chromeCastMessages.GetLaunchMessage(GetNextRequestId()));
        }

        /// <summary>
        /// Send a load media message.
        /// </summary>
        public void LoadMedia()
        {
            if (applicationLogic == null || chromeCastMessages == null || device == null || IsDisposed)
                return;

            device.SetDeviceState(DeviceState.LoadingMedia, null);
            SendMessage(chromeCastMessages.GetLoadMessage(applicationLogic.GetStreamingUrl(), chromeCastSource, chromeCastDestination, GetNextRequestId(), applicationLogic.GetStreamTitle()));
        }

        /// <summary>
        /// Send a pause media message.
        /// </summary>
        public void PauseMedia()
        {
            if (chromeCastMessages == null || device == null || IsDisposed)
                return;

            device.SetDeviceState(DeviceState.Paused, null);
            SendMessage(chromeCastMessages.GetPauseMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Send a play message.
        /// </summary>
        public void PlayMedia()
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            SendMessage(chromeCastMessages.GetPlayMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Set the volume to the new level.
        /// </summary>
        /// <param name="volumeSetting">the new volume level</param>
        public void VolumeSet(Volume volumeSetting)
        {
            if (volumeSetting == null || IsDisposed)
                return;

            nextVolumeSetItem = new VolumeSetItem { Setting = volumeSetting };
            SendVolumeSet();
        }

        /// <summary>
        /// Send a message to set the volume.
        /// </summary>
        private void SendVolumeSet()
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            if (!Connected)
                return;

            if ((nextVolumeSetItem != null && lastVolumeSetItem == null)
                || (lastVolumeSetItem != null && DateTime.Now.Subtract(lastVolumeSetItem.SendAt) > new TimeSpan(0, 0, 1)))
            {
                lastVolumeSetItem = nextVolumeSetItem;
                lastVolumeSetItem.RequestId = GetNextRequestId();
                lastVolumeSetItem.SendAt = DateTime.Now;
                SendMessage(chromeCastMessages.GetVolumeSetMessage(lastVolumeSetItem.Setting, lastVolumeSetItem.RequestId));
                nextVolumeSetItem = null;
            }
        }

        /// <summary>
        /// Send a message to (un)mute the volume.
        /// </summary>
        /// <param name="muted">true = mute, false = unmute</param>
        public void VolumeMute(bool muted)
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            if (!Connected)
                return;

            SendMessage(chromeCastMessages.GetVolumeMuteMessage(muted, GetNextRequestId()));
        }

        /// <summary>
        /// Send a pong response to ping.
        /// </summary>
        public void Pong()
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            if (!Connected)
                return;

            SendMessage(chromeCastMessages.GetPongMessage());
        }

        /// <summary>
        /// Send a message to get the device media or receiver status.
        /// </summary>
        public void GetStatus()
        {
            if (chromeCastMessages == null || device == null || IsDisposed)
                return;

            var deviceState = device.GetDeviceState();
            if (!pendingStatusMessage)
            {
                pendingStatusMessage = true;
                if (deviceState == DeviceState.Playing ||
                    deviceState == DeviceState.Buffering ||
                    deviceState == DeviceState.Paused)
                {
                    SendMessage(chromeCastMessages.GetMediaStatusMessage(GetNextRequestId(), chromeCastSource, chromeCastDestination));
                }
                else
                {
                    GetReceiverStatus();
                }
            }
            else
            {
                logger.Log($"[{DateTime.Now.ToLongTimeString()}] [{device.GetHost()}:{device.GetPort()}] Last received message: {lastReceivedMessage}");
                device.SetDeviceState(DeviceState.Undefined);
                device.CloseConnection();
                if (NoContactFor(15 * 60))
                    pendingStatusMessage = false;
            }

            // Keep trying to play when in playing mode.
            if (userMode == UserMode.Playing)
            {
                switch (deviceState)
                {
                    case DeviceState.NotConnected:
                    case DeviceState.Disposed:
                    case DeviceState.ConnectError:
                    case DeviceState.LoadFailed:
                    case DeviceState.LoadCancelled:
                    case DeviceState.InvalidRequest:
                    case DeviceState.Closed:
                    case DeviceState.Connected:
                        ResumePlaying();
                        break;
                    case DeviceState.LaunchingApplication:
                    case DeviceState.LaunchedApplication:
                    case DeviceState.Idle:
                        var deviceStateBefore = deviceState;
                        Task.Delay(5000).Wait();
                        if (device.GetDeviceState() == deviceStateBefore)
                            ResumePlaying();
                        break;
                    case DeviceState.LoadingMedia:
                    case DeviceState.LoadingMediaCheckFirewall:
                    case DeviceState.Buffering:
                    case DeviceState.Playing:
                    case DeviceState.Paused:
                    default:
                        break;
                }
            }
        }

        private bool NoContactFor(int nrSeconds)
        {
            return HadContact() && (DateTime.Now - lastReceivedMessage).Seconds > nrSeconds;
        }

        private bool HadContact()
        {
            return lastReceivedMessage != DateTime.MinValue;
        }

        /// <summary>
        /// Get the status text returned by the device.
        /// </summary>
        /// <returns>the status text</returns>
        public string GetStatusText()
        {
            return statusText;
        }

        /// <summary>
        /// Send a message to get the receiver status.
        /// </summary>
        private void GetReceiverStatus()
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            ConnectionConnect();
            SendMessage(chromeCastMessages.GetReceiverStatusMessage(GetNextRequestId()));
        }

        /// <summary>
        /// Send a connect message, when not connected.
        /// </summary>
        private void ConnectionConnect()
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            if (!Connected)
            {
                SendMessage(chromeCastMessages.GetConnectMessage(null, null));
                WaitDeviceConnected(new Action(() => { Connected = true; }));
            }
        }

        /// <summary>
        /// Send a stop message.
        /// </summary>
        public void Stop(bool changeUserMode = false)
        {
            if (chromeCastMessages == null || device == null || IsDisposed)
                return;

            if (changeUserMode)
                userMode = UserMode.Stopped;

            var deviceState = device.GetDeviceState();

            // Hack to stop a device. If nothing is send the device is in buffering state and doesn't respond to stop messages. Send some silence first.
            if (deviceState == DeviceState.Buffering)
                device.SendSilence();

            SendMessage(chromeCastMessages.GetStopMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Create a new request id (to be used in the messages to the device).
        /// </summary>
        /// <returns></returns>
        public int GetNextRequestId()
        {
            return ++requestId;
        }

        /// <summary>
        /// Do send a message.
        /// </summary>
        /// <param name="castMessage">the message to send</param>
        public void SendMessage(CastMessage castMessage)
        {
            if (chromeCastMessages == null || IsDisposed)
                return;

            if (NoContactFor(60))
            {
                device.CloseConnection();
                return;
            }

            var byteMessage = chromeCastMessages.MessageToByteArray(castMessage);
            sendMessage?.Invoke(byteMessage);

            logger.Log($"{Properties.Strings.Log_Out} [{DateTime.Now.ToLongTimeString()}][{device.GetHost()}:{device.GetPort()}] [{device.GetDeviceState()}]: {castMessage.PayloadUtf8}");
        }

        /// <summary>
        /// Handle a message from the device.
        /// </summary>
        /// <param name="castMessage">the received message</param>
        public void OnReceiveMessage(CastMessage castMessage)
        {
            if (castMessage == null || device == null || IsDisposed)
                return;

            logger.Log($"{Properties.Strings.Log_In} [{DateTime.Now.ToLongTimeString()}] [{device.GetHost()}:{device.GetPort()}] [{device.GetDeviceState()}]: {castMessage.PayloadUtf8}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var message = JsonSerializer.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8, options);
            if (message.type != "PING" && message.type != "PONG")
            {
                lastReceivedMessage = DateTime.Now;
                pendingStatusMessage = false;
            }

            switch (message.@type)
            {
                case "RECEIVER_STATUS":
                    OnReceiveReceiverStatus(JsonSerializer.Deserialize<MessageReceiverStatus>(castMessage.PayloadUtf8, options));
                    break;
                case "MEDIA_STATUS":
                    OnReceiveMediaStatus(JsonSerializer.Deserialize<MessageMediaStatus>(castMessage.PayloadUtf8, options));
                    break;
                case "PING":
                    Pong();
                    break;
                case "PONG":
                    break;
                case "CLOSE":
                    OnReceiveCloseMessage();
                    break;
                case "LOAD_FAILED":
                    device.SetDeviceState(DeviceState.LoadFailed, null);
                    break;
                case "LOAD_CANCELLED":
                    device.SetDeviceState(DeviceState.LoadCancelled, null);
                    break;
                case "INVALID_REQUEST":
                    device.SetDeviceState(DeviceState.InvalidRequest, null);
                    break;
                case "LAUNCH_ERROR":
                    device.SetDeviceState(DeviceState.LoadCancelled, null);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handle a close message from the device.
        /// </summary>
        private void OnReceiveCloseMessage()
        {
            if (applicationLogic == null || device == null || IsDisposed)
                return;

            if (!(applicationLogic.GetAutoRestart()))
                userMode = UserMode.Stopped;

            var deviceState = device.GetDeviceState();
            if (deviceState == DeviceState.Playing ||
                deviceState == DeviceState.Buffering ||
                deviceState == DeviceState.Paused ||
                deviceState == DeviceState.LoadingMedia ||
                deviceState == DeviceState.LoadingMediaCheckFirewall)
            {
                Stop();
            }
            device.SetDeviceState(DeviceState.Closed, null);
            Connected = false;
            var cancellationTokeSource = new CancellationTokenSource();
            applicationLogic.StartTask(() => {
                for (int i = 0; i < 20; i++)
                {
                    Task.Delay(100).Wait();

                    if (cancellationTokeSource.IsCancellationRequested)
                        return;
                }

                GetReceiverStatus();
            }, cancellationTokeSource);
        }

        /// <summary>
        /// Try to resume playing.
        /// </summary>
        public void ResumePlaying()
        {
            if (device == null || IsDisposed)
                return;

            logger.Log($"[{DateTime.Now.ToLongTimeString()}] [{device.GetHost()}:{device.GetPort()}] ResumePlaying");
            userMode = UserMode.Playing;
            pendingStatusMessage = false;

            var cancellationTokenSource = new CancellationTokenSource();
            applicationLogic.StartTask(() =>
            {
                Task.Delay(2000).Wait();
                var deviceState = device.GetDeviceState();
                if (deviceState == DeviceState.Playing ||
                    deviceState == DeviceState.Buffering ||
                    deviceState == DeviceState.Paused ||
                    deviceState == DeviceState.LoadingMedia ||
                    deviceState == DeviceState.LoadingMediaCheckFirewall)
                {
                    Stop();
                }

                if (cancellationTokenSource.IsCancellationRequested)
                    return;

                if (deviceState != DeviceState.ConnectError)
                    device.SetDeviceState(DeviceState.NotConnected, null);
                Disconnect();
                Task.Delay(2000).Wait();

                if (cancellationTokenSource.IsCancellationRequested)
                    return;

                if (device.GetDeviceState() == DeviceState.NotConnected
                        || device.GetDeviceState() == DeviceState.Connected
                        || device.GetDeviceState() == DeviceState.Closed
                        || device.GetDeviceState() == DeviceState.Idle)
                {
                    device.OnGetStatus();
                    if (device.IsStatusTextBlank())
                        WaitDeviceConnected(PlayStop, 50);
                }
            }, cancellationTokenSource);
        }

        /// <summary>
        /// Handle a media status message from the device.
        /// </summary>
        /// <param name="mediaStatusMessage">the media status message</param>
        private void OnReceiveMediaStatus(MessageMediaStatus mediaStatusMessage)
        {
            if (mediaStatusMessage == null || device == null || IsDisposed)
                return;

            Connected = true;

            if (mediaStatusMessage?.status?.First()?.volume?.controlType != null && 
                mediaStatusMessage?.status?.First()?.volume?.stepInterval > 0)
                device.OnVolumeUpdate(mediaStatusMessage.status.First().volume);

            chromeCastMediaSessionId = mediaStatusMessage.status.Any() ? mediaStatusMessage.status.First().mediaSessionId : 1;

            if (device.IsConnected() && mediaStatusMessage.status.Any())
            {
                switch (mediaStatusMessage.status.First().playerState)
                {
                    case "IDLE":
                        device.SetDeviceState(DeviceState.Idle, null);
                        break;
                    case "BUFFERING":
                        device.SetDeviceState(DeviceState.Buffering, GetPlayingTime(mediaStatusMessage));
                        break;
                    case "PAUSED":
                        device.SetDeviceState(DeviceState.Paused, null);
                        break;
                    case "PLAYING":
                        device.SetDeviceState(DeviceState.Playing, GetPlayingTime(mediaStatusMessage));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Extract the playing time from a media status message.
        /// </summary>
        /// <param name="mediaStatusMessage">a media status message</param>
        /// <returns>the playing time, format hh:mm</returns>
        private string GetPlayingTime(MessageMediaStatus mediaStatusMessage)
        {
            if (mediaStatusMessage == null || IsDisposed)
                return string.Empty;

            if (mediaStatusMessage.status != null && mediaStatusMessage.status.First() != null)
            {
                var minutes = ((int)(mediaStatusMessage.status.First().currentTime) % 3600) / 60;
                var hours = ((int)mediaStatusMessage.status.First().currentTime) / 3600;
                return $"{hours}:{minutes:D2}";
            }

            return null;
        }

        /// <summary>
        /// Handle a receiver status message from the device.
        /// </summary>
        /// <param name="receiverStatusMessage">a receiver status message</param>
        private void OnReceiveReceiverStatus(MessageReceiverStatus receiverStatusMessage)
        {
            if (receiverStatusMessage == null || device == null || IsDisposed)
                return;

            if (receiverStatusMessage?.status?.volume != null)
                device.OnVolumeUpdate(receiverStatusMessage.status.volume);

            statusText = receiverStatusMessage?.status?.applications?.FirstOrDefault()?.statusText;
            statusText = statusText?.Replace("Default Media Receiver", string.Empty);
            var state = device.GetDeviceState();
            if (state == DeviceState.ConnectError || state == DeviceState.NotConnected || state == DeviceState.Closed)
            {
                device.SetDeviceState(DeviceState.Connected, null);
                Connected = true;
            }
            device.SetDeviceState(device.GetDeviceState(), $" {statusText}");

            if (receiverStatusMessage != null && receiverStatusMessage.status != null && receiverStatusMessage.status.applications != null)
            {
                var deviceApplication = receiverStatusMessage.status.applications.Where(a => a.appId.Equals("CC1AD845"));
                if (deviceApplication.Any())
                {
                    chromeCastDestination = deviceApplication.First().transportId;
                    chromeCastApplicationSessionNr = deviceApplication.First().sessionId;

                    if (device.GetDeviceState().Equals(DeviceState.LaunchingApplication))
                    {
                        device.SetDeviceState(DeviceState.LaunchedApplication, null);
                        Connect(chromeCastSource, chromeCastDestination);
                        LoadMedia();
                    }
                }
            }

            if (lastVolumeSetItem != null && lastVolumeSetItem.RequestId == receiverStatusMessage.requestId)
            {
                lastVolumeSetItem = null;
                SendVolumeSet();
            }
        }

        /// <summary>
        /// Set the callbacks for the device communication.
        /// </summary>
        public void SetCallback(IDevice deviceIn, Action<byte[]> sendMessageIn, Func<bool> isDeviceConnectedIn)
        {
            device = deviceIn;
            sendMessage = sendMessageIn;
            isDeviceConnected = isDeviceConnectedIn;
            pendingStatusMessage = false;
        }

        /// <summary>
        /// Handle a clcik on the play button.
        /// </summary>
        public void OnPlayStop_Click()
        {
            if (userMode == UserMode.Stopped)
                userMode = UserMode.Playing;
            else
                userMode = UserMode.Stopped;

            PlayStop();
        }

        /// <summary>
        /// Play or stop.
        /// </summary>
        private void PlayStop()
        {
            switch (device.GetDeviceState())
            {
                case DeviceState.Buffering:
                case DeviceState.Playing:
                    Stop();
                    break;
                case DeviceState.LaunchingApplication:
                case DeviceState.LaunchedApplication:
                case DeviceState.LoadingMedia:
                case DeviceState.LoadingMediaCheckFirewall:
                case DeviceState.Idle:
                    LoadMedia();
                    break;
                case DeviceState.Paused:
                    PlayMedia();
                    break;
                case DeviceState.NotConnected:
                case DeviceState.Connected:
                case DeviceState.ConnectError:
                case DeviceState.Closed:
                case DeviceState.LoadCancelled:
                case DeviceState.LoadFailed:
                case DeviceState.InvalidRequest:
                case DeviceState.Undefined:
                    LaunchAndLoadMedia();
                    break;
                case DeviceState.Disposed:
                    break;
                default:
                    break;
            }
        }

        public void OnStop_Click()
        {
            switch (device.GetDeviceState())
            {
                case DeviceState.Buffering:
                case DeviceState.Playing:
                case DeviceState.LaunchingApplication:
                case DeviceState.LaunchedApplication:
                case DeviceState.LoadingMedia:
                case DeviceState.LoadingMediaCheckFirewall:
                case DeviceState.Paused:
                case DeviceState.Undefined:
                    Stop();
                    break;
                case DeviceState.Idle:
                case DeviceState.NotConnected:
                case DeviceState.Connected:
                case DeviceState.ConnectError:
                case DeviceState.Closed:
                case DeviceState.LoadCancelled:
                case DeviceState.LoadFailed:
                case DeviceState.InvalidRequest:
                case DeviceState.Disposed:
                default:
                    LaunchAndLoadMedia();
                    Task.Delay(750).Wait();
                    Stop();
                    break;
            }
        }

        public void Disconnect()
        {
            Connected = false;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
            userMode = UserMode.Stopped;
        }

        /// <summary>
        /// Return the usermode.
        /// </summary>
        /// <returns>the usermode</returns>
        public UserMode GetUserMode()
        {
            return userMode;
        }
    }
}
