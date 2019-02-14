namespace ChromeCast.Desktop.AudioStreamer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.grpLag = new System.Windows.Forms.GroupBox();
            this.lblLagMin = new System.Windows.Forms.Label();
            this.lblLagMax = new System.Windows.Forms.Label();
            this.trbLag = new System.Windows.Forms.TrackBar();
            this.grpVolume = new System.Windows.Forms.GroupBox();
            this.pnlVolumeAllButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnVolumeUp = new System.Windows.Forms.Button();
            this.btnVolumeDown = new System.Windows.Forms.Button();
            this.btnVolumeMute = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.tabPageOptions = new System.Windows.Forms.TabPage();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkAutoStartLastUsed = new System.Windows.Forms.CheckBox();
            this.chkStartApplicationWhenWindowsStarts = new System.Windows.Forms.CheckBox();
            this.lblNewReleaseAvailable = new System.Windows.Forms.LinkLabel();
            this.chkLogDeviceCommunication = new System.Windows.Forms.CheckBox();
            this.pnlOptionsResetSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.pnlOptionsComboBoxes = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlOptionsComboBoxesLabels = new System.Windows.Forms.Panel();
            this.lblFilterDevices = new System.Windows.Forms.Label();
            this.lblStreamFormat = new System.Windows.Forms.Label();
            this.lblIpAddressUsed = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblDevice = new System.Windows.Forms.Label();
            this.pnlOptionsComboBoxesRight = new System.Windows.Forms.Panel();
            this.cmbFilterDevices = new System.Windows.Forms.ComboBox();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.cmbIP4AddressUsed = new System.Windows.Forms.ComboBox();
            this.cmbRecordingDevice = new System.Windows.Forms.ComboBox();
            this.cmbStreamFormat = new System.Windows.Forms.ComboBox();
            this.chkShowLagControl = new System.Windows.Forms.CheckBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkShowWindowOnStart = new System.Windows.Forms.CheckBox();
            this.chkAutoRestart = new System.Windows.Forms.CheckBox();
            this.chkHook = new System.Windows.Forms.CheckBox();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.pnlLogCopyToClipboard = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClipboardCopy = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.grpDevices = new System.Windows.Forms.GroupBox();
            this.pnlDevices = new System.Windows.Forms.FlowLayoutPanel();
            this.lblLagExperimental = new System.Windows.Forms.Label();
            this.pnlPingPong = new System.Windows.Forms.Panel();
            this.pnlLog = new System.Windows.Forms.Panel();
            this.lblPingPong = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.grpLag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).BeginInit();
            this.grpVolume.SuspendLayout();
            this.pnlVolumeAllButtons.SuspendLayout();
            this.tabPageOptions.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.pnlOptionsResetSettings.SuspendLayout();
            this.pnlOptionsComboBoxes.SuspendLayout();
            this.pnlOptionsComboBoxesLabels.SuspendLayout();
            this.pnlOptionsComboBoxesRight.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.pnlLogCopyToClipboard.SuspendLayout();
            this.grpDevices.SuspendLayout();
            this.pnlPingPong.SuspendLayout();
            this.pnlLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageMain);
            this.tabControl.Controls.Add(this.tabPageOptions);
            this.tabControl.Controls.Add(this.tabPageLog);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(16, 16);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1119, 625);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.grpDevices);
            this.tabPageMain.Controls.Add(this.grpLag);
            this.tabPageMain.Controls.Add(this.grpVolume);
            this.tabPageMain.Location = new System.Drawing.Point(4, 25);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageMain.Size = new System.Drawing.Size(1111, 596);
            this.tabPageMain.TabIndex = 1;
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // grpLag
            // 
            this.grpLag.Controls.Add(this.lblLagExperimental);
            this.grpLag.Controls.Add(this.lblLagMin);
            this.grpLag.Controls.Add(this.lblLagMax);
            this.grpLag.Controls.Add(this.trbLag);
            this.grpLag.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpLag.Location = new System.Drawing.Point(10, 81);
            this.grpLag.Name = "grpLag";
            this.grpLag.Padding = new System.Windows.Forms.Padding(10);
            this.grpLag.Size = new System.Drawing.Size(1091, 113);
            this.grpLag.TabIndex = 12;
            this.grpLag.TabStop = false;
            this.grpLag.Text = "Lag Control";
            // 
            // lblLagMin
            // 
            this.lblLagMin.AutoSize = true;
            this.lblLagMin.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblLagMin.Location = new System.Drawing.Point(10, 25);
            this.lblLagMin.Name = "lblLagMin";
            this.lblLagMin.Size = new System.Drawing.Size(172, 17);
            this.lblLagMin.TabIndex = 2;
            this.lblLagMin.Text = "minimum lag / poor quality";
            // 
            // lblLagMax
            // 
            this.lblLagMax.AutoSize = true;
            this.lblLagMax.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblLagMax.Location = new System.Drawing.Point(908, 25);
            this.lblLagMax.Name = "lblLagMax";
            this.lblLagMax.Size = new System.Drawing.Size(173, 17);
            this.lblLagMax.TabIndex = 1;
            this.lblLagMax.Text = "maximum lag / best quality";
            this.lblLagMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // trbLag
            // 
            this.trbLag.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trbLag.LargeChange = 10;
            this.trbLag.Location = new System.Drawing.Point(10, 47);
            this.trbLag.Maximum = 1000;
            this.trbLag.Minimum = 1;
            this.trbLag.Name = "trbLag";
            this.trbLag.Size = new System.Drawing.Size(1071, 56);
            this.trbLag.SmallChange = 5;
            this.trbLag.TabIndex = 0;
            this.trbLag.TickFrequency = 100;
            this.trbLag.Value = 1000;
            this.trbLag.Scroll += new System.EventHandler(this.TrbLag_Scroll);
            // 
            // grpVolume
            // 
            this.grpVolume.Controls.Add(this.pnlVolumeAllButtons);
            this.grpVolume.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpVolume.Location = new System.Drawing.Point(10, 10);
            this.grpVolume.Name = "grpVolume";
            this.grpVolume.Padding = new System.Windows.Forms.Padding(10);
            this.grpVolume.Size = new System.Drawing.Size(1091, 71);
            this.grpVolume.TabIndex = 10;
            this.grpVolume.TabStop = false;
            this.grpVolume.Text = "Volume all devices:";
            // 
            // pnlVolumeAllButtons
            // 
            this.pnlVolumeAllButtons.AutoSize = true;
            this.pnlVolumeAllButtons.Controls.Add(this.btnVolumeUp);
            this.pnlVolumeAllButtons.Controls.Add(this.btnVolumeDown);
            this.pnlVolumeAllButtons.Controls.Add(this.btnVolumeMute);
            this.pnlVolumeAllButtons.Controls.Add(this.btnScan);
            this.pnlVolumeAllButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlVolumeAllButtons.Location = new System.Drawing.Point(10, 25);
            this.pnlVolumeAllButtons.Name = "pnlVolumeAllButtons";
            this.pnlVolumeAllButtons.Size = new System.Drawing.Size(1071, 33);
            this.pnlVolumeAllButtons.TabIndex = 17;
            // 
            // btnVolumeUp
            // 
            this.btnVolumeUp.AutoSize = true;
            this.btnVolumeUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVolumeUp.Location = new System.Drawing.Point(3, 3);
            this.btnVolumeUp.MinimumSize = new System.Drawing.Size(120, 0);
            this.btnVolumeUp.Name = "btnVolumeUp";
            this.btnVolumeUp.Size = new System.Drawing.Size(120, 27);
            this.btnVolumeUp.TabIndex = 15;
            this.btnVolumeUp.Text = "Up";
            this.btnVolumeUp.UseVisualStyleBackColor = true;
            this.btnVolumeUp.Click += new System.EventHandler(this.BtnVolumeUp_Click);
            // 
            // btnVolumeDown
            // 
            this.btnVolumeDown.AutoSize = true;
            this.btnVolumeDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVolumeDown.Location = new System.Drawing.Point(129, 3);
            this.btnVolumeDown.MinimumSize = new System.Drawing.Size(120, 0);
            this.btnVolumeDown.Name = "btnVolumeDown";
            this.btnVolumeDown.Size = new System.Drawing.Size(120, 27);
            this.btnVolumeDown.TabIndex = 14;
            this.btnVolumeDown.Text = "Down";
            this.btnVolumeDown.UseVisualStyleBackColor = true;
            this.btnVolumeDown.Click += new System.EventHandler(this.BtnVolumeDown_Click);
            // 
            // btnVolumeMute
            // 
            this.btnVolumeMute.AutoSize = true;
            this.btnVolumeMute.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVolumeMute.Location = new System.Drawing.Point(255, 3);
            this.btnVolumeMute.MinimumSize = new System.Drawing.Size(120, 0);
            this.btnVolumeMute.Name = "btnVolumeMute";
            this.btnVolumeMute.Size = new System.Drawing.Size(120, 27);
            this.btnVolumeMute.TabIndex = 11;
            this.btnVolumeMute.Text = "Mute";
            this.btnVolumeMute.UseVisualStyleBackColor = true;
            this.btnVolumeMute.Click += new System.EventHandler(this.BtnVolumeMute_Click);
            // 
            // btnScan
            // 
            this.btnScan.AutoSize = true;
            this.btnScan.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnScan.Location = new System.Drawing.Point(381, 3);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(162, 27);
            this.btnScan.TabIndex = 11;
            this.btnScan.Text = "Scan again for devices";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.BtnScan_Click);
            // 
            // tabPageOptions
            // 
            this.tabPageOptions.Controls.Add(this.grpOptions);
            this.tabPageOptions.Location = new System.Drawing.Point(4, 25);
            this.tabPageOptions.Name = "tabPageOptions";
            this.tabPageOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOptions.Size = new System.Drawing.Size(1111, 596);
            this.tabPageOptions.TabIndex = 2;
            this.tabPageOptions.Text = "Options";
            this.tabPageOptions.UseVisualStyleBackColor = true;
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.chkAutoStartLastUsed);
            this.grpOptions.Controls.Add(this.chkStartApplicationWhenWindowsStarts);
            this.grpOptions.Controls.Add(this.lblNewReleaseAvailable);
            this.grpOptions.Controls.Add(this.chkLogDeviceCommunication);
            this.grpOptions.Controls.Add(this.pnlOptionsResetSettings);
            this.grpOptions.Controls.Add(this.pnlOptionsComboBoxes);
            this.grpOptions.Controls.Add(this.chkShowLagControl);
            this.grpOptions.Controls.Add(this.lblVersion);
            this.grpOptions.Controls.Add(this.chkAutoStart);
            this.grpOptions.Controls.Add(this.chkShowWindowOnStart);
            this.grpOptions.Controls.Add(this.chkAutoRestart);
            this.grpOptions.Controls.Add(this.chkHook);
            this.grpOptions.Location = new System.Drawing.Point(27, 22);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(1038, 553);
            this.grpOptions.TabIndex = 14;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // chkAutoStartLastUsed
            // 
            this.chkAutoStartLastUsed.AutoSize = true;
            this.chkAutoStartLastUsed.Location = new System.Drawing.Point(15, 292);
            this.chkAutoStartLastUsed.Name = "chkAutoStartLastUsed";
            this.chkAutoStartLastUsed.Size = new System.Drawing.Size(398, 21);
            this.chkAutoStartLastUsed.TabIndex = 38;
            this.chkAutoStartLastUsed.Text = "Automatically start last used devices and groups at startup";
            this.chkAutoStartLastUsed.UseVisualStyleBackColor = true;
            // 
            // chkStartApplicationWhenWindowsStarts
            // 
            this.chkStartApplicationWhenWindowsStarts.AutoSize = true;
            this.chkStartApplicationWhenWindowsStarts.Location = new System.Drawing.Point(15, 240);
            this.chkStartApplicationWhenWindowsStarts.Name = "chkStartApplicationWhenWindowsStarts";
            this.chkStartApplicationWhenWindowsStarts.Size = new System.Drawing.Size(268, 21);
            this.chkStartApplicationWhenWindowsStarts.TabIndex = 37;
            this.chkStartApplicationWhenWindowsStarts.Text = "Start application when Windows starts";
            this.chkStartApplicationWhenWindowsStarts.UseVisualStyleBackColor = true;
            this.chkStartApplicationWhenWindowsStarts.CheckedChanged += new System.EventHandler(this.ChkStartApplicationWhenWindowsStarts_CheckedChanged);
            // 
            // lblNewReleaseAvailable
            // 
            this.lblNewReleaseAvailable.AutoSize = true;
            this.lblNewReleaseAvailable.Location = new System.Drawing.Point(12, 485);
            this.lblNewReleaseAvailable.Name = "lblNewReleaseAvailable";
            this.lblNewReleaseAvailable.Size = new System.Drawing.Size(206, 17);
            this.lblNewReleaseAvailable.TabIndex = 36;
            this.lblNewReleaseAvailable.TabStop = true;
            this.lblNewReleaseAvailable.Text = "Version x is available on Github";
            this.lblNewReleaseAvailable.Visible = false;
            this.lblNewReleaseAvailable.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblNewReleaseAvailable_LinkClicked);
            // 
            // chkLogDeviceCommunication
            // 
            this.chkLogDeviceCommunication.AutoSize = true;
            this.chkLogDeviceCommunication.Checked = true;
            this.chkLogDeviceCommunication.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogDeviceCommunication.Location = new System.Drawing.Point(15, 370);
            this.chkLogDeviceCommunication.Name = "chkLogDeviceCommunication";
            this.chkLogDeviceCommunication.Size = new System.Drawing.Size(197, 21);
            this.chkLogDeviceCommunication.TabIndex = 35;
            this.chkLogDeviceCommunication.Text = "Log device communication";
            this.chkLogDeviceCommunication.UseVisualStyleBackColor = true;
            this.chkLogDeviceCommunication.CheckedChanged += new System.EventHandler(this.ChkLogDeviceCommunication_CheckedChanged);
            // 
            // pnlOptionsResetSettings
            // 
            this.pnlOptionsResetSettings.Controls.Add(this.btnResetSettings);
            this.pnlOptionsResetSettings.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlOptionsResetSettings.Location = new System.Drawing.Point(248, 508);
            this.pnlOptionsResetSettings.Name = "pnlOptionsResetSettings";
            this.pnlOptionsResetSettings.Size = new System.Drawing.Size(754, 33);
            this.pnlOptionsResetSettings.TabIndex = 34;
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.AutoSize = true;
            this.btnResetSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnResetSettings.Location = new System.Drawing.Point(641, 3);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(110, 27);
            this.btnResetSettings.TabIndex = 26;
            this.btnResetSettings.Text = "Reset Settings";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.BtnResetSettings_Click);
            // 
            // pnlOptionsComboBoxes
            // 
            this.pnlOptionsComboBoxes.Controls.Add(this.pnlOptionsComboBoxesLabels);
            this.pnlOptionsComboBoxes.Controls.Add(this.pnlOptionsComboBoxesRight);
            this.pnlOptionsComboBoxes.Location = new System.Drawing.Point(0, 29);
            this.pnlOptionsComboBoxes.Name = "pnlOptionsComboBoxes";
            this.pnlOptionsComboBoxes.Size = new System.Drawing.Size(965, 153);
            this.pnlOptionsComboBoxes.TabIndex = 33;
            // 
            // pnlOptionsComboBoxesLabels
            // 
            this.pnlOptionsComboBoxesLabels.AutoSize = true;
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblFilterDevices);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblStreamFormat);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblIpAddressUsed);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblLanguage);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblDevice);
            this.pnlOptionsComboBoxesLabels.Location = new System.Drawing.Point(3, 3);
            this.pnlOptionsComboBoxesLabels.Name = "pnlOptionsComboBoxesLabels";
            this.pnlOptionsComboBoxesLabels.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
            this.pnlOptionsComboBoxesLabels.Size = new System.Drawing.Size(159, 137);
            this.pnlOptionsComboBoxesLabels.TabIndex = 0;
            // 
            // lblFilterDevices
            // 
            this.lblFilterDevices.AutoSize = true;
            this.lblFilterDevices.Location = new System.Drawing.Point(9, 120);
            this.lblFilterDevices.Name = "lblFilterDevices";
            this.lblFilterDevices.Size = new System.Drawing.Size(91, 17);
            this.lblFilterDevices.TabIndex = 32;
            this.lblFilterDevices.Text = "Filter devices";
            // 
            // lblStreamFormat
            // 
            this.lblStreamFormat.AutoSize = true;
            this.lblStreamFormat.Location = new System.Drawing.Point(9, 62);
            this.lblStreamFormat.Name = "lblStreamFormat";
            this.lblStreamFormat.Size = new System.Drawing.Size(97, 17);
            this.lblStreamFormat.TabIndex = 28;
            this.lblStreamFormat.Text = "Stream format";
            // 
            // lblIpAddressUsed
            // 
            this.lblIpAddressUsed.AutoSize = true;
            this.lblIpAddressUsed.Location = new System.Drawing.Point(9, 3);
            this.lblIpAddressUsed.Name = "lblIpAddressUsed";
            this.lblIpAddressUsed.Size = new System.Drawing.Size(122, 17);
            this.lblIpAddressUsed.TabIndex = 19;
            this.lblIpAddressUsed.Text = "IP4 address used:";
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(9, 91);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(72, 17);
            this.lblLanguage.TabIndex = 31;
            this.lblLanguage.Text = "Language";
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(9, 33);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(122, 17);
            this.lblDevice.TabIndex = 21;
            this.lblDevice.Text = "Recording device:";
            // 
            // pnlOptionsComboBoxesRight
            // 
            this.pnlOptionsComboBoxesRight.AutoSize = true;
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbFilterDevices);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbLanguage);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbIP4AddressUsed);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbRecordingDevice);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbStreamFormat);
            this.pnlOptionsComboBoxesRight.Location = new System.Drawing.Point(168, 3);
            this.pnlOptionsComboBoxesRight.Name = "pnlOptionsComboBoxesRight";
            this.pnlOptionsComboBoxesRight.Size = new System.Drawing.Size(448, 145);
            this.pnlOptionsComboBoxesRight.TabIndex = 1;
            // 
            // cmbFilterDevices
            // 
            this.cmbFilterDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterDevices.FormattingEnabled = true;
            this.cmbFilterDevices.Location = new System.Drawing.Point(3, 118);
            this.cmbFilterDevices.Name = "cmbFilterDevices";
            this.cmbFilterDevices.Size = new System.Drawing.Size(442, 24);
            this.cmbFilterDevices.TabIndex = 33;
            this.cmbFilterDevices.SelectedIndexChanged += new System.EventHandler(this.CmbFilterDevices_SelectedIndexChanged);
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(3, 88);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(442, 24);
            this.cmbLanguage.TabIndex = 32;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.CmbLanguage_SelectedIndexChanged);
            // 
            // cmbIP4AddressUsed
            // 
            this.cmbIP4AddressUsed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIP4AddressUsed.FormattingEnabled = true;
            this.cmbIP4AddressUsed.Location = new System.Drawing.Point(3, 0);
            this.cmbIP4AddressUsed.Name = "cmbIP4AddressUsed";
            this.cmbIP4AddressUsed.Size = new System.Drawing.Size(442, 24);
            this.cmbIP4AddressUsed.TabIndex = 20;
            // 
            // cmbRecordingDevice
            // 
            this.cmbRecordingDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordingDevice.FormattingEnabled = true;
            this.cmbRecordingDevice.Location = new System.Drawing.Point(3, 30);
            this.cmbRecordingDevice.Name = "cmbRecordingDevice";
            this.cmbRecordingDevice.Size = new System.Drawing.Size(442, 24);
            this.cmbRecordingDevice.TabIndex = 22;
            // 
            // cmbStreamFormat
            // 
            this.cmbStreamFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStreamFormat.FormattingEnabled = true;
            this.cmbStreamFormat.Location = new System.Drawing.Point(3, 59);
            this.cmbStreamFormat.Name = "cmbStreamFormat";
            this.cmbStreamFormat.Size = new System.Drawing.Size(442, 24);
            this.cmbStreamFormat.TabIndex = 29;
            this.cmbStreamFormat.SelectedIndexChanged += new System.EventHandler(this.CmbStreamFormat_SelectedIndexChanged);
            // 
            // chkShowLagControl
            // 
            this.chkShowLagControl.AutoSize = true;
            this.chkShowLagControl.Location = new System.Drawing.Point(15, 344);
            this.chkShowLagControl.Name = "chkShowLagControl";
            this.chkShowLagControl.Size = new System.Drawing.Size(228, 21);
            this.chkShowLagControl.TabIndex = 27;
            this.chkShowLagControl.Text = "Show lag control (experimental)";
            this.chkShowLagControl.UseVisualStyleBackColor = true;
            this.chkShowLagControl.CheckedChanged += new System.EventHandler(this.ChkShowLagControl_CheckedChanged);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(12, 516);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(56, 17);
            this.lblVersion.TabIndex = 25;
            this.lblVersion.Text = "Version";
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(15, 266);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(261, 21);
            this.chkAutoStart.TabIndex = 24;
            this.chkAutoStart.Text = "Automatically start devices at startup";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // chkShowWindowOnStart
            // 
            this.chkShowWindowOnStart.AutoSize = true;
            this.chkShowWindowOnStart.Checked = true;
            this.chkShowWindowOnStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowWindowOnStart.Location = new System.Drawing.Point(15, 214);
            this.chkShowWindowOnStart.Name = "chkShowWindowOnStart";
            this.chkShowWindowOnStart.Size = new System.Drawing.Size(177, 21);
            this.chkShowWindowOnStart.TabIndex = 23;
            this.chkShowWindowOnStart.Text = "Show window at startup";
            this.chkShowWindowOnStart.UseVisualStyleBackColor = true;
            // 
            // chkAutoRestart
            // 
            this.chkAutoRestart.AutoSize = true;
            this.chkAutoRestart.Location = new System.Drawing.Point(15, 318);
            this.chkAutoRestart.Name = "chkAutoRestart";
            this.chkAutoRestart.Size = new System.Drawing.Size(325, 21);
            this.chkAutoRestart.TabIndex = 18;
            this.chkAutoRestart.Text = "Automatically restart when the stream is closed";
            this.chkAutoRestart.UseVisualStyleBackColor = true;
            this.chkAutoRestart.CheckedChanged += new System.EventHandler(this.ChkAutoRestart_CheckedChanged);
            // 
            // chkHook
            // 
            this.chkHook.AutoSize = true;
            this.chkHook.Location = new System.Drawing.Point(15, 188);
            this.chkHook.Name = "chkHook";
            this.chkHook.Size = new System.Drawing.Size(556, 21);
            this.chkHook.TabIndex = 14;
            this.chkHook.Text = "Use Keyboard shortcuts: Up = Ctrl+Alt+U; Down = Ctrl+Alt+D; (Un)Mute = Ctrl+Alt+M" +
    "";
            this.chkHook.UseVisualStyleBackColor = true;
            this.chkHook.CheckedChanged += new System.EventHandler(this.ChkHook_CheckedChanged);
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.pnlLog);
            this.tabPageLog.Controls.Add(this.pnlPingPong);
            this.tabPageLog.Controls.Add(this.pnlLogCopyToClipboard);
            this.tabPageLog.Location = new System.Drawing.Point(4, 25);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(20);
            this.tabPageLog.Size = new System.Drawing.Size(1111, 596);
            this.tabPageLog.TabIndex = 0;
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // pnlLogCopyToClipboard
            // 
            this.pnlLogCopyToClipboard.AutoSize = true;
            this.pnlLogCopyToClipboard.Controls.Add(this.btnClipboardCopy);
            this.pnlLogCopyToClipboard.Controls.Add(this.btnClearLog);
            this.pnlLogCopyToClipboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLogCopyToClipboard.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlLogCopyToClipboard.Location = new System.Drawing.Point(20, 543);
            this.pnlLogCopyToClipboard.Name = "pnlLogCopyToClipboard";
            this.pnlLogCopyToClipboard.Size = new System.Drawing.Size(1071, 33);
            this.pnlLogCopyToClipboard.TabIndex = 4;
            // 
            // btnClipboardCopy
            // 
            this.btnClipboardCopy.AutoSize = true;
            this.btnClipboardCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClipboardCopy.Location = new System.Drawing.Point(940, 3);
            this.btnClipboardCopy.Name = "btnClipboardCopy";
            this.btnClipboardCopy.Size = new System.Drawing.Size(128, 27);
            this.btnClipboardCopy.TabIndex = 3;
            this.btnClipboardCopy.Text = "Copy to clipboard";
            this.btnClipboardCopy.UseVisualStyleBackColor = true;
            this.btnClipboardCopy.Click += new System.EventHandler(this.BtnClipboardCopy_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.AutoSize = true;
            this.btnClearLog.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClearLog.Location = new System.Drawing.Point(855, 3);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(79, 27);
            this.btnClearLog.TabIndex = 4;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.BtnClearLog_Click);
            // 
            // grpDevices
            // 
            this.grpDevices.Controls.Add(this.pnlDevices);
            this.grpDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDevices.Location = new System.Drawing.Point(10, 194);
            this.grpDevices.Name = "grpDevices";
            this.grpDevices.Padding = new System.Windows.Forms.Padding(10);
            this.grpDevices.Size = new System.Drawing.Size(1091, 392);
            this.grpDevices.TabIndex = 13;
            this.grpDevices.TabStop = false;
            this.grpDevices.Text = "Devices (click name to start streaming)";
            // 
            // pnlDevices
            // 
            this.pnlDevices.AutoScroll = true;
            this.pnlDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDevices.Location = new System.Drawing.Point(10, 25);
            this.pnlDevices.Name = "pnlDevices";
            this.pnlDevices.Size = new System.Drawing.Size(1071, 357);
            this.pnlDevices.TabIndex = 10;
            // 
            // lblLagExperimental
            // 
            this.lblLagExperimental.BackColor = System.Drawing.SystemColors.Control;
            this.lblLagExperimental.Location = new System.Drawing.Point(120, 86);
            this.lblLagExperimental.Name = "lblLagExperimental";
            this.lblLagExperimental.Size = new System.Drawing.Size(726, 17);
            this.lblLagExperimental.TabIndex = 4;
            this.lblLagExperimental.Text = "Experimental feature: Try to keep the buffer on the device as small as possible w" +
    "ithout hearing gaps.";
            this.lblLagExperimental.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pnlPingPong
            // 
            this.pnlPingPong.AutoSize = true;
            this.pnlPingPong.Controls.Add(this.lblPingPong);
            this.pnlPingPong.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPingPong.Location = new System.Drawing.Point(20, 520);
            this.pnlPingPong.Name = "pnlPingPong";
            this.pnlPingPong.Padding = new System.Windows.Forms.Padding(3);
            this.pnlPingPong.Size = new System.Drawing.Size(1071, 23);
            this.pnlPingPong.TabIndex = 5;
            // 
            // pnlLog
            // 
            this.pnlLog.Controls.Add(this.txtLog);
            this.pnlLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLog.Location = new System.Drawing.Point(20, 20);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Size = new System.Drawing.Size(1071, 500);
            this.pnlLog.TabIndex = 6;
            // 
            // lblPingPong
            // 
            this.lblPingPong.AutoSize = true;
            this.lblPingPong.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPingPong.Location = new System.Drawing.Point(3, 3);
            this.lblPingPong.Name = "lblPingPong";
            this.lblPingPong.Size = new System.Drawing.Size(148, 17);
            this.lblPingPong.TabIndex = 3;
            this.lblPingPong.Text = "                                   ";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Courier New", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1071, 500);
            this.txtLog.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 657);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(16);
            this.Text = "Chromecast Desktop Audio Streamer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.grpLag.ResumeLayout(false);
            this.grpLag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).EndInit();
            this.grpVolume.ResumeLayout(false);
            this.grpVolume.PerformLayout();
            this.pnlVolumeAllButtons.ResumeLayout(false);
            this.pnlVolumeAllButtons.PerformLayout();
            this.tabPageOptions.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.pnlOptionsResetSettings.ResumeLayout(false);
            this.pnlOptionsResetSettings.PerformLayout();
            this.pnlOptionsComboBoxes.ResumeLayout(false);
            this.pnlOptionsComboBoxes.PerformLayout();
            this.pnlOptionsComboBoxesLabels.ResumeLayout(false);
            this.pnlOptionsComboBoxesLabels.PerformLayout();
            this.pnlOptionsComboBoxesRight.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.pnlLogCopyToClipboard.ResumeLayout(false);
            this.pnlLogCopyToClipboard.PerformLayout();
            this.grpDevices.ResumeLayout(false);
            this.pnlPingPong.ResumeLayout(false);
            this.pnlPingPong.PerformLayout();
            this.pnlLog.ResumeLayout(false);
            this.pnlLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.GroupBox grpLag;
        private System.Windows.Forms.TrackBar trbLag;
        private System.Windows.Forms.GroupBox grpVolume;
        private System.Windows.Forms.Button btnVolumeMute;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Label lblLagMin;
        private System.Windows.Forms.Label lblLagMax;
        private System.Windows.Forms.Button btnVolumeUp;
        private System.Windows.Forms.Button btnVolumeDown;
        private System.Windows.Forms.Button btnClipboardCopy;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.TabPage tabPageOptions;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkShowWindowOnStart;
        private System.Windows.Forms.ComboBox cmbRecordingDevice;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.ComboBox cmbIP4AddressUsed;
        private System.Windows.Forms.Label lblIpAddressUsed;
        private System.Windows.Forms.CheckBox chkAutoRestart;
        private System.Windows.Forms.CheckBox chkHook;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnResetSettings;
        private System.Windows.Forms.CheckBox chkShowLagControl;
        private System.Windows.Forms.ComboBox cmbStreamFormat;
        private System.Windows.Forms.Label lblStreamFormat;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.FlowLayoutPanel pnlVolumeAllButtons;
        private System.Windows.Forms.FlowLayoutPanel pnlOptionsComboBoxes;
        private System.Windows.Forms.Panel pnlOptionsComboBoxesLabels;
        private System.Windows.Forms.Panel pnlOptionsComboBoxesRight;
        private System.Windows.Forms.FlowLayoutPanel pnlLogCopyToClipboard;
        private System.Windows.Forms.FlowLayoutPanel pnlOptionsResetSettings;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkLogDeviceCommunication;
        private System.Windows.Forms.LinkLabel lblNewReleaseAvailable;
        private System.Windows.Forms.CheckBox chkStartApplicationWhenWindowsStarts;
        private System.Windows.Forms.Label lblFilterDevices;
        private System.Windows.Forms.ComboBox cmbFilterDevices;
        private System.Windows.Forms.CheckBox chkAutoStartLastUsed;
        private System.Windows.Forms.GroupBox grpDevices;
        private System.Windows.Forms.FlowLayoutPanel pnlDevices;
        private System.Windows.Forms.Label lblLagExperimental;
        private System.Windows.Forms.Panel pnlLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel pnlPingPong;
        private System.Windows.Forms.Label lblPingPong;
    }
}

