using System;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using ChromeCast.Desktop.AudioStreamer.Classes;
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

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

            ApplyLocalization();
            applicationLogic = applicationLogicIn;
            devices = devicesIn;
            logger = loggerIn;
            logger.SetCallback(Log);
            devices.SetDependencies(this, applicationLogic);
            applicationLogic.SetDependencies(this);
        }

        public MainForm()
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Update();
            AddIP4Addresses();
            applicationLogic.Start();
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            lblVersion.Text = $"{Properties.Strings.Version} {fvi.FileVersion}";
            FillStreamFormats();
        }

        private void ApplyLocalization()
        {
            Text = Properties.Strings.MainForm_Text;
            grpVolume.Text = Properties.Strings.Group_VolumeAllDevices_Text;
            btnVolumeUp.Text = Properties.Strings.Button_Up_Text;
            btnVolumeDown.Text = Properties.Strings.Button_Down_Text;
            btnVolumeMute.Text = Properties.Strings.Button_Mute_Text;
            btnSyncDevices.Text = Properties.Strings.Button_SyncDevices_Text;
            grpDevices.Text = Properties.Strings.Group_Devices_Text;
            btnScan.Text = Properties.Strings.Button_ScanAgain_Text;
            grpLag.Text = Properties.Strings.Group_Lag_Text;
            lblLagMin.Text = Properties.Strings.Label_MinimumLag_Text;
            lblLagMax.Text = Properties.Strings.Label_MaximumLag_Text;
            lblLagExperimental.Text = Properties.Strings.Label_LagExperimental_Text;
            tabPageOptions.Text = Properties.Strings.Tab_Options_Text;
            grpOptions.Text = Properties.Strings.Group_Options_Text;
            lblIpAddressUsed.Text = Properties.Strings.Label_IPAddressUsed_Text;
            lblDevice.Text = Properties.Strings.Label_RecordingDevice_Text;
            lblStreamFormat.Text = Properties.Strings.Label_StreamFormat_Text;
            lblStreamFormatExtra.Text = Properties.Strings.Label_StreamFormatExtra_Text;
            chkHook.Text = Properties.Strings.Check_KeyboardShortcuts_Text;
            chkShowWindowOnStart.Text = Properties.Strings.Check_ShowWindowOnStart_Text;
            chkAutoStart.Text = Properties.Strings.Check_AutomaticallyStart_Text;
            chkAutoRestart.Text = Properties.Strings.Check_AutomaticallyRestart_Text;
            chkShowLagControl.Text = Properties.Strings.Check_ShowLagControl_Text;
            btnResetSettings.Text = Properties.Strings.Button_ResetSetting_Text;
            tabPageLog.Text = Properties.Strings.Tab_Log_Text;
            btnClipboardCopy.Text = Properties.Strings.Button_ClipboardCopy_Text;
        }

        private void FillStreamFormats()
        {
            if (cmbStreamFormat.Items.Count == 0)
            {
                cmbStreamFormat.Items.Add(SupportedStreamFormat.Wav);
                cmbStreamFormat.Items.Add(SupportedStreamFormat.Mp3_128);
                cmbStreamFormat.Items.Add(SupportedStreamFormat.Mp3_320);
                cmbStreamFormat.SelectedItem = SupportedStreamFormat.Wav;
                SetStreamFormat();
            }
        }

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            AddIP4Addresses();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            applicationLogic.CloseApplication();
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
            deviceControl.SetStatus(device.GetDeviceState(), null);
            pnlDevices.Controls.Add(deviceControl);
            device.SetDeviceControl(deviceControl);

            // Sort alphabetically.
            var deviceName = device.GetFriendlyName();
            for (int i = 0; i < pnlDevices.Controls.Count - 1; i++)
            {
                if (pnlDevices.Controls[i] is DeviceControl)
                {
                    var name = ((DeviceControl)pnlDevices.Controls[i]).GetDeviceName();
                    if (string.CompareOrdinal(deviceName, name) < 0)
                    {
                        pnlDevices.Controls.SetChildIndex(deviceControl, i);
                        return;
                    }
                }
            }
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
                grpDevices.Height = tabPageMain.Height - grpVolume.Height - 30;
            }
            else
            {
                grpDevices.Height = tabPageMain.Height - grpVolume.Height - grpLag.Height - 30;
            }
            pnlDevices.Height = grpDevices.Height - 30;
            btnScan.Top = grpDevices.Height - btnScan.Height - 10;
            grpLag.Visible = showLag;
            chkShowLagControl.Checked = showLag;
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
                tabControl.TabPages.RemoveAt(2);
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
                    labelPingPong.Text = Properties.Strings.Label_KeepAlive_Text + DateTime.Now.ToLongTimeString();
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

        public async void SetWindowVisibility(bool visible)
        {
            chkShowWindowOnStart.Checked = visible;
            if (visible)
                Show();
            else
            {
                await HideWindow();
            }
        }

        private async Task HideWindow()
        {
            await Task.Delay(1000);
            Hide();
        }

        public bool DoSyncDevices()
        {
            return devices.Count() > 1;
        }

        private void btnSyncDevices_Click(object sender, EventArgs e)
        {
            devices.Sync();
        }

        public void AddRecordingDevices(MMDeviceCollection devices, MMDevice defaultdevice)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<MMDeviceCollection, MMDevice>(AddRecordingDevices), new object[] { devices, defaultdevice });
                return;
            }
            if (IsDisposed) return;

            foreach (var device in devices)
            {
                if (!cmbRecordingDevice.Items.Contains(device))
                {
                    var index = cmbRecordingDevice.Items.Add(device);
                    if (device.DeviceID == defaultdevice.DeviceID)
                        cmbRecordingDevice.SelectedIndex = index;
                }
            }
        }

        public void GetRecordingDevice(Action<MMDevice> startRecordingSetDevice)
        {
            if (InvokeRequired)
            {
                try
                {
                    SetDevice(startRecordingSetDevice);
                    return;
                }
                catch (Exception)
                {
                }
                Invoke(new Action<Action<MMDevice>>(GetRecordingDevice), new object[] { startRecordingSetDevice });
                return;
            }
            if (IsDisposed) return;

            SetDevice(startRecordingSetDevice);
        }

        private void SetDevice(Action<MMDevice> startRecordingSetDevice)
        {
            if (cmbRecordingDevice.Items.Count > 0)
                startRecordingSetDevice((MMDevice)cmbRecordingDevice.SelectedItem);
            else
                startRecordingSetDevice(null);
        }

        private void cmbRecordingDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            applicationLogic.RecordingDeviceChanged();
        }

        private void chkAutoRestart_CheckedChanged(object sender, EventArgs e)
        {
            applicationLogic.OnSetAutoRestart(chkAutoRestart.Checked);
        }

        public void SetAutoRestart(bool autoRestart)
        {
            chkAutoRestart.Checked = autoRestart;
        }

        public void AddIP4Addresses()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(AddIP4Addresses));
                return;
            }

            var oldAddressUsed = (IPAddress)cmbIP4AddressUsed.SelectedItem;
            var ip4Adresses = Network.GetIp4ddresses();
            foreach (var adapter in ip4Adresses)
            {
                if (cmbIP4AddressUsed.Items.IndexOf(adapter.IPAddress) < 0)
                    cmbIP4AddressUsed.Items.Add(adapter.IPAddress);
            }
            for (int i = cmbIP4AddressUsed.Items.Count - 1; i >= 0; i--)
            {
                if (!ip4Adresses.Any(x => x.IPAddress.ToString() == ((IPAddress)cmbIP4AddressUsed.Items[i]).ToString()))
                    cmbIP4AddressUsed.Items.RemoveAt(i);
            }

            if (!ip4Adresses.Any(x => x.IPAddress.ToString() == oldAddressUsed?.ToString()))
            {
                var addressUsed = Network.GetIp4Address();
                cmbIP4AddressUsed.SelectedItem = addressUsed;
            }
        }

        private void cmbIP4AddressUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ipAddress = (IPAddress)cmbIP4AddressUsed.SelectedItem;
            applicationLogic.ChangeIPAddressUsed(ipAddress);
        }

        private void btnClipboardCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textLog.Text);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            applicationLogic.ScanForDevices();
        }

        public void SetAutoStart(bool autoStart)
        {
            chkAutoStart.Checked = autoStart;
        }

        public bool GetUseKeyboardShortCuts()
        {
            return chkHook.Checked;
        }

        public bool GetAutoStartDevices()
        {
            return chkAutoStart.Checked;
        }

        public bool GetShowWindowOnStart()
        {
            return chkShowWindowOnStart.Checked;
        }

        public bool GetAutoRestart()
        {
            return chkAutoRestart.Checked;
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            applicationLogic.ResetSettings();
        }

        public void DoDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Length >= 1 &&
                e.Data.GetData(e.Data.GetFormats()[0]) is DeviceControl &&
                sender is DeviceControl)
            {
                var draggingControl = (DeviceControl)e.Data.GetData(e.Data.GetFormats()[0]);
                var droppingOnControl = (DeviceControl)sender;
                var indexDrop = pnlDevices.Controls.GetChildIndex(droppingOnControl);

                pnlDevices.Controls.SetChildIndex(draggingControl, indexDrop);
            }
        }

        private void chkShowLagControl_CheckedChanged(object sender, EventArgs e)
        {
            ShowLagControl(chkShowLagControl.Checked);
        }

        public bool? GetShowLagControl()
        {
            return chkShowLagControl.Checked;
        }

        public int? GetLagValue()
        {
            return trbLag.Value;
        }

        public void SetStreamFormat(SupportedStreamFormat format)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SupportedStreamFormat>(SetStreamFormat), new object[] { format });
                return;
            }
            if (IsDisposed) return;

            FillStreamFormats();
            cmbStreamFormat.SelectedItem = format;
            SetStreamFormat();
        }

        public void GetStreamFormat()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(GetStreamFormat));
                return;
            }
            if (IsDisposed) return;

            SetStreamFormat();
        }

        private void cmbStreamFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStreamFormat();
        }

        private void SetStreamFormat()
        {
            if (cmbStreamFormat.SelectedItem != null)
                applicationLogic.SetStreamFormat((SupportedStreamFormat)cmbStreamFormat.SelectedItem);
        }
    }
}
