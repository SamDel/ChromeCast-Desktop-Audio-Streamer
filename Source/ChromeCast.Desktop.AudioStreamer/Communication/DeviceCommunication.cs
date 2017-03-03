using System;
using System.Linq;
using System.Web.Script.Serialization;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceCommunication
    {
        private ChromeCastMessages chromeCastMessages;
        private ApplicationLogic application;
        private Device device;
        private string chromeCastDestination;
        private string chromeCastSource;
        private string chromeCastSessionId;
        private int chromeCastMediaSessionId;
        private int requestId;

        public DeviceCommunication(Device deviceIn, ApplicationLogic app)
        {
            device = deviceIn;
            application = app;
            chromeCastDestination = string.Empty;
            chromeCastSource = string.Format("client-8{0}", new Random().Next(10000, 99999));
            chromeCastMessages = new ChromeCastMessages();
            requestId = 0;
        }

        public void LaunchAndLoadMedia()
        {
            device.SetDeviceState(DeviceState.LaunchingApplication);
            Connect();
            if (device.DeviceConnection.IsConnected())
            {
                Launch();
            }
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
            device.SetDeviceState(DeviceState.LoadingMedia);
            SendMessage(chromeCastMessages.GetLoadMessage(application.GetStreamingUrl(), chromeCastSource, chromeCastDestination));
        }

        public void PauseMedia()
        {
            device.SetDeviceState(DeviceState.Paused);
            SendMessage(chromeCastMessages.GetPauseMessage(chromeCastSessionId, chromeCastMediaSessionId, GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public void VolumeSet(Volume volumeSetting)
        {
            if (device.IsConnected())
                SendMessage(chromeCastMessages.GetVolumeSetMessage(volumeSetting, GetNextRequestId()));
        }

        public void VolumeMute(bool muted)
        {
            if (device.IsConnected())
                SendMessage(chromeCastMessages.GetVolumeMuteMessage(muted, GetNextRequestId()));
        }

        public void Pong()
        {
            SendMessage(chromeCastMessages.GetPongMessage());
        }

        public void GetReceiverStatus()
        {
            SendMessage(chromeCastMessages.GetReceiverStatusMessage(GetNextRequestId()));
        }

        public void GetMediaStatus()
        {
            SendMessage(chromeCastMessages.GetMediaStatusMessage(GetNextRequestId(), chromeCastSource, chromeCastDestination));
        }

        public int GetNextRequestId()
        {
            requestId++;
            return requestId;
        }

        public void SendMessage(CastMessage.Builder castMessage)
        {
            var byteMessage = chromeCastMessages.MessageToByteArray(castMessage);
            device.DeviceConnection.SendMessage(byteMessage);

            if (application != null) application.Log(string.Format("out [{2}][{0}]: {1}", device.DiscoveredSsdpDevice.DescriptionLocation.Host, castMessage.PayloadUtf8, DateTime.Now.ToLongTimeString()));
        }

        public void OnReceiveMessage(CastMessage castMessage)
        {
            if (application != null && castMessage != null) application.Log(string.Format("in [{2}] [{0}]: {1}", device.DiscoveredSsdpDevice.DescriptionLocation.Host, castMessage.PayloadUtf8, DateTime.Now.ToLongTimeString()));
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
                    var closeMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    device.SetDeviceState(DeviceState.Closed);
                    break;
                case "LOAD_FAILED":
                    var loadFailedMessage = js.Deserialize<MessageLoadFailed>(castMessage.PayloadUtf8);
                    break;
                case "LOAD_CANCELLED":
                    var loadCancelledMessage = js.Deserialize<MessageLoadCancelled>(castMessage.PayloadUtf8);
                    break;
                case "INVALID_REQUEST":
                    var invalidRequestMessage = js.Deserialize<PayloadMessageBase>(castMessage.PayloadUtf8);
                    break;
                default:
                    break;
            }
        }

        private void OnReceiveMediaStatus(MessageMediaStatus mediaStatusMessage)
        {
            chromeCastMediaSessionId = mediaStatusMessage.status.Any() ? mediaStatusMessage.status.First().mediaSessionId : 1;

            if (device.IsConnected() && mediaStatusMessage.status.Any())
            {
                switch (mediaStatusMessage.status.First().playerState)
                {
                    case "IDLE":
                        device.SetDeviceState(DeviceState.Idle);
                        break;
                    case "BUFFERING":
                        device.SetDeviceState(DeviceState.Buffering);
                        break;
                    case "PAUSED":
                        device.SetDeviceState(DeviceState.Paused);
                        break;
                    case "PLAYING":
                        device.SetDeviceState(DeviceState.Playing);
                        var seconds = (int)(mediaStatusMessage.status.First().currentTime % 60);
                        var minutes = ((int)(mediaStatusMessage.status.First().currentTime) % 3600) / 60;
                        var hours = ((int)mediaStatusMessage.status.First().currentTime) / 3600;
                        device.SetDeviceState(DeviceState.Playing, string.Format("{0}:{1}:{2}", hours, minutes.ToString("D2"), seconds.ToString("D2")));
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnReceiveReceiverStatus(MessageReceiverStatus receiverStatusMessage)
        {
            if (receiverStatusMessage != null && receiverStatusMessage.status != null && receiverStatusMessage.status.applications != null)
            {
                device.OnVolumeUpdate(receiverStatusMessage.status.volume);

                var deviceApplication = receiverStatusMessage.status.applications.Where(a => a.appId.Equals("CC1AD845"));
                if (deviceApplication.Any())
                {
                    chromeCastDestination = deviceApplication.First().transportId;
                    chromeCastSessionId = deviceApplication.First().sessionId;

                    if (device.DeviceState.Equals(DeviceState.LaunchingApplication))
                    {
                        device.SetDeviceState(DeviceState.LaunchedApplication);
                        Connect(chromeCastSource, chromeCastDestination);
                        LoadMedia();
                    }
                }
            }
        }
    }
}
