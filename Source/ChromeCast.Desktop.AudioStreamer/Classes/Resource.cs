using System.Globalization;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public static class Resource
    {
        /// <summary>
        /// Translate a resource to the current culture.
        /// </summary>
        /// <param name="id">the id of the text to translate</param>
        /// <returns>the translated text</returns>
        public static string Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            return Properties.Strings.ResourceManager.GetString(id, CultureInfo.CurrentUICulture);
        }
    }
}
