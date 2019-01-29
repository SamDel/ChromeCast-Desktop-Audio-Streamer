using System;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Logger : ILogger
    {
        private Action<string> logCallback;

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">the message to log</param>
        public void Log(string message)
        {
            logCallback?.Invoke(message);
        }

        /// <summary>
        /// Set the callback that does the actual logging.
        /// </summary>
        public void SetCallback(Action<string> logCallbackIn)
        {
            logCallback = logCallbackIn;
        }
    }
}