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
        private Action PlayPause_Click;
        private Action Stop_Click;

        public DeviceControl(IDevice deviceIn)
        {
            InitializeComponent();
            device = deviceIn;
            btnDevice.FlatAppearance.MouseOverBackColor = btnDevice.BackColor;
            btnDevice.BackColorChanged += (s, e) =>
            {
                btnDevice.FlatAppearance.MouseOverBackColor = btnDevice.BackColor;
            };
        }

        public void SetDeviceName(string name)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetDeviceName), new object[] { name });
                return;
            }

            btnDevice.Text = name;
            pictureGroup.Visible = device.IsGroup();
            toolTipGroup.SetToolTip(pictureGroup, Properties.Strings.Tooltip_Group_Text);
        }

        public string GetDeviceName()
        {
            return btnDevice.Text;
        }

        public void SetStatus(DeviceState state, string text)
        {
            if (device == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<DeviceState, string>(SetStatus), new object[] { state, text });
                return;
            }

            lblStatus.Text = $"{Resource.Get(state.ToString())} {text}";
            var tmpMenuItem = device.GetMenuItem();

            switch (state)
            {
                case DeviceState.LoadingMediaCheckFirewall:
                    SetBackColor(Color.MistyRose);
                    lblStatus.Text = $"{Resource.Get(state.ToString())}";
                    if (tmpMenuItem != null) tmpMenuItem.Checked = false;
                    picturePlayPause.Image = Properties.Resources.Play;
                    break;
                case DeviceState.NotConnected:
                case DeviceState.Connected:
                case DeviceState.Idle:
                case DeviceState.Disposed:
                case DeviceState.LaunchingApplication:
                case DeviceState.LaunchedApplication:
                case DeviceState.LoadingMedia:
                case DeviceState.Closed:
                case DeviceState.Paused:
                    SetBackColor(Color.LightGray);
                    if (tmpMenuItem != null) tmpMenuItem.Checked = false;
                    picturePlayPause.Image = Properties.Resources.Play;
                    break;
                case DeviceState.Buffering:
                case DeviceState.Playing:
                    SetBackColor(Color.PaleGreen);
                    if (tmpMenuItem != null) tmpMenuItem.Checked = true;
                    picturePlayPause.Image = Properties.Resources.Stop;
                    break;
                case DeviceState.ConnectError:
                case DeviceState.LoadCancelled:
                case DeviceState.LoadFailed:
                case DeviceState.InvalidRequest:
                    SetBackColor(Color.PeachPuff);
                    if (tmpMenuItem != null) tmpMenuItem.Checked = false;
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

        public void SetClickCallBack(Action playPauseAction, Action stopAction)
        {
            PlayPause_Click = playPauseAction;
            Stop_Click = stopAction;
        }

        public void OnVolumeUpdate(Volume volume)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke(new Action<Volume>(OnVolumeUpdate), new object[] { volume });
                return;
            }

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

        private void TrbVolume_Scroll(object sender, EventArgs e)
        {
            if (device == null)
                return;

            device.VolumeSet(trbVolume.Value / 100f);
        }

        private void PictureVolumeMute_Click(object sender, EventArgs e)
        {
            if (device == null)
                return;

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
            if (e == null)
                return;

            e.Effect = DragDropEffects.All;
        }

        private void DeviceControl_DragDrop(object sender, DragEventArgs e)
        {
            if (sender == null || !(sender is DeviceControl))
                return;

            ((IMainForm)((DeviceControl)sender).ParentForm).DoDragDrop(sender, e);
        }

        private void BtnDevicePlay_Click(object sender, EventArgs e)
        {
            PlayPause_Click();
        }

        private void BtnDeviceStop_Click(object sender, EventArgs e)
        {
            Stop_Click();
        }
    }
}
