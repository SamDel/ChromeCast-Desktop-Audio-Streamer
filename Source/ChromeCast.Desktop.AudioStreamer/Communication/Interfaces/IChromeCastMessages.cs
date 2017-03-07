using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IChromeCastMessages
    {
        CastMessage.Builder GetConnectMessage(string sourceId, string destinationId);
        CastMessage.Builder GetLaunchMessage(int requestId);
        CastMessage.Builder GetLoadMessage(string streamUrl, string chromeCastSource, string chromeCastDestination);
        CastMessage.Builder GetPauseMessage(string chromeCastSessionId, int chromeCastMediaSessionId, int requestId, string chromeCastSource, string chromeCastDestination);
        CastMessage.Builder GetVolumeSetMessage(Volume volumeSetting, int requestId, string sourceId = null, string destinationId = null);
        CastMessage.Builder GetVolumeMuteMessage(bool muted, int requestId, string sourceId = null, string destinationId = null);
        CastMessage.Builder GetPongMessage();
        CastMessage.Builder GetReceiverStatusMessage(int requestId);
        CastMessage.Builder GetMediaStatusMessage(int requestId, string chromeCastSource, string chromeCastDestination);
        byte[] MessageToByteArray(CastMessage.Builder castMessage);
    }
}