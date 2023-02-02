using Microsoft.Win32;
using System;
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
                    rk.SetValue("Desktop Audio Streamer", System.Windows.Forms.Application.ExecutablePath);
                else
                    rk.DeleteValue("Desktop Audio Streamer", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
