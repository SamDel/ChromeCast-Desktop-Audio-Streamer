namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public enum DeviceState
    {
        NotConnected,
        Idle,
        Disposed,
        LaunchingApplication,
        LaunchedApplication,
        LoadingMedia,
        Buffering,
        Playing,
        Paused,
        ConnectError,
        LoadFailed,
        LoadCancelled,
        InvalidRequest,
        Closed,
        Connected
    };
}
