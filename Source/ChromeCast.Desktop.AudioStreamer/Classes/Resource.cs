using System.Globalization;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public static class Resource
    {
        public static string Get(string id)
        {
            return Properties.Strings.ResourceManager.GetString(id, CultureInfo.CurrentUICulture);
        }
    }
}
