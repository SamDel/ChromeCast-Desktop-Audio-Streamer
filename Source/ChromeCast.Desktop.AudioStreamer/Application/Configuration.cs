using System;
using System.Configuration;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Configuration : IConfiguration
    {
        public void Load(Action<string> configurationCallback)
        {
            try
            {
                string ipAddressesDevices = ConfigurationManager.AppSettings["IpAddressesDevices"];

                configurationCallback(ipAddressesDevices);
            }
            catch (Exception)
            {
            }
        }
    }
}
