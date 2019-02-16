using System;
using System.Linq;
using System.Web.Script.Serialization;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using System.Threading.Tasks;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceCommunication : IDeviceCommunication
    {
        private IDevice device;
        private Action<byte[]> sendMessage;
        private Func<bool> isDeviceConnected;
        private IApplicationLogic applicationLogic;
        private ILogger logger;
        private IChromeCastMessages chromeCastMessages;
        private string chromeCastDestination;
        private readonly string chromeCastSource;
        private string chromeCastApplicationSessionNr;
        private int chromeCastMediaSessionId;
        private int requestId;
        private VolumeSetItem lastVolumeSetItem;
        private VolumeSetItem nextVolumeSetItem;
        private bool Connected = false;
        private UserMode userMode = UserMode.Stopped;

        public DeviceCommunication(IApplicationLogic applicationLogicIn, ILogger loggerIn, IChromeCastMessages chromeCastMessagesIn)
        {
            applicationLogic = applicationLogicIn;
            logger = loggerIn;
            chromeCastMessages = chromeCastMessagesIn;
            chromeCastDestination = string.Empty;
            chromeCastSource = string.Format("client-8{0}", new Random().Next(10000, 99999));
            requestId = 0;
        }

        /// <summary>
        /// Launch the device, media is loaded when the device responded.
        /// </summary>
        public void LaunchAndLoadMedia()
        {
            device.SetDeviceState(DeviceState.LaunchingApplication, null);
            Connect();

            WaitDeviceConnected(Launch);
        }

        /// <summary>
        /// Wait till the connection is established.
        /// </summary>
        private void WaitDeviceConnected(Action callback)
        {
            var attempt = 0;
            while (!isDeviceConnected() && attempt++ < 5)
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
            if (chromeCastMessages == null)
                return;

            SendMessage(chromeCastMessages.GetConnectMessage(sourceId, destinationId));
        }

        /// <summary>
        /// Send a launch message.
        /// </summary>
        public void Launch()
        {
            if (chromeCastMessages == null)
                return;

            SendMessage(chromeCastMessages.GetLaunchMessage(GetNextRequestId()));
        }

        /// <summary>
        /// Send a load media message.
        /// </summary>
        public void LoadMedia()
        {
            if (applicationLogic == null || chromeCastMessages == null)
                return;

            device.SetDeviceState(DeviceState.LoadingMedia, null);
            SendMessage(chromeCastMessages.GetLoadMessage(applicationLogic.GetStreamingUrl(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Send a pause media message.
        /// </summary>
        public void PauseMedia()
        {
            if (chromeCastMessages == null)
                return;

            device.SetDeviceState(DeviceState.Paused, null);
            SendMessage(chromeCastMessages.GetPauseMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Send a play message.
        /// </summary>
        public void PlayMedia()
        {
            if (chromeCastMessages == null)
                return;

            SendMessage(chromeCastMessages.GetPlayMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Set the volume to the new level.
        /// </summary>
        /// <param name="volumeSetting">the new volume level</param>
        public void VolumeSet(Volume volumeSetting)
        {
            if (volumeSetting == null)
                return;

            nextVolumeSetItem = new VolumeSetItem { Setting = volumeSetting };
            SendVolumeSet();
        }

        /// <summary>
        /// Send a message to set the volume.
        /// </summary>
        private void SendVolumeSet()
        {
            if (chromeCastMessages == null)
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
            if (chromeCastMessages == null)
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
            if (chromeCastMessages == null)
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
            if (chromeCastMessages == null)
                return;

            var deviceState = device.GetDeviceState();
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
                    case DeviceState.LaunchingApplication:
                    case DeviceState.LaunchedApplication:
                    case DeviceState.Idle:
                        ResumePlaying();
                        break;
                    case DeviceState.LoadingMedia:
                    case DeviceState.Buffering:
                    case DeviceState.Playing:
                    case DeviceState.Paused:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Send a message to get the receiver status.
        /// </summary>
        private void GetReceiverStatus()
        {
            if (chromeCastMessages == null)
                return;

            ConnectionConnect();
            SendMessage(chromeCastMessages.GetReceiverStatusMessage(GetNextRequestId()));
        }

        /// <summary>
        /// Send a connect message, when not connected.
        /// </summary>
        private void ConnectionConnect()
        {
            if (chromeCastMessages == null)
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
        public void Stop()
        {
            if (chromeCastMessages == null)
                return;

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
            if (chromeCastMessages == null)
                return;

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
            if (castMessage == null)
                return;

            logger.Log($"{Properties.Strings.Log_In} [{DateTime.Now.ToLongTimeString()}] [{device.GetHost()}:{device.GetPort()}] [{device.GetDeviceState()}]: {castMessage.PayloadUtf8}");
            var js = new JavaScriptSerializer();

            var message = new JavaScriptSerializer().Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
            switch (message.@type)
            {
                case "RECEIVER_STATUS":
                    OnReceiveReceiverStatus(js.Deserialize<MessageReceiverStatus>(castMessage.PayloadUtf8));
                    break;
                case "MEDIA_STATUS":
                    OnReceiveMediaStatus(js.Deserialize<MessageMediaStatus>(castMessage.PayloadUtf8));
                    break;
                case "PING":
                    Pong();
                    break;
                case "PONG":
                    var pongMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    break;
                case "CLOSE":
                    var previousState = device.GetDeviceState();
                    var closeMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    OnReceiveCloseMessage(closeMessage);
                    break;
                case "LOAD_FAILED":
                    var loadFailedMessage = js.Deserialize<MessageLoadFailed>(castMessage.PayloadUtf8);
                    device.SetDeviceState(DeviceState.LoadFailed, null);
                    break;
                case "LOAD_CANCELLED":
                    var loadCancelledMessage = js.Deserialize<MessageLoadCancelled>(castMessage.PayloadUtf8);
                    device.SetDeviceState(DeviceState.LoadCancelled, null);
                    break;
                case "INVALID_REQUEST":
                    var invalidRequestMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
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
        /// <param name="closeMessage">the close message</param>
        private void OnReceiveCloseMessage(PayloadMessageBase closeMessage)
        {
            if (applicationLogic == null)
                return;

            if (!(applicationLogic.GetAutoRestart() || device.WasPlayingWhenStopped()))
                userMode = UserMode.Stopped;

            var deviceState = device.GetDeviceState();
            if (deviceState == DeviceState.Playing ||
                deviceState == DeviceState.Buffering ||
                deviceState == DeviceState.Paused ||
                deviceState == DeviceState.LoadingMedia)
            {
                Stop();
            }
            device.SetDeviceState(DeviceState.Closed, null);
            Connected = false;
            Task.Run(() => {
                Task.Delay(2000).Wait();
                GetReceiverStatus();
            });
        }

        /// <summary>
        /// Try to resume playing.
        /// </summary>
        public void ResumePlaying()
        {
            logger.Log("ResumePlaying");

            Task.Run(() =>
            {
                Task.Delay(2000).Wait();
                var deviceState = device.GetDeviceState();
                if (deviceState == DeviceState.Playing ||
                    deviceState == DeviceState.Buffering ||
                    deviceState == DeviceState.Paused ||
                    deviceState == DeviceState.LoadingMedia)
                {
                    Stop();
                }
                device.SetDeviceState(DeviceState.NotConnected, null);
                Disconnect();
                Task.Delay(2000).Wait();
                var attempt = 0;
                while ((device.GetDeviceState() == DeviceState.NotConnected
                        || device.GetDeviceState() == DeviceState.Connected
                        || device.GetDeviceState() == DeviceState.Closed
                        || device.GetDeviceState() == DeviceState.Idle)
                    && attempt++ < 6)
                {
                    device.OnGetStatus();
                    Task.Delay(5000).Wait();
                    if (device.GetDeviceState() == DeviceState.Connected
                        || device.GetDeviceState() == DeviceState.Idle)
                    {
                        PlayStop();
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// Handle a media status message from the device.
        /// </summary>
        /// <param name="mediaStatusMessage">the media status message</param>
        private void OnReceiveMediaStatus(MessageMediaStatus mediaStatusMessage)
        {
            if (mediaStatusMessage == null)
                return;

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
            if (mediaStatusMessage == null)
                return string.Empty;

            if (mediaStatusMessage.status != null && mediaStatusMessage.status.First() != null)
            {
                var seconds = (int)(mediaStatusMessage.status.First().currentTime % 60);
                var minutes = ((int)(mediaStatusMessage.status.First().currentTime) % 3600) / 60;
                var hours = ((int)mediaStatusMessage.status.First().currentTime) / 3600;
                return $"{hours}:{minutes.ToString("D2")}";
            }

            return null;
        }

        /// <summary>
        /// Handle a receiver status message from the device.
        /// </summary>
        /// <param name="receiverStatusMessage">a receiver status message</param>
        private void OnReceiveReceiverStatus(MessageReceiverStatus receiverStatusMessage)
        {
            if (receiverStatusMessage == null)
                return;

            if (receiverStatusMessage?.status?.volume != null)
                device.OnVolumeUpdate(receiverStatusMessage.status.volume);

            var statusText = receiverStatusMessage?.status?.applications?.FirstOrDefault()?.statusText;
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
                case DeviceState.Paused:
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
    }
}
