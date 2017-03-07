using System;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class DeviceStatusTimer : IDeviceStatusTimer
    {
        private Action onGetStatus;
        private Timer timer;

        public void StartPollingDevice(Action onGetStatusIn)
        {
            onGetStatus = onGetStatusIn;

            timer = new Timer();
            timer.Interval = 10000;
            timer.Tick += new EventHandler(OnGetStatus);
            timer.Start();
        }

        private void OnGetStatus(object sender, EventArgs e)
        {
            onGetStatus?.Invoke();
        }
    }
}