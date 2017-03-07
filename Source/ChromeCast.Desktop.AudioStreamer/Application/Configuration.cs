using System;
using System.Configuration;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Configuration : IConfiguration
    {
        public void Load(Action<bool, bool, bool, int, bool, string> configurationCallback)
        {
            try
            {
                string useKeyboardShortCuts = ConfigurationManager.AppSettings["UseKeyboardShortCuts"];
                string showLog = ConfigurationManager.AppSettings["ShowLog"];
                string showLagControl = ConfigurationManager.AppSettings["ShowLagControl"];
                string lagControlValue = ConfigurationManager.AppSettings["LagControlValue"];
                string autoStartDevices = ConfigurationManager.AppSettings["AutoStartDevices"];
                string ipAddressesDevices = ConfigurationManager.AppSettings["IpAddressesDevices"];

                bool useShortCuts = false;
                bool boolShowLog = false;
                bool showLag = false;
                int lagValue = 1000;
                bool autoStart = false;
                bool.TryParse(useKeyboardShortCuts, out useShortCuts);
                bool.TryParse(showLog, out boolShowLog);
                bool.TryParse(showLagControl, out showLag);
                int.TryParse(lagControlValue, out lagValue);
                bool.TryParse(autoStartDevices, out autoStart);

                configurationCallback(useShortCuts, boolShowLog, showLag, lagValue, autoStart, ipAddressesDevices);
            }
            catch (Exception)
            {
            }
        }
    }
}
