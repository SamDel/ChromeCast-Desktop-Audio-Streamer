using System;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer
{
    public partial class MainForm : Form, IMainForm
    {
        private IApplicationLogic applicationLogic;
        private IDevices devices;
        private ILogger logger;

        public MainForm(IApplicationLogic applicationLogicIn, IDevices devicesIn, ILogger loggerIn)
        {
            InitializeComponent();

            applicationLogic = applicationLogicIn;
            devices = devicesIn;
            logger = loggerIn;
            logger.SetCallback(Log);
            applicationLogic.SetDependencies(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Update();
            applicationLogic.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void AddDevice(IDevice device)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Device>(AddDevice), new object[] { device });
                return;
            }
            if (IsDisposed) return;

            var deviceControl = new DeviceControl(device);
            deviceControl.SetDeviceName(device.GetFriendlyName());
            deviceControl.SetClickCallBack(device.OnClickDeviceButton);
            pnlDevices.Controls.Add(deviceControl);
            device.SetDeviceControl(deviceControl);
        }

        public void ShowLagControl(bool showLag)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(ShowLagControl), new object[] { showLag });
                return;
            }
            if (IsDisposed) return;

            if (!showLag)
            {
                grpDevices.Height += grpLag.Height;
                pnlDevices.Height += grpLag.Height;
            }
            grpLag.Visible = showLag;
        }

        public void ShowLog(bool boolShowLog)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(ShowLog), new object[] { boolShowLog });
                return;
            }
            if (IsDisposed) return;

            if (!boolShowLog)
                tabControl.TabPages.RemoveAt(1);
        }

        public void SetLagValue(int lagValue)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(SetLagValue), new object[] { lagValue });
                return;
            }
            if (IsDisposed) return;

            trbLag.Value = lagValue;
        }

        public void SetKeyboardHooks(bool useShortCuts)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(SetKeyboardHooks), new object[] { useShortCuts });
                return;
            }
            if (IsDisposed) return;

            chkHook.Checked = useShortCuts;
        }

        public void ToggleVisibility()
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
                Activate();
                WindowState = FormWindowState.Normal;
            }
        }

        public void Log(string message)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(Log), new object[] { message });
                    return;
                }
                if (IsDisposed) return;

                if (message.Contains("\"type\":\"PONG\"") || message.Contains("\"type\":\"PING\""))
                {
                    labelPingPong.Text = "Latest keep-alive message:" + DateTime.Now.ToLongTimeString();
                    labelPingPong.Update();
                }
                else
                {
                    textLog.AppendText(message + "\r\n\r\n");
                }
            }
            catch (Exception)
            {
            }
        }

        private void trbLag_Scroll(object sender, EventArgs e)
        {
            applicationLogic.SetLagThreshold(trbLag.Value);
        }

        private void chkHook_CheckedChanged(object sender, EventArgs e)
        {
            applicationLogic.OnSetHooks(chkHook.Checked);

        }

        private void btnVolumeUp_Click(object sender, EventArgs e)
        {
            devices.VolumeUp();
        }

        private void btnVolumeDown_Click(object sender, EventArgs e)
        {
            devices.VolumeDown();
        }

        private void btnVolumeMute_Click(object sender, EventArgs e)
        {
            devices.VolumeMute();
        }
    }
}
