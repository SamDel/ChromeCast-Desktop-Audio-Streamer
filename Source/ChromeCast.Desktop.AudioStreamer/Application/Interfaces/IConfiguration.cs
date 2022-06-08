using System;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface IConfiguration
    {
        void Load(Action<string, string, bool> configurationCallback, ILogger logger);
    }
}