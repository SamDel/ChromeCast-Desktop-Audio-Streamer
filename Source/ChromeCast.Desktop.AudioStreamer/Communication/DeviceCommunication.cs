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
        private Func<ushort> getPort;
        private Action sendSilence;
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

        public void LaunchAndLoadMedia()
        {
            setDeviceState?.Invoke(DeviceState.LaunchingApplication, null);
            Connect();
            if (isDeviceConnected())
                Launch();
        }

        public void Connect(string sourceId = null, string destinationId = null)
        {
            SendMessage(chromeCastMessages.GetConnectMessage(sourceId, destinationId));
        }

        public void Launch()
        {
            SendMessage(chromeCastMessages.GetLaunchMessage(GetNextRequestId()));
        }

        public void LoadMedia()
        {
            if (applicationLogic == null)
                return;

            setDeviceState?.Invoke(DeviceState.LoadingMedia, null);
            SendMessage(chromeCastMessages.GetLoadMessage(applicationLogic.GetStreamingUrl(), chromeCastSource, chromeCastDestination));
        }

        public void PauseMedia()
        {
            setDeviceState?.Invoke(DeviceState.Paused, null);
            SendMessage(chromeCastMessages.GetPauseMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public void PlayMedia()
        {
            setDeviceState?.Invoke(DeviceState.Playing, null);
            SendMessage(chromeCastMessages.GetPlayMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public void VolumeSet(Volume volumeSetting)
        {
            nextVolumeSetItem = new VolumeSetItem { Setting = volumeSetting };
            SendVolumeSet();
        }

        private void SendVolumeSet()
        {
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

        public void VolumeMute(bool muted)
        {
            SendMessage(chromeCastMessages.GetVolumeMuteMessage(muted, GetNextRequestId()));
        }

        public void Pong()
        {
            SendMessage(chromeCastMessages.GetPongMessage());
        }

        public void GetStatus()
        {
            var deviceState = getDeviceState();
            if (deviceState == DeviceState.Playing ||
                deviceState == DeviceState.Buffering ||
                deviceState == DeviceState.Paused)
                SendMessage(chromeCastMessages.GetMediaStatusMessage(GetNextRequestId(), chromeCastSource, chromeCastDestination));
            else
                GetReceiverStatus();
        }

        private void GetReceiverStatus()
        {
            ConnectionConnect();
            SendMessage(chromeCastMessages.GetReceiverStatusMessage(GetNextRequestId()));
        }

        private void ConnectionConnect()
        {
            var deviceState = getDeviceState();
            if (!Connected)
            {
                SendMessage(chromeCastMessages.GetConnectMessage(null, null));
                if (isDeviceConnected())
                    Connected = true;
            }
        }

        public void Stop()
        {
            var deviceState = getDeviceState();

            // Hack to stop a device. If nothing is send the device is in buffering state and doesn't respond to stop messages. Send some silence first.
            if (deviceState == DeviceState.Buffering)
                sendSilence();

            SendMessage(chromeCastMessages.GetStopMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public int GetNextRequestId()
        {
            return ++requestId;
        }

        public void SendMessage(CastMessage castMessage)
        {
            var byteMessage = chromeCastMessages.MessageToByteArray(castMessage);
            sendMessage?.Invoke(byteMessage);

            logger.Log($"{Properties.Strings.Log_Out} [{DateTime.Now.ToLongTimeString()}][{getHost?.Invoke()}:{getPort?.Invoke()}] [{getDeviceState()}]: {castMessage.PayloadUtf8}");
        }

        public void OnReceiveMessage(CastMessage castMessage)
        {
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

        private void OnReceiveCloseMessage(PayloadMessageBase closeMessage)
        {
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
            if (applicationLogic.GetAutoRestart() && previousState == DeviceState.Playing)
            {
                Task.Delay(5000).Wait();
                PlayMedia();
            }
            else
            {
                Task.Delay(2000).Wait();
                GetReceiverStatus();
            }
        }

        private void OnReceiveMediaStatus(MessageMediaStatus mediaStatusMessage)
        {
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

        private string GetPlayingTime(MessageMediaStatus mediaStatusMessage)
        {
            if (mediaStatusMessage.status != null && mediaStatusMessage.status.First() != null)
            {
                var seconds = (int)(mediaStatusMessage.status.First().currentTime % 60);
                var minutes = ((int)(mediaStatusMessage.status.First().currentTime) % 3600) / 60;
                var hours = ((int)mediaStatusMessage.status.First().currentTime) / 3600;
                return $"{hours}:{minutes.ToString("D2")}";
            }

            return null;
        }

        private void OnReceiveReceiverStatus(MessageReceiverStatus receiverStatusMessage)
        {
            if (receiverStatusMessage?.status?.volume != null)
                onVolumeUpdate(receiverStatusMessage.status.volume);

            var statusText = receiverStatusMessage?.status?.applications?.FirstOrDefault()?.statusText;
            statusText = statusText?.Replace("Default Media Receiver", string.Empty);
            var state = getDeviceState();
            if (state == DeviceState.ConnectError)
                state = DeviceState.NotConnected;
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

        public void SetCallback(Action<DeviceState, string> setDeviceStateIn, 
            Action<Volume> onVolumeUpdateIn, 
            Action<byte[]> sendMessageIn, 
            Func<DeviceState> getDeviceStateIn, 
            Func<bool> isConnectedIn, 
            Func<bool> isDeviceConnectedIn, 
            Func<string> getHostIn, 
            Func<ushort> getPortIn,
            Action sendSilenceIn)
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
        }

        public void OnPlayPause_Click()
        {
            switch (getDeviceState())
            {
                case DeviceState.Buffering:
                    Stop();
                    break;
                case DeviceState.Playing:
                    PauseMedia();
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
    }

    public class VolumeSetItem
    {
        public Volume Setting { get; set; }
        public int RequestId { get; set; }
        public DateTime SendAt { get; set; }
    }
}
