using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IChromeCastMessages
    {
        CastMessage GetConnectMessage(string sourceId, string destinationId);
        CastMessage GetCloseMessage();
        CastMessage GetLaunchMessage(int requestId);
        CastMessage GetLoadMessage(string streamUrl, string chromeCastSource, string chromeCastDestination, int requestId, string streamTitle);
        CastMessage GetStopMessage(string chromeCastSessionId, int chromeCastMediaSessionId, int requestId, string chromeCastSource, string chromeCastDestination);
        CastMessage GetPauseMessage(string chromeCastSessionId, int chromeCastMediaSessionId, int requestId, string chromeCastSource, string chromeCastDestination);
        CastMessage GetPlayMessage(string chromeCastApplicationSessionNr, int chromeCastMediaSessionId, int v, string chromeCastSource, string chromeCastDestination);
        CastMessage GetVolumeSetMessage(Volume volumeSetting, int requestId, string sourceId = null, string destinationId = null);
        CastMessage GetVolumeMuteMessage(bool muted, int requestId, string sourceId = null, string destinationId = null);
        CastMessage GetPongMessage();
        CastMessage GetReceiverStatusMessage(int requestId);
        CastMessage GetMediaStatusMessage(int requestId, string chromeCastSource, string chromeCastDestination);
        byte[] MessageToByteArray(CastMessage castMessage);
    }
}