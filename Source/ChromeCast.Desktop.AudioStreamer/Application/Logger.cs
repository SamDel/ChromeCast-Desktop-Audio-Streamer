using System;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Logger : ILogger
    {
        private Action<string> logCallback;

        public void Log(string message)
        {
            logCallback?.Invoke(message);
        }

        public void SetCallback(Action<string> logCallbackIn)
        {
            logCallback = logCallbackIn;
        }
    }
}