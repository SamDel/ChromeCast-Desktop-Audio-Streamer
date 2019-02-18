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
using System.Globalization;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ChromeCast.Desktop.AudioStreamer
{
    public partial class MainForm : Form, IMainForm
    {
        private IApplicationLogic applicationLogic;
        private IDevices devices;
        private ILogger logger;
        private IPAddress previousIpAddress;

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
            applicationLogic.Initialize();
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
            cmbIP4AddressUsed.SelectedIndexChanged += CmbIP4AddressUsed_SelectedIndexChanged;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            FillStreamFormats();
            lblVersion.Text = $"{Properties.Strings.Version} {fvi.FileVersion}";
            Task.Run(() =>
            {
                CheckForNewVersion(fvi.FileVersion);
            });
        }

        private void ApplyLocalization()
        {
            Text = Properties.Strings.MainForm_Text;
            grpVolume.Text = Properties.Strings.Group_VolumeAllDevices_Text;
            btnVolumeUp.Text = Properties.Strings.Button_Up_Text;
            btnVolumeDown.Text = Properties.Strings.Button_Down_Text;
            btnVolumeMute.Text = Properties.Strings.Button_Mute_Text;
            grpDevices.Text = Properties.Strings.Group_Devices_Text;
            btnScan.Text = Properties.Strings.Button_ScanAgain_Text;
            grpLag.Text = Properties.Strings.Group_Lag_Text;
            lblLagMin.Text = Properties.Strings.Label_MinimumLag_Text;
            lblLagMax.Text = Properties.Strings.Label_MaximumLag_Text;
            lblLagExperimental.Text = Properties.Strings.Label_LagExperimental_Text;
            tabPageMain.Text = Properties.Strings.Tab_Main_Text;
            tabPageOptions.Text = Properties.Strings.Tab_Options_Text;
            grpOptions.Text = Properties.Strings.Group_Options_Text;
            lblIpAddressUsed.Text = Properties.Strings.Label_IPAddressUsed_Text;
            lblDevice.Text = Properties.Strings.Label_RecordingDevice_Text;
            lblStreamFormat.Text = Properties.Strings.Label_StreamFormat_Text;
            chkHook.Text = Properties.Strings.Check_KeyboardShortcuts_Text;
            chkShowWindowOnStart.Text = Properties.Strings.Check_ShowWindowOnStart_Text;
            chkAutoStart.Text = Properties.Strings.Check_AutomaticallyStart_Text;
            chkAutoRestart.Text = Properties.Strings.Check_AutomaticallyRestart_Text;
            chkShowLagControl.Text = Properties.Strings.Check_ShowLagControl_Text;
            chkStartApplicationWhenWindowsStarts.Text = Properties.Strings.Check_StartApplicationWhenWindowsStarts_Text;
            btnResetSettings.Text = Properties.Strings.Button_ResetSetting_Text;
            tabPageLog.Text = Properties.Strings.Tab_Log_Text;
            btnClipboardCopy.Text = Properties.Strings.Button_ClipboardCopy_Text;
            lblLanguage.Text = Properties.Strings.Label_Language_Text;
            btnClearLog.Text = Properties.Strings.Button_ClearLog_Text;
            chkLogDeviceCommunication.Text = Properties.Strings.Check_LogDeviceCommunication_Text;

            if (cmbLanguage.Items.Count == 0)
            {
                cmbLanguage.Items.Add(Resource.Get("Language", CultureInfo.GetCultureInfo("en")));
                cmbLanguage.Items.Add(Resource.Get("Language", CultureInfo.GetCultureInfo("fr")));
            }
            else
            {
                if (cmbLanguage.Items[0].ToString() != Resource.Get("Language", CultureInfo.GetCultureInfo("en")))
                    cmbLanguage.Items[0] = Resource.Get("Language", CultureInfo.GetCultureInfo("en"));
                if (cmbLanguage.Items[1].ToString() != Resource.Get("Language", CultureInfo.GetCultureInfo("fr")))
                    cmbLanguage.Items[1] = Resource.Get("Language", CultureInfo.GetCultureInfo("fr"));
            }
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
            {
                if (cmbLanguage.SelectedIndex != 1)
                    cmbLanguage.SelectedIndex = 1;
            }
            else
            {
                if (cmbLanguage.SelectedIndex != 0)
                    cmbLanguage.SelectedIndex = 0;
            }
        }

        private void FillStreamFormats()
        {
            if (cmbStreamFormat == null)
                return;

            if (cmbStreamFormat.Items.Count == 0)
            {
                cmbStreamFormat.Items.Add(new ComboboxItem(SupportedStreamFormat.Wav));
                cmbStreamFormat.Items.Add(new ComboboxItem(SupportedStreamFormat.Mp3_128));
                cmbStreamFormat.Items.Add(new ComboboxItem(SupportedStreamFormat.Mp3_320));
                cmbStreamFormat.SelectedIndex = 2;
                SetStreamFormat();
            }
        }

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            AddIP4Addresses();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.CloseApplication();
        }

        public void AddDevice(IDevice device)
        {
            if (device == null || pnlDevices == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<Device>(AddDevice), new object[] { device });
                return;
            }
            if (IsDisposed) return;

            var deviceControl = new DeviceControl(device);
            deviceControl.SetDeviceName(device.GetFriendlyName());
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
            if (pnlDevices == null)
                return;

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

        public void SetLagValue(int lagValue)
        {
            if (trbLag == null)
                return;

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
            if (chkHook == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<bool>(SetKeyboardHooks), new object[] { useShortCuts });
                return;
            }
            if (IsDisposed) return;

            chkHook.Checked = useShortCuts;
        }

        public void ToggleFormVisibility(object sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MouseEventArgs)))
            {
                if (((MouseEventArgs)e).Button != MouseButtons.Left) return;
            }

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
            if (txtLog == null)
                return;

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
                    lblPingPong.Text = $"{Properties.Strings.Label_KeepAlive_Text} {DateTime.Now.ToLongTimeString()}";
                    lblPingPong.Update();
                }
                else
                {
                    if (chkLogDeviceCommunication.Checked)
                        txtLog.AppendText(message + "\r\n\r\n");
                }
            }
            catch (Exception)
            {
            }
        }

        private void TrbLag_Scroll(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.SetLagThreshold(trbLag.Value);
        }

        private void ChkHook_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHook == null)
                return;

            if (chkHook.Checked)
                NativeMethods.StartSetWindowsHooks(devices);
            else
                NativeMethods.StopSetWindowsHooks();
        }

        private void BtnVolumeUp_Click(object sender, EventArgs e)
        {
            if (devices == null)
                return;

            devices.VolumeUp();
        }

        private void BtnVolumeDown_Click(object sender, EventArgs e)
        {
            if (devices == null)
                return;

            devices.VolumeDown();
        }

        private void BtnVolumeMute_Click(object sender, EventArgs e)
        {
            if (devices == null)
                return;

            devices.VolumeMute();
        }

        public async void SetWindowVisibility(bool visible)
        {
            if (chkShowWindowOnStart == null)
                return;

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

        public void AddRecordingDevices(MMDeviceCollection devices, MMDevice defaultdevice)
        {
            if (devices == null || cmbRecordingDevice == null)
                return;

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
            cmbRecordingDevice.SelectedIndexChanged += CmbRecordingDevice_SelectedIndexChanged;
        }

        public void GetRecordingDevice(Func<MMDevice, bool> startRecordingSetDevice)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Func<MMDevice, bool>>(GetRecordingDevice), new object[] { startRecordingSetDevice });
                return;
            }
            if (IsDisposed) return;

            SetDevice(startRecordingSetDevice);
        }

        private void SetDevice(Func<MMDevice, bool> startRecordingSetDevice)
        {
            if (cmbRecordingDevice == null)
                return;

            if (cmbRecordingDevice.Items.Count > 0)
            {
                if (!startRecordingSetDevice((MMDevice)cmbRecordingDevice.SelectedItem))
                {
                    // Start the first device that has no error, wait for ~ 1 minute till the devices are up and running.
                    for (int attempt = 0; attempt < 6; attempt++)
                    {
                        for (int i = 0; i < cmbRecordingDevice.Items.Count; i++)
                        {
                            if (startRecordingSetDevice((MMDevice)cmbRecordingDevice.Items[i]))
                            {
                                cmbRecordingDevice.SelectedIndex = i;
                                return;
                            }
                        }
                        Task.Delay(10000).Wait();
                    }

                }
            }

            startRecordingSetDevice(null);
        }

        private void CmbRecordingDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.RecordingDeviceChanged();
        }

        private void ChkAutoRestart_CheckedChanged(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.OnSetAutoRestart(chkAutoRestart.Checked);
        }

        public void SetAutoRestart(bool autoRestart)
        {
            if (chkAutoRestart == null)
                return;

            chkAutoRestart.Checked = autoRestart;
        }

        public void AddIP4Addresses()
        {
            if (cmbIP4AddressUsed == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action(AddIP4Addresses));
                return;
            }

            if (cmbIP4AddressUsed == null)
                return;

            var oldAddressUsed = (IPAddress)cmbIP4AddressUsed.SelectedItem;
            var ip4Adresses = Network.GetIp4ddresses();

            logger?.Log($"Add IP4 addresses: {string.Join(" - ", ip4Adresses.Select(x => x.IPAddress))}");
            cmbIP4AddressUsed.Items.Clear();
            if (ip4Adresses.Count > 0)
            {
                foreach (var adapter in ip4Adresses)
                {
                    cmbIP4AddressUsed.Items.Add(adapter.IPAddress);
                }

                if (ip4Adresses.Any(x => x.IPAddress?.ToString() == oldAddressUsed?.ToString()))
                {
                    cmbIP4AddressUsed.SelectedItem = oldAddressUsed;
                    previousIpAddress = oldAddressUsed;
                }
                else
                {
                    var addressUsed = Network.GetIp4Address();
                    if (addressUsed != null)
                    {
                        cmbIP4AddressUsed.SelectedItem = addressUsed;
                        previousIpAddress = addressUsed;
                    }
                }
            }
        }

        private void CmbIP4AddressUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbIP4AddressUsed == null)
                return;

            var ipAddress = (IPAddress)cmbIP4AddressUsed.SelectedItem;
            if (ipAddress?.ToString() != previousIpAddress?.ToString())
                applicationLogic.ChangeIPAddressUsed(ipAddress);
            previousIpAddress = ipAddress;
        }

        private void BtnClipboardCopy_Click(object sender, EventArgs e)
        {
            if (txtLog == null)
                return;

            if (!string.IsNullOrEmpty(txtLog.Text))
                Clipboard.SetText(txtLog.Text);
        }

        private void BtnScan_Click(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.ScanForDevices();
        }

        public void SetAutoStart(bool autoStart)
        {
            if (chkAutoStart == null)
                return;

            chkAutoStart.Checked = autoStart;
        }

        public bool GetUseKeyboardShortCuts()
        {
            if (chkHook == null)
                return false;

            return chkHook.Checked;
        }

        public bool GetAutoStartDevices()
        {
            if (chkAutoStart == null)
                return false;

            return chkAutoStart.Checked;
        }

        public bool GetShowWindowOnStart()
        {
            if (chkShowWindowOnStart == null)
                return false;

            return chkShowWindowOnStart.Checked;
        }

        public bool GetAutoRestart()
        {
            if (chkAutoRestart == null)
                return false;

            return chkAutoRestart.Checked;
        }

        private void BtnResetSettings_Click(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.ResetSettings();
        }

        public void DoDragDrop(object sender, DragEventArgs e)
        {
            if (e == null || pnlDevices == null)
                return;

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

        private void ChkShowLagControl_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowLagControl == null)
                return;

            ShowLagControl(chkShowLagControl.Checked);
        }

        public bool? GetShowLagControl()
        {
            if (chkShowLagControl == null)
                return false;

            return chkShowLagControl.Checked;
        }

        public int? GetLagValue()
        {
            if (trbLag == null)
                return 1000;

            return trbLag.Value;
        }

        public void SetStreamFormat(SupportedStreamFormat format)
        {
            if (cmbStreamFormat == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<SupportedStreamFormat>(SetStreamFormat), new object[] { format });
                return;
            }
            if (IsDisposed) return;

            FillStreamFormats();
            for (int i = 0; i < cmbStreamFormat.Items.Count; i++)
            {
                if ((SupportedStreamFormat)((ComboboxItem)cmbStreamFormat.Items[i]).Value == format)
                    cmbStreamFormat.SelectedIndex = i;
            }
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

        private void CmbStreamFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStreamFormat();
        }

        private void SetStreamFormat()
        {
            if (cmbStreamFormat == null || applicationLogic == null)
                return;

            if (cmbStreamFormat.SelectedItem != null)
                applicationLogic.SetStreamFormat((SupportedStreamFormat)((ComboboxItem)cmbStreamFormat.SelectedItem).Value);
        }

        private void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLanguage == null)
                return;

            if (cmbLanguage.SelectedItem.ToString() == Resource.Get("Language", CultureInfo.GetCultureInfo("en")))
                SetCulture("en");
            else if (cmbLanguage.SelectedItem.ToString() == Resource.Get("Language", CultureInfo.GetCultureInfo("fr")))
                SetCulture("fr");
        }

        public void SetCulture(string culture)
        {
            if (applicationLogic == null)
                return;

            CultureInfo ci = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            ApplyLocalization();
            applicationLogic.SetCulture(culture);
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            if (txtLog == null)
                return;

            txtLog.Clear();
        }

        public void SetLogDeviceCommunication(bool logDeviceCommunication)
        {
            if (chkLogDeviceCommunication == null || tabControl == null)
                return;

            chkLogDeviceCommunication.Checked = logDeviceCommunication;
            if (logDeviceCommunication)
            {
                if (!tabControl.TabPages.Contains(tabPageLog))
                    tabControl.TabPages.Add(tabPageLog);
            }
            else
            {
                if (tabControl.TabPages.Contains(tabPageLog))
                    tabControl.TabPages.Remove(tabPageLog);
            }
        }

        public bool GetLogDeviceCommunication()
        {
            if (chkLogDeviceCommunication == null)
                return false;

            return chkLogDeviceCommunication.Checked;
        }

        public void SetStartApplicationWhenWindowsStarts(bool value)
        {
            chkStartApplicationWhenWindowsStarts.Checked = value;
        }

        public bool GetStartApplicationWhenWindowsStarts()
        {
            return chkStartApplicationWhenWindowsStarts.Checked;
        }

        private void ChkLogDeviceCommunication_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogDeviceCommunication == null)
                return;

            SetLogDeviceCommunication(chkLogDeviceCommunication.Checked);
        }

        private void CheckForNewVersion(string currentVersion)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = HttpWebRequest.Create("https://api.github.com/repos/SamDel/ChromeCast-Desktop-Audio-Streamer/releases/latest");
                ((HttpWebRequest)request).KeepAlive = false;
                ((HttpWebRequest)request).UserAgent = "SamDel/ChromeCast-Desktop-Audio-Streamer";
                var response = request.GetResponse();
                var dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);
                var responseFromServer = reader.ReadToEnd();
                var json = JsonConvert.DeserializeObject(responseFromServer);
                var latestRelease = ((JObject)json)["tag_name"].ToString().Replace("v", "");
                if (latestRelease.CompareTo(currentVersion) > 0)
                {
                    var latestReleaseUrl = ((JObject)json)["html_url"].ToString();
                    ShowLatestRelease(latestRelease, latestReleaseUrl);
                }

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                logger.Log($"CheckForNewVersion: {ex.Message}");
            }
        }

        private void ShowLatestRelease(string latestRelease, string latestReleaseUrl)
        {
            if (lblNewReleaseAvailable == null)
                return;

            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke(new Action<string, string>(ShowLatestRelease), new object[] { latestRelease, latestReleaseUrl });
                return;
            }

            lblNewReleaseAvailable.Text = $"{Properties.Strings.Label_NewVersionAvailable} ({latestRelease})";
            var link = new LinkLabel.Link
            {
                LinkData = latestReleaseUrl
            };
            lblNewReleaseAvailable.Links.Add(link);
            lblNewReleaseAvailable.Visible = true;
        }

        private void LblNewReleaseAvailable_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e == null)
                return;

            Process.Start(e.Link.LinkData as string);
        }

        private void ChkStartApplicationWhenWindowsStarts_CheckedChanged(object sender, EventArgs e)
        {
            WindowsStartup.StartApplicationWhenWindowsStarts(chkStartApplicationWhenWindowsStarts.Checked);
        }
    }
}
