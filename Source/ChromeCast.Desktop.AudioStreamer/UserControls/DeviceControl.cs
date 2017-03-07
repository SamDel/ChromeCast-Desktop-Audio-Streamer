using System;
using System.Drawing;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;

namespace ChromeCast.Desktop.AudioStreamer.UserControls
{
    public partial class DeviceControl : UserControl
    {
        private IDevice device;

        public DeviceControl(IDevice deviceIn)
        {
            InitializeComponent();
            device = deviceIn;
        }

        public void SetDeviceName(string name)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetDeviceName), new object[] { name });
                return;
            }

            btnDevice.Text = name;
        }

        public void SetStatus(DeviceState state, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DeviceState, string>(SetStatus), new object[] { state, text });
                return;
            }

            lblStatus.Text = string.Format("{0} {1}", state, text);
            btnDevice.Width = Width - 2;
            trbVolume.Width = Width - trbVolume.Left - 2;

            switch (state)
            {
                case DeviceState.NotConnected:
                case DeviceState.Idle:
                case DeviceState.Disposed:
                case DeviceState.LaunchingApplication:
                case DeviceState.LaunchedApplication:
                case DeviceState.LoadingMedia:
                case DeviceState.Closed:
                case DeviceState.Paused:
                    SetBackColor(Color.LightGray);
                    device.GetMenuItem().Checked = false;
                    break;
                case DeviceState.Buffering:
                case DeviceState.Playing:
                    SetBackColor(Color.PaleGreen);
                    device.GetMenuItem().Checked = true;
                    break;
                case DeviceState.ConnectError:
                case DeviceState.LoadCancelled:
                case DeviceState.LoadFailed:
                case DeviceState.InvalidRequest:
                    SetBackColor(Color.PeachPuff);
                    device.GetMenuItem().Checked = false;
                    break;
                default:
                    break;
            }
        }

        public void SetBackColor(Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Color>(SetBackColor), new object[] { color });
                return;
            }

            BackColor = color;
            btnDevice.BackColor = color;
            lblStatus.BackColor = color;
            Update();
        }

        public void SetClickCallBack(EventHandler handler)
        {
            btnDevice.Click += handler;
            lblStatus.Click += handler;
        }

        public void OnVolumeUpdate(Volume volume)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Volume>(OnVolumeUpdate), new object[] { volume });
                return;
            }
            if (IsDisposed) return;

            trbVolume.Value = (int)(volume.level * 100);
            trbVolume.Enabled = true;
            btnVolumeMute.Enabled = true;
            int change = (int)(volume.stepInterval * 100);
            trbVolume.LargeChange = change;
            trbVolume.SmallChange = change;
            trbVolume.TickFrequency = change;
            if (volume.muted)
                btnVolumeMute.Text = "U";
            else
                btnVolumeMute.Text = "M";
        }

        private void trbVolume_Scroll(object sender, EventArgs e)
        {
            device.VolumeSet(trbVolume.Value / 100f);
        }

        private void btnVolumeMute_Click(object sender, EventArgs e)
        {
            device.VolumeMute();
        }
    }
}
