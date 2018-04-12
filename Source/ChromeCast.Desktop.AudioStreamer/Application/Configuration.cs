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

                bool useShortCuts;
                bool boolShowLog;
                bool showLag;
                int lagValue;
                bool autoStart;
                bool showWindow;
                bool.TryParse(useKeyboardShortCuts, out useShortCuts);
                bool.TryParse(showLog, out boolShowLog);
                bool.TryParse(showLagControl, out showLag);
                int.TryParse(lagControlValue, out lagValue);
                bool.TryParse(autoStartDevices, out autoStart);
                bool.TryParse(showWindowOnStart, out showWindow);

                configurationCallback(useShortCuts, boolShowLog, showLag, lagValue, autoStart, ipAddressesDevices, showWindow);
            }
            catch (Exception)
            {
            }
        }
    }
}
