using System;
using System.Drawing;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;

namespace ChromeCast.Desktop.AudioStreamer.UserControls
{
    public partial class DeviceControl : UserControl
    {
        private IDevice device;
        private EventHandler StartPause_Click;

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

        public string GetDeviceName()
        {
            return btnDevice.Text;
        }

        public void SetStatus(DeviceState state, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DeviceState, string>(SetStatus), new object[] { state, text });
                return;
            }

            lblStatus.Text = $"{Resource.Get(state.ToString())} {text}";

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
                    picturePlayPause.Image = Properties.Resources.Play;
                    break;
                case DeviceState.Buffering:
                case DeviceState.Playing:
                    SetBackColor(Color.PaleGreen);
                    device.GetMenuItem().Checked = true;
                    picturePlayPause.Image = Properties.Resources.Pause;
                    break;
                case DeviceState.ConnectError:
                case DeviceState.LoadCancelled:
                case DeviceState.LoadFailed:
                case DeviceState.InvalidRequest:
                    SetBackColor(Color.PeachPuff);
                    device.GetMenuItem().Checked = false;
                    picturePlayPause.Image = Properties.Resources.Play;
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
            StartPause_Click = handler;
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
            pictureVolumeMute.Enabled = true;
            int change = (int)(volume.stepInterval * 100);
            trbVolume.LargeChange = change;
            trbVolume.SmallChange = change;
            trbVolume.TickFrequency = change;
            if (volume.muted)
                pictureVolumeMute.Image = Properties.Resources.Mute;
            else
                pictureVolumeMute.Image = Properties.Resources.Unmute;
        }

        private void trbVolume_Scroll(object sender, EventArgs e)
        {
            device.VolumeSet(trbVolume.Value / 100f);
        }

        private void pictureVolumeMute_Click(object sender, EventArgs e)
        {
            device.VolumeMute();
        }

        private void DeviceControl_MouseDown(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            control.DoDragDrop(control, DragDropEffects.Move);
        }

        private void DeviceChildControl_MouseDown(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            control.DoDragDrop(control.Parent, DragDropEffects.Move);
        }

        private void DeviceControl_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void DeviceControl_DragDrop(object sender, DragEventArgs e)
        {
            ((IMainForm)((DeviceControl)sender).ParentForm).DoDragDrop(sender, e);
        }

        private void btnDevice_Click(object sender, EventArgs e)
        {
            StartPause_Click(sender, e);
        }
    }
}
