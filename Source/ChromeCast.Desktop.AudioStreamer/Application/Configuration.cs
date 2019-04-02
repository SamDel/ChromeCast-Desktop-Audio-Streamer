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
        public void Load(Action<string, bool> configurationCallback, ILogger loggerIn)
        {
            try
            {
                string showLagControl = ConfigurationManager.AppSettings["ShowLagControl"];
                bool.TryParse(showLagControl, out bool showLag);
                configurationCallback(ConfigurationManager.AppSettings["IpAddressesDevices"], showLag);
            }
            catch (Exception ex)
            {
                loggerIn.Log(ex, "Configuration.Load");
            }
        }
    }
}
