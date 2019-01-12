using System;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface IConfiguration
    {
        void Load(Action<bool, bool, int, string> configurationCallback);
    }
}