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
using System.Drawing;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using System.Text;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using System.Net.Sockets;

namespace ChromeCast.Desktop.AudioStreamer
{
    public partial class MainForm : Form, IMainForm
    {
        private readonly IApplicationLogic applicationLogic;
        private readonly IDevices devices;
        private readonly ILogger logger;
        private IPAddress previousIpAddress;
        private readonly ILoopbackRecorder loopbackRecorder;
        private Size windowSize;
        private readonly StringBuilder log = new StringBuilder();
        private string previousRecordingDeviceID;
        private bool eventHandlerAdded;
        private bool isRecordingDeviceSelected;
        private readonly WavGenerator wavGenerator;

        public MainForm(IApplicationLogic applicationLogicIn, IDevices devicesIn, ILoopbackRecorder loopbackRecorderIn, ILogger loggerIn)
        {
            InitializeComponent();

            ApplyLocalization();
            loopbackRecorder = loopbackRecorderIn;
            applicationLogic = applicationLogicIn;
            devices = devicesIn;
            logger = loggerIn;
            logger.SetCallback(Log);
            devices.SetDependencies(this, applicationLogic);
            applicationLogic.SetDependencies(this);
            wavGenerator = new WavGenerator();
        }

        public MainForm()
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            Update();
            AddIP4Addresses();
            applicationLogic.Initialize();
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
            cmbIP4AddressUsed.SelectedIndexChanged += CmbIP4AddressUsed_SelectedIndexChanged;
            cmbBufferInSeconds.SelectedIndexChanged += CmbBufferInSeconds_SelectedIndexChanged;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            FillStreamFormats();
            FillFilterDevices();
            lblVersion.Text = $"{Properties.Strings.Version} {fvi.FileVersion}";
            applicationLogic.StartTask(() =>
            {
                CheckForNewVersion(fvi.FileVersion);
            });
            loopbackRecorder.Start(this, applicationLogic.OnRecordingDataAvailable, applicationLogic.ClearMp3Buffer);
        }

