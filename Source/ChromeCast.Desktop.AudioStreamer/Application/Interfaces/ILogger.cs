using System;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void SetCallback(Action<string> logCallbackIn);
    }
}