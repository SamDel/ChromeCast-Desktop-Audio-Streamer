using System;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception ex, string message = null);
        void SetCallback(Action<string> logCallbackIn);
    }
}