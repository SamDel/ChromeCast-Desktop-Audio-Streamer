using System;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.UserControls;

namespace ChromeCast.Desktop.AudioStreamer
{
    public partial class MainForm : Form
    {
        private ApplicationLogic application;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            application = new ApplicationLogic(this);
            application.Start();

            Update();
            ReadConfiguration();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void DisposeForm()
        {
            Dispose();
        }

        public void AddDevice(Device device)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Device>(AddDevice), new object[] { device });
                return;
            }
            if (IsDisposed) return;

            var button = new DeviceControl(device);
            button.SetDeviceName(device.SsdpDevice.FriendlyName);
            button.SetClickCallBack(device.OnClickDeviceButton);
            pnlDevices.Controls.Add(button);
            device.DeviceControl = button;
        }

        private void ReadConfiguration()
        {
            try
            {
                string useKeyboardShortCuts = System.Configuration.ConfigurationManager.AppSettings["UseKeyboardShortCuts"];
                string showLog = System.Configuration.ConfigurationManager.AppSettings["ShowLog"];
                string showLagControl = System.Configuration.ConfigurationManager.AppSettings["ShowLagControl"];
                string lagControlValue = System.Configuration.ConfigurationManager.AppSettings["LagControlValue"];
                string autoStartDevices = System.Configuration.ConfigurationManager.AppSettings["AutoStartDevices"];
                string ipAddressesDevices = System.Configuration.ConfigurationManager.AppSettings["IpAddressesDevices"];

                bool useShortCuts;
                if (bool.TryParse(useKeyboardShortCuts, out useShortCuts))
                    chkHook.Checked = useShortCuts;

                bool boolShowLog;
                if (bool.TryParse(showLog, out boolShowLog))
                    if (!boolShowLog)
                        tabControl.TabPages.RemoveAt(1);

                bool showLag;
                if (bool.TryParse(showLagControl, out showLag))
                {
                    if (!showLag)
                    {
                        grpDevices.Height += grpLag.Height;
                        pnlDevices.Height += grpLag.Height;
                    }
                    grpLag.Visible = showLag;
                }

                int lagValue;
                if (int.TryParse(lagControlValue, out lagValue))
                    trbLag.Value = lagValue;

                bool autoStart;
                if (bool.TryParse(autoStartDevices, out autoStart))
                    application.AutoStart = autoStart;

                if (!string.IsNullOrWhiteSpace(ipAddressesDevices))
                {
                    var ipDevices = ipAddressesDevices.Split(';');
                    foreach (var ipDevice in ipDevices)
                    {
                        var arrDevice = ipDevice.Split(',');
                        application.OnDeviceAvailable(
                                new Rssdp.DiscoveredSsdpDevice { DescriptionLocation = new Uri(string.Format("http://{0}", arrDevice[0])) },
                                new Rssdp.SsdpRootDevice { FriendlyName = arrDevice[1] }
                            );
                    }
                }
            }
            catch (Exception)
            {
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
            application.SetLagThreshold(trbLag.Value);
        }

        private void chkHook_CheckedChanged(object sender, EventArgs e)
        {
            application.OnSetHooks(chkHook.Checked);

        }

        private void btnVolumeUp_Click(object sender, EventArgs e)
        {
            application.VolumeUp();
        }

        private void btnVolumeDown_Click(object sender, EventArgs e)
        {
            application.VolumeDown();
        }

        private void btnVolumeMute_Click(object sender, EventArgs e)
        {
            application.VolumeMute();
        }
    }
}
