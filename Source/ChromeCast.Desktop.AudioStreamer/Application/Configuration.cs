using System;
using System.Configuration;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Configuration : IConfiguration
    {
        public void Load(Action<bool, bool, int, string> configurationCallback)
        {
            try
            {
                string showLog = ConfigurationManager.AppSettings["ShowLog"];
                string showLagControl = ConfigurationManager.AppSettings["ShowLagControl"];
                string lagControlValue = ConfigurationManager.AppSettings["LagControlValue"];
                string ipAddressesDevices = ConfigurationManager.AppSettings["IpAddressesDevices"];

                bool.TryParse(showLog, out bool boolShowLog);
                bool.TryParse(showLagControl, out bool showLag);
                int.TryParse(lagControlValue, out int lagValue);

                configurationCallback(boolShowLog, showLag, lagValue, ipAddressesDevices);
            }
            catch (Exception)
            {
            }
        }
    }
}
