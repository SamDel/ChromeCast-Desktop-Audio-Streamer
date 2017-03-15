using System.Collections.Generic;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Classes
{
    public class PayloadMessageBase
    {
        public string type;
    }

    public class MessageVolume : PayloadMessageBase
    {
        public SendVolume volume;
        public int requestId;
    }

    public class SendVolume
    {
        public float level;
    }

    public class MessageVolumeMute : PayloadMessageBase
    {
        public SendVolumeMute volume;
        public int requestId;
    }

    public class SendVolumeMute
    {
        public bool muted;
    }

    public class MessageLaunch : PayloadMessageBase
    {
        public string appId;
        public int requestId;
    }

    public class MessageLoad : PayloadMessageBase
    {
        public bool autoplay;
        public float currentTime;
        public List<object> activeTrackIds;
        public string repeatMode;
        public Media media;
        public int requestId;
    }

    public class MessagePause : PayloadMessageBase
    {
        public int mediaSessionId;
        public string sessionId;
        public int requestId;
    }

    public class MessageStatus : PayloadMessageBase
    {
        public int requestId;
    }

    public class Media
    {
        public string contentId;
        public string contentType;
        public string streamType;
        public Metadata metadata;
    }

    public class Metadata
    {
        public int type;
        public int metadataType;
        public string title;
        public List<Image> images;
    }

    public class Image
    {
        public string url;
    }

    /// <summary>
    /// If the stream couldn't be opened (socket problems etc.) you get this message from the device.
    /// type = 'LOAD_FAILED'
    /// </summary>
    public class MessageLoadFailed : PayloadMessageBase
    {
        public int requestId;
    }

    /// <summary>
    /// If calling 'LOAD' a second time with the same url and the first is still 'loading' (application.statusText 
    /// = 'Now Casting...'), you get this message from the device
    /// type = 'LOAD_CANCELLED'
    /// </summary>
    public class MessageLoadCancelled : PayloadMessageBase
    {
        public int requestId;
    }

    /// <summary>
    /// You get this message after LOAD, Volume change, Mute etc., and when you request the status.
    /// type = 'MEDIA_STATUS'
    /// </summary>
    public class MessageMediaStatus : PayloadMessageBase
    {
        public List<MediaStatus> status;
        public int requestId;
    }

    public class MediaStatus
    {
        public int mediaSessionId;
        public int playbackRate;
        public string playerState;
        public float currentTime;
        public int supportedMediaCommands;
        public Volume volume;
        public List<object> activeTrackIds;
        public Media media;
        public int currentItemId;
        public ExtendedStatus extendedStatus;
        public string repeatMode;
    }

    public class ExtendedStatus
    {
        public string playerState;
        public Media media;
    }

    /// <summary>
    /// After a 'LAUNCH' you get this message from the device, or when you request the device for it.
    /// type = 'RECEIVER_STATUS'
    /// </summary>
    public class MessageReceiverStatus : PayloadMessageBase
    {
        public int requestId;
        public ReceiverStatus status;
    }

    public class ReceiverStatus
    {
        public List<Application> applications;
        public Volume volume;
    }

    public class Volume
    {
        public string controlType;
        public float level;
        public bool muted;
        public float stepInterval;
    }

    public class Application
    {
        public string appId;
        public string displayName;
        public bool isIdleScreen;
        public List<Namespaces> namespaces;
        public string sessionId;
        public string statusText;
        public string transportId;
    }

    public class Namespaces
    {
        public string name;
    }
}