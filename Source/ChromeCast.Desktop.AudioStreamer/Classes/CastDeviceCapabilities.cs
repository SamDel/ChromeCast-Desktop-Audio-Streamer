using System;
using System.Text.Json;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    /// <summary>
    /// The capabilities of a device.
    /// </summary>
    public class CastDeviceCapabilities
    {
        public bool bluetooth_supported;
        public bool display_supported;
        public bool hi_res_audio_supported;
    }

    /// <summary>
    /// When connecting to a stream the device adds a http header with his capabilities.
    /// It's not now, maybe for later use.
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
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            return JsonSerializer.Deserialize<CastDeviceCapabilities>(header[1].Trim(), options);
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
