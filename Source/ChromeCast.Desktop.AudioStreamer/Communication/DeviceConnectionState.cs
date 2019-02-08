namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    /// <summary>
    /// States for the socket connection used for control messages.
    /// </summary>
    public enum DeviceConnectionState
    {
        None,
        Connecting,
        Connected,
        Error,
        Disconnected
    }
}
