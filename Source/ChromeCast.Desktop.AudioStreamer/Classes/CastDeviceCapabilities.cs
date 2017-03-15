using System;
using System.Web.Script.Serialization;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class CastDeviceCapabilities
    {
        public bool bluetooth_supported;
        public bool display_supported;
        public bool hi_res_audio_supported;
    }

    /// <summary>
    /// When connecting to a stream the device adds a http header with his capabilities.
    /// </summary>
    public static class CastDeviceCapabilitiesHelper
    {
        public static CastDeviceCapabilities GetCastDeviceCapabilities(string content)
        {
            if (content != null)
            {
                content.Replace('\r', '\n').Replace("\n\n", "\n");
                var lines = content.Split('\n');
                foreach (var line in lines)
                {
                    var header = line.Split(':');
                    if (header.Length == 2
                        && header[0].IndexOf("CAST-DEVICE-CAPABILITIES", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        try
                        {
                            return new JavaScriptSerializer().Deserialize<CastDeviceCapabilities>(header[1].Trim());
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return null;
        }
    }
}
