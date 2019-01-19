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
        private IDeviceConnection deviceConnection;
        private DeviceState deviceState;
        private Action<DeviceState, string> setControlDeviceState;
        private Action<Volume> onVolumeUpdate;
        private Func<string> getHost;
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
        private Volume volumeSetting;
        private DateTime lastVolumeChange;

        public DeviceCommunication(IApplicationLogic applicationLogicIn, ILogger loggerIn, IChromeCastMessages chromeCastMessagesIn, IDeviceConnection deviceConnectionIn)
        {
            applicationLogic = applicationLogicIn;
            deviceConnection = deviceConnectionIn;
            logger = loggerIn;
            chromeCastMessages = chromeCastMessagesIn;
            chromeCastDestination = string.Empty;
            chromeCastSource = string.Format("client-8{0}", new Random().Next(10000, 99999));
            requestId = 0;
            SetDeviceState(DeviceState.NotConnected);
            volumeSetting = new Volume
            {
                controlType = "attenuation",
                level = 0.0f,
                muted = false,
                stepInterval = 0.05f
            };
        }

        private void LaunchAndLoadMedia()
        {
            SetDeviceState(DeviceState.LaunchingApplication, null);
            Connect();
            if (IsConnected())
                Launch();
        }

        public void ConnectAndLaunch()
        {
            Connect();
            Task.Delay(250);
            GetReceiverStatus();
        }

        private void Connect(string sourceId = null, string destinationId = null)
        {
            SendMessage(chromeCastMessages.GetConnectMessage(sourceId, destinationId));
        }

        private void Close(string sourceId = null, string destinationId = null)
        {
            SendMessage(chromeCastMessages.GetConnectMessage(sourceId, destinationId));
        }

        private void Launch()
        {
            SendMessage(chromeCastMessages.GetLaunchMessage(GetNextRequestId()));
        }

        public void LoadMedia()
        {
            SetDeviceState(DeviceState.LoadingMedia, null);
            SendMessage(chromeCastMessages.GetLoadMessage(applicationLogic.GetStreamingUrl(), chromeCastSource, chromeCastDestination));
        }

        public void PauseMedia()
        {
            SetDeviceState(DeviceState.Paused, null);
            SendMessage(chromeCastMessages.GetPauseMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public void PlayMedia()
        {
            SetDeviceState(DeviceState.Playing, null);
            SendMessage(chromeCastMessages.GetPlayMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public void VolumeSet(float level)
        {
            if (lastVolumeChange != null && DateTime.Now.Ticks - lastVolumeChange.Ticks < 1000)
                return;

            lastVolumeChange = DateTime.Now;

            if (volumeSetting.level > level)
                while (volumeSetting.level > level) volumeSetting.level -= volumeSetting.stepInterval;
            if (volumeSetting.level < level)
                while (volumeSetting.level < level) volumeSetting.level += volumeSetting.stepInterval;
            if (level > 1) level = 1;
            if (level < 0) level = 0;

            volumeSetting.level = level;

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

        private void Pong()
        {
            SendMessage(chromeCastMessages.GetPongMessage());
        }

        public void GetReceiverStatus()
        {
            SendMessage(chromeCastMessages.GetReceiverStatusMessage(GetNextRequestId()));
        }

        public void GetMediaStatus()
        {
            if (!IsConnected())
                ConnectAndLaunch();

            SendMessage(chromeCastMessages.GetMediaStatusMessage(GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public bool Stop()
        {
            var previousState = deviceState;
            switch (deviceState)
            {
                case DeviceState.Playing:
                case DeviceState.LoadingMedia:
                case DeviceState.Buffering:
                case DeviceState.Paused:
                    SendMessage(chromeCastMessages.GetStopMessage(chromeCastApplicationSessionNr, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
                    break;
                default:
                    break;
            }
            SetDeviceState(DeviceState.Closed);
            return previousState.Equals(DeviceState.Playing);
        }

        private int GetNextRequestId()
        {
            return ++requestId;
        }

        private void SendMessage(CastMessage castMessage)
        {
            var byteMessage = chromeCastMessages.MessageToByteArray(castMessage);
            deviceConnection.SendMessage(byteMessage);

            logger.Log($"{Properties.Strings.Log_Out} [{DateTime.Now.ToLongTimeString()}][{getHost?.Invoke()}]: {castMessage.PayloadUtf8}");
        }

        private async void OnReceiveMessage(CastMessage castMessage)
        {
            if (deviceState == DeviceState.Disposed)
                return;

            logger.Log($"{Properties.Strings.Log_In} [{DateTime.Now.ToLongTimeString()}] [{getHost?.Invoke()}]: {castMessage.PayloadUtf8}");
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
                    var previousState = deviceState;
                    var closeMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    Stop();
                    Close();
                    SetDeviceState(DeviceState.Closed, null);
                    deviceConnection.Dispose();

                    // Restart
                    if (applicationLogic.GetAutoRestart() && previousState == DeviceState.Playing)
                    {
                        await Task.Delay(5000);
                        OnPlayPause_Click();
                    }
                    else
                    {
                        await Task.Delay(500);
                        ConnectAndLaunch();
                    }
                    break;
                case "LOAD_FAILED":
                    var loadFailedMessage = js.Deserialize<MessageLoadFailed>(castMessage.PayloadUtf8);
                    SetDeviceState(DeviceState.LoadFailed, null);
                    break;
                case "LOAD_CANCELLED":
                    var loadCancelledMessage = js.Deserialize<MessageLoadCancelled>(castMessage.PayloadUtf8);
                    SetDeviceState(DeviceState.LoadCancelled, null);
                    break;
                case "INVALID_REQUEST":
                    var invalidRequestMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    SetDeviceState(DeviceState.InvalidRequest, null);
                    break;
                default:
                    break;
            }
        }

        private void SetDeviceState(DeviceState state, string text = null)
        {
            deviceState = state;
            setControlDeviceState?.Invoke(state, text);
        }

        private void OnReceiveMediaStatus(MessageMediaStatus mediaStatusMessage)
        {
            chromeCastMediaSessionId = mediaStatusMessage.status.Any() ? mediaStatusMessage.status.First().mediaSessionId : 1;

            if (deviceConnection.IsConnected() && mediaStatusMessage.status.Any())
            {
                switch (mediaStatusMessage.status.First().playerState)
                {
                    case "IDLE":
                        SetDeviceState(DeviceState.Idle, null);
                        break;
                    case "BUFFERING":
                        SetDeviceState(DeviceState.Buffering, GetPlayingTime(mediaStatusMessage));
                        break;
                    case "PAUSED":
                        SetDeviceState(DeviceState.Paused, null);
                        break;
                    case "PLAYING":
                        SetDeviceState(DeviceState.Playing, GetPlayingTime(mediaStatusMessage));
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
            if (receiverStatusMessage != null && receiverStatusMessage.status != null && receiverStatusMessage.status.applications != null)
            {
                OnVolumeUpdate(receiverStatusMessage.status.volume);

                var deviceApplication = receiverStatusMessage.status.applications.Where(a => a.appId.Equals("CC1AD845"));
                if (deviceApplication.Any())
                {
                    chromeCastDestination = deviceApplication.First().transportId;
                    chromeCastApplicationSessionNr = deviceApplication.First().sessionId;

                    if (deviceState.Equals(DeviceState.LaunchingApplication))
                    {
                        SetDeviceState(DeviceState.LaunchedApplication, null);
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

        private void OnVolumeUpdate(Volume volume)
        {
            onVolumeUpdate(volume);
            volumeSetting = volume;
        }

        public void SetCallback(Action<DeviceState, string> setDeviceStateIn, Action<Volume> onVolumeUpdateIn,
            Func<string> getHostIn)
        {
            setControlDeviceState = setDeviceStateIn;
            onVolumeUpdate = onVolumeUpdateIn;
            getHost = getHostIn;
            deviceConnection.SetCallback(getHostIn, setDeviceStateIn, OnReceiveMessage, OnClosed);
        }

        private void OnClosed()
        {
            SetDeviceState(DeviceState.Closed);
        }

        public void OnPlayPause_Click()
        {
            switch (deviceState)
            {
                case DeviceState.Buffering:
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

        public DeviceState GetDeviceState()
        {
            return deviceState;
        }

        private bool IsConnected()
        {
            return !(deviceState.Equals(DeviceState.NotConnected) ||
                deviceState.Equals(DeviceState.ConnectError) ||
                deviceState.Equals(DeviceState.Closed));
        }

        public void VolumeUp()
        {
            VolumeSet(volumeSetting.level + 0.05f);
        }

        public void VolumeDown()
        {
            VolumeSet(volumeSetting.level - 0.05f);
        }

        public void VolumeMute()
        {
            if (deviceConnection.IsConnected())
                SendMessage(chromeCastMessages.GetVolumeMuteMessage(!volumeSetting.muted, GetNextRequestId()));
        }

        public void Close()
        {
            SendMessage(chromeCastMessages.GetCloseMessage());
        }

        public void Dispose()
        {
            Close();
            SetDeviceState(DeviceState.Disposed, null);
        }
    }

    public class VolumeSetItem
    {
        public Volume Setting { get; set; }
        public int RequestId { get; set; }
        public DateTime SendAt { get; set; }
    }
}
