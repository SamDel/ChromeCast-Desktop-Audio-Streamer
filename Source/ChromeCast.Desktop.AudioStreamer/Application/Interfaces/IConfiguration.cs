using System;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface IConfiguration
    {
        void Load(Action<string, bool> configurationCallback, ILogger logger);
    }
}