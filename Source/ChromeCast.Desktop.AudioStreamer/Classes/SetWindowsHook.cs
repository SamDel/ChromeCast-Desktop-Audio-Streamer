using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class SetWindowsHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static IDevices devices;
        private static readonly LowLevelKeyboardProc callbackProcedure = HookCallback;
        private static IntPtr hookId = IntPtr.Zero;
        private static bool isPressedInCtrl = false;
        private static bool isPressedInAlt = false;
        private static bool isPressedInU = false;
        private static bool isPressedInD = false;
        private static bool isPressedInM = false;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Start using hte hook.
        /// </summary>
        /// <param name="devicesIn">devices object that's used to trigger the events</param>
        public static void Start(IDevices devicesIn)
        {
            devices = devicesIn;

            try
            {
                hookId = SetHook(callbackProcedure);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Stop using the hooks.
        /// </summary>
        public static void Stop()
        {
            try
            {
                UnhookWindowsHookEx(hookId);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Set the hooks on the system.
        /// </summary>
        /// <param name="proc">the callback</param>
        /// <returns></returns>
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        /// <summary>
        /// Callback function for the system hooks, the key combinations are detected here.
        /// </summary>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var isKeyDown = wParam == (IntPtr)WM_KEYDOWN;
            var isKeyUp = wParam == (IntPtr)WM_KEYUP;
            if (nCode >= 0 && (isKeyDown || isKeyUp))
            {
                var key = (Keys)Marshal.ReadInt32(lParam);
                switch (key)
                {
                    case Keys.U:
                        if (isKeyDown)
                            isPressedInU = true;
                        else
                            isPressedInU = false;
                        break;
                    case Keys.D:
                        isPressedInD = isKeyDown ? true : false;
                        break;
                    case Keys.M:
                        isPressedInM = isKeyDown ? true : false;
                        break;
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        isPressedInCtrl = isKeyDown ? true : false;
                        break;
                    case Keys.LMenu:
                    case Keys.RMenu:
                        isPressedInAlt = isKeyDown ? true : false;
                        break;
                    default:
                        break;
                }
                if (isPressedInCtrl && isPressedInAlt)
                {
                    if (isPressedInU) devices.VolumeUp();
                    if (isPressedInD) devices.VolumeDown();
                    if (isPressedInM) devices.VolumeMute();
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
