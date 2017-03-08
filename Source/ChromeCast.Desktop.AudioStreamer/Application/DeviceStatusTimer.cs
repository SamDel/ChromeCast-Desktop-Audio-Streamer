using System;
using System.Timers;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class DeviceStatusTimer : IDeviceStatusTimer
    {
        public const int Interval = 10000;
        private Action onGetStatus;
        private Timer timer;

        public void StartPollingDevice(Action onGetStatusIn)
        {
            onGetStatus = onGetStatusIn;

            timer = new Timer();
            timer.Interval = Interval;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(OnGetStatus);
            timer.Start();
        }

        private void OnGetStatus(object sender, ElapsedEventArgs e)
        {
            onGetStatus?.Invoke();
        }
    }
}