        public IntPtr GetHandle()
        {
            return this.Handle;
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
            chkAutoStartLastUsed.Text = Properties.Strings.Check_AutonaticallyStartLastUsed_Text;
            chkAutoRestart.Text = Properties.Strings.Check_AutomaticallyRestart_Text;
            chkShowLagControl.Text = Properties.Strings.Check_ShowLagControl_Text;
            chkStartApplicationWhenWindowsStarts.Text = Properties.Strings.Check_StartApplicationWhenWindowsStarts_Text;
            chkMinimizeToTray.Text = Properties.Strings.Check_MinimizeToTray_Text;
            btnResetSettings.Text = Properties.Strings.Button_ResetSetting_Text;
            tabPageLog.Text = Properties.Strings.Tab_Log_Text;
            btnClipboardCopy.Text = Properties.Strings.Button_ClipboardCopy_Text;
            lblLanguage.Text = Properties.Strings.Label_Language_Text;
            btnClearLog.Text = Properties.Strings.Button_ClearLog_Text;
            chkLogDeviceCommunication.Text = Properties.Strings.Check_LogDeviceCommunication_Text;
            chkAutoMute.Text = Properties.Strings.Check_AutoMute_Text;
            linkHelp.Text = Properties.Strings.Label_LinkHelp_Text;
            volumeMeterTooltip.SetToolTip(pnlVolumeMeter, Properties.Strings.Tooltip_RecordingLevel_Text);
            volumeMeterTooltip.SetToolTip(lblDb, Properties.Strings.Tooltip_RecordingLevel_Text);
            volumeMeterTooltip.SetToolTip(volumeMeter, Properties.Strings.Tooltip_RecordingLevel_Text);
            lblFilterDevices.Text = Properties.Strings.Label_FilterDevices_Text;
            lblBufferInSeconds.Text = Properties.Strings.Label_BufferInSeconds_Text;

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

            if (GetMinimizeToTray())
            {
                applicationLogic.SaveSettings();
                Hide();
                e.Cancel = true;
            }
            else
            {
                applicationLogic.CloseApplication();
            }
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
            var filter = GetFilterDevices();
            if (filter != null)
                deviceControl.Visible = FilterDevices.ShowFilterDevices(device, filter.Value);

            // Sort alphabetically.
            var deviceName = device.GetFriendlyName();
            for (int i = 0; i < pnlDevices.Controls.Count - 1; i++)
            {
                if (pnlDevices.Controls[i] is DeviceControl recordingDevice)
                {
                    var name = recordingDevice.GetDeviceName();
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
            if (txtLog == null || log == null)
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
                    {
                        message += "\r\n\r\n";
                        txtLog.AppendText(message);
                        log.Append(message);

                        if (txtLog.Text.Length > 2000000)
                            txtLog.Clear();

                        if (log.Length > 100000000)
                            log.Clear();
                    }
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
                var exists = false;
                for (int i = 0; i < cmbRecordingDevice.Items.Count; i++)
                {
                    if (((MMDevice)cmbRecordingDevice.Items[i]).DeviceID == device.DeviceID)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    var index = cmbRecordingDevice.Items.Add(device);
                }
            }

            // Select the right device.
            if (!isRecordingDeviceSelected)
            {
                for (int i = 0; i < cmbRecordingDevice.Items.Count; i++)
                {
                    var device = (MMDevice)cmbRecordingDevice.Items[i];
                    if (previousRecordingDeviceID == null && device.DeviceID == defaultdevice.DeviceID)
                    {
                        // Nothing previously selected, select the default device.
                        if (cmbRecordingDevice.SelectedIndex != i)
                        {
                            cmbRecordingDevice.SelectedIndex = i;
                            PlaySilence();
                            isRecordingDeviceSelected = true;
                        }
                    }
                    else if (!string.IsNullOrEmpty(previousRecordingDeviceID) && device.DeviceID == previousRecordingDeviceID)
                    {
                        // Select the previously selected device (only once).
                        cmbRecordingDevice.SelectedIndex = i;
                        PlaySilence();
                        previousRecordingDeviceID = string.Empty;
                        isRecordingDeviceSelected = true;
                    }
                }
            }

            if (!eventHandlerAdded)
            {
                cmbRecordingDevice.SelectedIndexChanged += CmbRecordingDevice_SelectedIndexChanged;
                eventHandlerAdded = true;
            }
        }

        public void GetRecordingDevice()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(GetRecordingDevice));
                return;
            }
            if (IsDisposed) return;

            if (cmbRecordingDevice == null)
                return;

            if (!StartRecordingDevice())
                loopbackRecorder.StartRecordingSetDevice(null);
        }

        private bool StartRecordingDevice()
        {
            if (cmbRecordingDevice == null || cmbRecordingDevice.Items.Count == 0)
                return false;

            if (!loopbackRecorder.StartRecordingSetDevice((MMDevice)cmbRecordingDevice.SelectedItem))
            {
                // Start the first device that has no error.
                for (int i = 0; i < cmbRecordingDevice.Items.Count; i++)
                {
                    if (loopbackRecorder.StartRecordingSetDevice((MMDevice)cmbRecordingDevice.Items[i]))
                    {
                        cmbRecordingDevice.SelectedIndex = i;
                        PlaySilence();
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private void CmbRecordingDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            isRecordingDeviceSelected = true;
            loopbackRecorder?.StopRecording();
            StartRecordingDevice();
            PlaySilence();
        }

        private void PlaySilence()
        {
            if (cmbRecordingDevice.SelectedItem != null)
            {
                wavGenerator.Stop();
                var device = (MMDevice)cmbRecordingDevice.SelectedItem;
                wavGenerator.PlaySilenceLoop(device.FriendlyName, device.DeviceFormat);
            }
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

            if (!InvokeRequired)
            {
                var oldAddressUsed = (IPAddress)cmbIP4AddressUsed.SelectedItem;
                //LogNetworkInformation();
                var ip4Adresses = Network.GetIp4ddresses();

                //logger?.Log($"Add IP4 addresses: {string.Join(" - ", ip4Adresses.Select(x => x.IPAddress))}");
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
        }

        private void LogNetworkInformation()
        {
            Log($"Network information:");
            Log($"");
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                Log($"Name networkInterface: {networkInterface.Name}");
                var properties = networkInterface.GetIPProperties();
                if (properties != null)
                {
                    foreach (var ip in properties.UnicastAddresses)
                    {
                        if (ip != null && ip.Address != null)
                        {
                            var address = ip.Address;
                            if (address.ToString().Length >= 5)
                                Log($"Address: {address.ToString().Substring(0, 5)}**************");
                            else
                                Log($"Address: {address.ToString()}");
                            Log($"Address family: {address.AddressFamily.ToString()}");
                            Log($"Operational status: {networkInterface.OperationalStatus.ToString()}");
                            Log($"NetworkInterface type: {networkInterface.NetworkInterfaceType.ToString()}");
                            Log($"GatewayAddresses count: {properties.GatewayAddresses.Count.ToString()}");
                            if (address.AddressFamily == AddressFamily.InterNetwork
                                && Network.IsInLocalIpRange(address)
                                && networkInterface.OperationalStatus != OperationalStatus.Down
                                && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback
                                && properties.GatewayAddresses.Count > 0)
                            {
                                var IsEthernet = networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT;
                                Log($"Is ethernet: {IsEthernet}");
                            }
                        }
                        Log($"");
                    }
                    Log($"_______________________________________________________________________");
                    Log($"");
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
            if (log == null)
                return;

            if (!string.IsNullOrEmpty(log.ToString()))
                Clipboard.SetText(log.ToString());
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
                e.Data.GetData(format: e.Data.GetFormats()[0]) is DeviceControl &&
                sender is DeviceControl deviceControl)
            {
                var draggingControl = (DeviceControl)e.Data.GetData(e.Data.GetFormats()[0]);
                var droppingOnControl = deviceControl;
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
            if (txtLog == null || log == null)
                return;

            txtLog.Clear();
            log.Clear();
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
                request.Timeout = 5000;
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

        private void FillFilterDevices()
        {
            if (cmbFilterDevices == null)
                return;

            if (cmbFilterDevices.Items.Count == 0)
            {
                cmbFilterDevices.Items.Add(new ComboboxItem(FilterDevicesEnum.ShowAll));
                cmbFilterDevices.Items.Add(new ComboboxItem(FilterDevicesEnum.DevicesOnly));
                cmbFilterDevices.Items.Add(new ComboboxItem(FilterDevicesEnum.GroupsOnly));
                cmbFilterDevices.SelectedIndex = 0;
            }
        }

        public void SetFilterDevices(FilterDevicesEnum value)
        {
            if (cmbFilterDevices == null || applicationLogic == null)
                return;

            FillFilterDevices();
            for (int i = 0; i < cmbFilterDevices.Items.Count; i++)
            {
                if ((FilterDevicesEnum)((ComboboxItem)cmbFilterDevices.Items[i]).Value == value)
                    cmbFilterDevices.SelectedIndex = i;
            }
            applicationLogic.SetFilterDevices(value);
        }

        public FilterDevicesEnum? GetFilterDevices()
        {
            if (cmbFilterDevices == null)
                return null;

            return (FilterDevicesEnum)((ComboboxItem)cmbFilterDevices.SelectedItem).Value;
        }

        private void CmbFilterDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (applicationLogic == null)
                return;

            applicationLogic.SetFilterDevices((FilterDevicesEnum)((ComboboxItem)cmbFilterDevices.SelectedItem).Value);
        }

        public void SetStartLastUsedDevices(bool value)
        {
            if (chkAutoStartLastUsed == null)
                return;

            chkAutoStartLastUsed.Checked = value;
        }

        public bool? GetStartLastUsedDevices()
        {
            if (chkAutoStartLastUsed == null)
                return false;

            return chkAutoStartLastUsed.Checked;
        }

        public void SetSize(Size size)
        {
            logger.Log($"Set size, height: {size.Height} width: {size.Width}");
            Height = size.Height;
            Width = size.Width;
        }

        public Size GetSize()
        {
            return windowSize;
        }

        public void SetPosition(int? left, int? top)
        {
            if (left.HasValue && top.HasValue)
            {
                Left = left.Value;
                Top = top.Value;
            }
            else
            {
                StartPosition = FormStartPosition.CenterScreen;
            }
        }

        public int GetLeft()
        {
            if (WindowState == FormWindowState.Normal)
                return Left;
            else
                return RestoreBounds.Left;
        }

        public int GetTop()
        {
            if (WindowState == FormWindowState.Normal)
                return Top;
            else
                return RestoreBounds.Top;
        }

        private void LinkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e == null)
                return;

            Process.Start("https://github.com/SamDel/ChromeCast-Desktop-Audio-Streamer/wiki#options");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (Size.Width >= 50 && Size.Height >= 50)
                windowSize = Size;
        }

        public void ShowWavMeterValue(byte[] data)
        {
            try
            {
                var maximum = 0f;
                for (int index = 0; index < data.Length; index += 2)
                {
                    var sample = (short)((data[index + 1] << 8) | data[index + 0]);
                    var sample32 = sample / 32768f;
                    if (sample32 > maximum)
                        maximum = sample32;
                }
                volumeMeter.Amplitude = maximum;
            }
            catch (Exception ex)
            {
                logger.Log(ex, "MainFrom.ViewWav");
            }
        }

        /// <summary>
        /// Set the device buffer value.
        /// </summary>
        /// <param name="extraBufferInSecondsIn">buffer in seconds</param>
        public void SetExtraBufferInSeconds(int extraBufferInSecondsIn)
        {
            if (devices == null)
                return;

            for (int i = 0; i < cmbBufferInSeconds.Items.Count; i++)
            {
                if (int.Parse((string)cmbBufferInSeconds.Items[i]) == extraBufferInSecondsIn)
                    cmbBufferInSeconds.SelectedIndex = i;
            }
        }

        /// <summary>
        /// Get the device buffer value (in seconds).
        /// </summary>
        /// <returns>buffer in seconds</returns>
        public int? GetExtraBufferInSeconds()
        {
            if (devices == null)
                return 0;

            return int.Parse((string)cmbBufferInSeconds.SelectedItem);
        }

        /// <summary>
        /// Change the buffer on the devices.
        /// </summary>
        private void CmbBufferInSeconds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBufferInSeconds == null || devices == null)
                return;

            var bufferInSeconds = int.Parse((string)cmbBufferInSeconds.SelectedItem);
            devices.SetExtraBufferInSeconds(bufferInSeconds);
        }

        public void SetRecordingDeviceID(string recordingDeviceIDIn)
        {
            previousRecordingDeviceID = recordingDeviceIDIn;
            isRecordingDeviceSelected = false;
        }

        public string GetRecordingDeviceID()
        {
            if (cmbRecordingDevice.Items.Count == 0 || cmbRecordingDevice.SelectedItem == null)
                return null;

            return ((MMDevice)cmbRecordingDevice.SelectedItem).DeviceID;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
            loopbackRecorder?.Dispose();
            wavGenerator?.Dispose();
        }

        private void ChkAutoMute_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoMute(chkAutoMute.Checked);
        }

        public void SetAutoMute(bool autoMute)
        {
            if (chkLogDeviceCommunication == null || tabControl == null)
                return;

            chkAutoMute.Checked = autoMute;
        }

        public bool GetAutoMute()
        {
            if (chkAutoMute == null)
                return false;

            return chkAutoMute.Checked;
        }

        public void RestartRecording()
        {
            loopbackRecorder.Restart();
        }

        public void SetMinimizeToTray(bool minimizeToTray)
        {
            if (chkMinimizeToTray == null)
                return;

            chkMinimizeToTray.Checked = minimizeToTray;
        }

        public bool GetMinimizeToTray()
        {
            if (chkMinimizeToTray == null)
                return false;

            return chkMinimizeToTray.Checked;
        }
    }
}
