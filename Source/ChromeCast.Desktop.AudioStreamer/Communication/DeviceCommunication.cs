using System;
using System.Linq;
using System.Web.Script.Serialization;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using System.Threading.Tasks;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceCommunication : IDeviceCommunication
    {
        private Action<DeviceState, string> setDeviceState;
        private Action<Volume> onVolumeUpdate;
        private Action<byte[]> sendMessage;
        private Func<bool> isConnected;
        private Func<bool> isDeviceConnected;
        private Func<string> getHost;
        private Func<int> getPort;
        private Action sendSilence;
        private Func<bool> wasPlayingWhenStopped;
        private Func<DeviceState> getDeviceState;
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
            setDeviceState?.Invoke(DeviceState.LaunchingApplication, null);
            Connect();
            if (isDeviceConnected())
                Launch();
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

            setDeviceState?.Invoke(DeviceState.LoadingMedia, null);
            SendMessage(chromeCastMessages.GetLoadMessage(applicationLogic.GetStreamingUrl(), chromeCastSource, chromeCastDestination));
        }

        /// <summary>
        /// Send a pause media message.
        /// </summary>
        public void PauseMedia()
        {
            if (chromeCastMessages == null)
                return;

            setDeviceState?.Invoke(DeviceState.Paused, null);
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

            SendMessage(chromeCastMessages.GetVolumeMuteMessage(muted, GetNextRequestId()));
        }

        /// <summary>
        /// Send a pong response to ping.
        /// </summary>
        public void Pong()
        {
            if (chromeCastMessages == null)
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

            var deviceState = getDeviceState();
            if (deviceState == DeviceState.Playing ||
                deviceState == DeviceState.Buffering ||
                deviceState == DeviceState.Paused)
                SendMessage(chromeCastMessages.GetMediaStatusMessage(GetNextRequestId(), chromeCastSource, chromeCastDestination));
            else
                GetReceiverStatus();
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
                if (isDeviceConnected())
                    Connected = true;
            }
        }

        /// <summary>
        /// Send a stop message.
        /// </summary>
        public void Stop()
        {
            if (chromeCastMessages == null)
                return;

            var deviceState = getDeviceState();

            // Hack to stop a device. If nothing is send the device is in buffering state and doesn't respond to stop messages. Send some silence first.
            if (deviceState == DeviceState.Buffering)
                sendSilence();

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

            logger.Log($"{Properties.Strings.Log_Out} [{DateTime.Now.ToLongTimeString()}][{getHost?.Invoke()}:{getPort?.Invoke()}] [{getDeviceState()}]: {castMessage.PayloadUtf8}");
        }

        /// <summary>
        /// Handle a message from the device.
        /// </summary>
        /// <param name="castMessage">the received message</param>
        public void OnReceiveMessage(CastMessage castMessage)
        {
            if (castMessage == null)
                return;

            logger.Log($"{Properties.Strings.Log_In} [{DateTime.Now.ToLongTimeString()}] [{getHost?.Invoke()}:{getPort?.Invoke()}] [{getDeviceState()}]: {castMessage.PayloadUtf8}");
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
                    var previousState = getDeviceState();
                    var closeMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    OnReceiveCloseMessage(closeMessage);
                    break;
                case "LOAD_FAILED":
                    var loadFailedMessage = js.Deserialize<MessageLoadFailed>(castMessage.PayloadUtf8);
                    setDeviceState(DeviceState.LoadFailed, null);
                    break;
                case "LOAD_CANCELLED":
                    var loadCancelledMessage = js.Deserialize<MessageLoadCancelled>(castMessage.PayloadUtf8);
                    setDeviceState(DeviceState.LoadCancelled, null);
                    break;
                case "INVALID_REQUEST":
                    var invalidRequestMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    setDeviceState(DeviceState.InvalidRequest, null);
                    break;
                case "LAUNCH_ERROR":
                    setDeviceState(DeviceState.LoadCancelled, null);
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

            var deviceState = getDeviceState();
            var previousState = deviceState;
            if (deviceState == DeviceState.Playing ||
                deviceState == DeviceState.Buffering ||
                deviceState == DeviceState.Paused ||
                deviceState == DeviceState.LoadingMedia)
            {
                Stop();
            }
            setDeviceState(DeviceState.Closed, null);
            Connected = false;

            // Restart
            if (wasPlayingWhenStopped() ||
                (applicationLogic.GetAutoRestart() && (previousState == DeviceState.Playing || previousState == DeviceState.ConnectError)))
            {
                Task.Run(() => {
                    Task.Delay(5000).Wait();
                    OnPlayStop_Click();
                });
            }
            else
            {
                Task.Run(() => {
                    Task.Delay(2000).Wait();
                    GetReceiverStatus();
                });
            }
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

            if (isConnected() && mediaStatusMessage.status.Any())
            {
                switch (mediaStatusMessage.status.First().playerState)
                {
                    case "IDLE":
                        setDeviceState(DeviceState.Idle, null);
                        break;
                    case "BUFFERING":
                        setDeviceState(DeviceState.Buffering, GetPlayingTime(mediaStatusMessage));
                        break;
                    case "PAUSED":
                        setDeviceState(DeviceState.Paused, null);
                        break;
                    case "PLAYING":
                        setDeviceState(DeviceState.Playing, GetPlayingTime(mediaStatusMessage));
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
                onVolumeUpdate(receiverStatusMessage.status.volume);

            var statusText = receiverStatusMessage?.status?.applications?.FirstOrDefault()?.statusText;
            statusText = statusText?.Replace("Default Media Receiver", string.Empty);
            var state = getDeviceState();
            if (state == DeviceState.ConnectError)
            {
                setDeviceState(DeviceState.NotConnected, null);
                Disconnect();
            }
            setDeviceState(getDeviceState(), $" {statusText}");

            if (receiverStatusMessage != null && receiverStatusMessage.status != null && receiverStatusMessage.status.applications != null)
            {
                var deviceApplication = receiverStatusMessage.status.applications.Where(a => a.appId.Equals("CC1AD845"));
                if (deviceApplication.Any())
                {
                    chromeCastDestination = deviceApplication.First().transportId;
                    chromeCastApplicationSessionNr = deviceApplication.First().sessionId;

                    if (getDeviceState().Equals(DeviceState.LaunchingApplication))
                    {
                        setDeviceState(DeviceState.LaunchedApplication, null);
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
        public void SetCallback(Action<DeviceState, string> setDeviceStateIn, 
            Action<Volume> onVolumeUpdateIn, 
            Action<byte[]> sendMessageIn, 
            Func<DeviceState> getDeviceStateIn, 
            Func<bool> isConnectedIn, 
            Func<bool> isDeviceConnectedIn, 
            Func<string> getHostIn, 
            Func<int> getPortIn,
            Action sendSilenceIn,
            Func<bool> wasPlayingWhenStoppedIn)
        {
            setDeviceState = setDeviceStateIn;
            onVolumeUpdate = onVolumeUpdateIn;
            sendMessage = sendMessageIn;
            getDeviceState = getDeviceStateIn;
            isConnected = isConnectedIn;
            isDeviceConnected = isDeviceConnectedIn;
            getHost = getHostIn;
            getPort = getPortIn;
            sendSilence = sendSilenceIn;
            wasPlayingWhenStopped = wasPlayingWhenStoppedIn;
        }

        /// <summary>
        /// Handle a clcik on the play button.
        /// </summary>
        public void OnPlayStop_Click()
        {
            switch (getDeviceState())
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
            switch (getDeviceState())
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
