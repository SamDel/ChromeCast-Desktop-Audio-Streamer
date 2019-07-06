using System;
using System.Runtime.InteropServices;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public static class SystemVolume
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public static void Mute(bool mute, IMainForm form)
        {
            if (mute)
                Mute(form);
            else
                Unmute(form);
        }

        private static void Mute(IMainForm form)
        {
            // Make sure the volume unmuted.
            SendMessageW(form.GetHandle(), WM_APPCOMMAND, form.GetHandle(), (IntPtr)APPCOMMAND_VOLUME_UP);
            SendMessageW(form.GetHandle(), WM_APPCOMMAND, form.GetHandle(), (IntPtr)APPCOMMAND_VOLUME_DOWN);
            // Then mute
            SendMessageW(form.GetHandle(), WM_APPCOMMAND, form.GetHandle(), (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        private static void Unmute(IMainForm form)
        {
            // Make sure the volume unmuted.
            SendMessageW(form.GetHandle(), WM_APPCOMMAND, form.GetHandle(), (IntPtr)APPCOMMAND_VOLUME_UP);
            SendMessageW(form.GetHandle(), WM_APPCOMMAND, form.GetHandle(), (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }
    }
}
