using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public static class WindowsStartup
    {
        public static void StartApplicationWhenWindowsStarts(bool value)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey 
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (value)
                    rk.SetValue(Properties.Strings.MainForm_Text, Assembly.GetExecutingAssembly().Location);
                else
                    rk.DeleteValue(Properties.Strings.MainForm_Text, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
