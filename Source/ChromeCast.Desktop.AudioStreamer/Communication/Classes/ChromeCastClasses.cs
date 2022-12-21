using System.Collections.Generic;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Classes
{
    /// <summary>
    /// Classes used for messages received from a Chromecast device.
    /// </summary>
    public class PayloadMessageBase
    {
        public string type { get; set; }
    }

    public class MessageVolume : PayloadMessageBase
    {
        public SendVolume volume { get; set; }
        public int requestId { get; set; }
    }

    public class SendVolume
    {
        public float level { get; set; }
    }

    public class MessageVolumeMute : PayloadMessageBase
    {
        public SendVolumeMute volume { get; set; }
        public int requestId { get; set; }
    }

    public class SendVolumeMute
    {
        public bool muted { get; set; }
    }

    public class MessageLaunch : PayloadMessageBase
    {
        public string appId { get; set; }
        public int requestId { get; set; }
    }

    public class MessageLoad : PayloadMessageBase
    {
        public bool autoplay { get; set; }
        public float currentTime { get; set; }
        public List<object> activeTrackIds { get; set; }
        public string repeatMode { get; set; }
        public Media media { get; set; }
        public int requestId { get; set; }
    }

    public class MessagePause : PayloadMessageBase
    {
        public int mediaSessionId { get; set; }
        public string sessionId { get; set; }
        public int requestId { get; set; }
    }

    public class MessageStatus : PayloadMessageBase
    {
        public int requestId { get; set; }
    }

    public class Media
    {
        public string contentId { get; set; }
        public string contentType { get; set; }
        public string streamType { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Metadata
    {
        public int type { get; set; }
        public int metadataType { get; set; }
        public string title { get; set; }
        public List<Image> images { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }

    /// <summary>
    /// If the stream couldn't be opened (socket problems etc.) you get this message from the device.
    /// type = 'LOAD_FAILED'
    /// </summary>
    public class MessageLoadFailed : PayloadMessageBase
    {
        public int requestId { get; set; }
    }

    /// <summary>
    /// If calling 'LOAD' a second time with the same url and the first is still 'loading' 
    /// , you get this message from the device.
    /// type = 'LOAD_CANCELLED'
    /// </summary>
    public class MessageLoadCancelled : PayloadMessageBase
    {
        public int requestId { get; set; }
    }

    /// <summary>
    /// You get this message after LOAD, Volume change, Mute etc., and when you request the status.
    /// type = 'MEDIA_STATUS'
    /// </summary>
    public class MessageMediaStatus : PayloadMessageBase
    {
        public List<MediaStatus> status { get; set; }
        public int requestId { get; set; }
    }

    public class MediaStatus
    {
        public int mediaSessionId { get; set; }
        public int playbackRate { get; set; }
        public string playerState { get; set; }
        public float currentTime { get; set; }
        public int supportedMediaCommands { get; set; }
        public Volume volume { get; set; }
        public List<object> activeTrackIds { get; set; }
        public Media media { get; set; }
        public int currentItemId { get; set; }
        public ExtendedStatus extendedStatus { get; set; }
        public string repeatMode { get; set; }
    }

    public class ExtendedStatus
    {
        public string playerState { get; set; }
        public Media media { get; set; }
    }

    /// <summary>
    /// After a 'LAUNCH' you get this message from the device, or when you request the device for it.
    /// type = 'RECEIVER_STATUS'
    /// </summary>
    public class MessageReceiverStatus : PayloadMessageBase
    {
        public int requestId { get; set; }
        public ReceiverStatus status { get; set; }
    }

    public class ReceiverStatus
    {
        public List<Application> applications { get; set; }
        public Volume volume { get; set; }
    }

    public class Volume
    {
        public string controlType { get; set; }
        public float level { get; set; }
        public bool muted { get; set; }
        public float stepInterval { get; set; }
    }

    public class Application
    {
        public string appId { get; set; }
        public string displayName { get; set; }
        public bool isIdleScreen { get; set; }
        public List<Namespaces> namespaces { get; set; }
        public string sessionId { get; set; }
        public string statusText { get; set; }
        public string transportId { get; set; }
    }

    public class Namespaces
    {
        public string name { get; set; }
    }
}