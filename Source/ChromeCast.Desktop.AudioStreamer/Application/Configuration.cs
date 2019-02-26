using System;
using System.Configuration;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// Load the configuration from app.config.
        /// </summary>
        /// <param name="configurationCallback">callback to process the configuration</param>
        public void Load(Action<string> configurationCallback, ILogger loggerIn)
        {
            try
            {
                configurationCallback(ConfigurationManager.AppSettings["IpAddressesDevices"]);
            }
            catch (Exception ex)
            {
                loggerIn.Log(ex, "Configuration.Load");
            }
        }
    }
}
