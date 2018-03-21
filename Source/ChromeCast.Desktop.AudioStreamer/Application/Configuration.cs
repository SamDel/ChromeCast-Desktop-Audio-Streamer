using System;
using System.Configuration;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Configuration : IConfiguration
    {
        public void Load(Action<bool, bool, bool, int, bool, string, bool> configurationCallback)
        {
            try
            {
                string useKeyboardShortCuts = ConfigurationManager.AppSettings["UseKeyboardShortCuts"];
                string showLog = ConfigurationManager.AppSettings["ShowLog"];
                string showLagControl = ConfigurationManager.AppSettings["ShowLagControl"];
                string lagControlValue = ConfigurationManager.AppSettings["LagControlValue"];
                string autoStartDevices = ConfigurationManager.AppSettings["AutoStartDevices"];
                string ipAddressesDevices = ConfigurationManager.AppSettings["IpAddressesDevices"];
                string showWindowOnStart = ConfigurationManager.AppSettings["ShowWindowOnStart"];

                bool.TryParse(useKeyboardShortCuts, out bool useShortCuts);
                bool.TryParse(showLog, out bool boolShowLog);
                bool.TryParse(showLagControl, out bool showLag);
                int.TryParse(lagControlValue, out int lagValue);
                bool.TryParse(autoStartDevices, out bool autoStart);
                bool.TryParse(showWindowOnStart, out bool showWindow);

                configurationCallback(useShortCuts, boolShowLog, showLag, lagValue, autoStart, ipAddressesDevices, showWindow);
            }
            catch (Exception)
            {
            }
        }
    }
}
