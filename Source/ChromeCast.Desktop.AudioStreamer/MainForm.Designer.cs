namespace ChromeCast.Desktop.AudioStreamer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.grpDevices = new System.Windows.Forms.GroupBox();
            this.pnlDevices = new System.Windows.Forms.FlowLayoutPanel();
            this.grpLag = new System.Windows.Forms.GroupBox();
            this.lblLagExperimental = new System.Windows.Forms.Label();
            this.lblLagMin = new System.Windows.Forms.Label();
            this.lblLagMax = new System.Windows.Forms.Label();
            this.trbLag = new System.Windows.Forms.TrackBar();
            this.grpVolume = new System.Windows.Forms.GroupBox();
            this.pnlVolumeMeter = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDb = new System.Windows.Forms.Label();
            this.volumeMeter = new NAudio.Gui.VolumeMeter();
            this.pnlVolumeAllButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnVolumeUp = new System.Windows.Forms.Button();
            this.btnVolumeDown = new System.Windows.Forms.Button();
            this.btnVolumeMute = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.tabPageOptions = new System.Windows.Forms.TabPage();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlResetSettings = new System.Windows.Forms.Panel();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.pnlOptionsCheckBoxes = new System.Windows.Forms.Panel();
            this.chkDarkMode = new System.Windows.Forms.CheckBox();
            this.chkConvertMultiChannelToStereo = new System.Windows.Forms.CheckBox();
            this.chkMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.chkAutoMute = new System.Windows.Forms.CheckBox();
            this.chkLogDeviceCommunication = new System.Windows.Forms.CheckBox();
            this.chkShowLagControl = new System.Windows.Forms.CheckBox();
            this.chkAutoRestart = new System.Windows.Forms.CheckBox();
            this.chkAutoStartLastUsed = new System.Windows.Forms.CheckBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkStartApplicationWhenWindowsStarts = new System.Windows.Forms.CheckBox();
            this.chkShowWindowOnStart = new System.Windows.Forms.CheckBox();
            this.chkHook = new System.Windows.Forms.CheckBox();
            this.pnlOptionsComboBoxes = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlOptionsComboBoxesLabels = new System.Windows.Forms.Panel();
            this.lblBufferInSeconds = new System.Windows.Forms.Label();
            this.lblFilterDevices = new System.Windows.Forms.Label();
            this.lblStreamFormat = new System.Windows.Forms.Label();
            this.lblIpAddressUsed = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblDevice = new System.Windows.Forms.Label();
            this.pnlOptionsComboBoxesRight = new System.Windows.Forms.Panel();
            this.cmbBufferInSeconds = new System.Windows.Forms.ComboBox();
            this.cmbFilterDevices = new System.Windows.Forms.ComboBox();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.cmbIP4AddressUsed = new System.Windows.Forms.ComboBox();
            this.cmbRecordingDevice = new System.Windows.Forms.ComboBox();
            this.cmbStreamFormat = new System.Windows.Forms.ComboBox();
            this.lblNewReleaseAvailable = new System.Windows.Forms.LinkLabel();
            this.linkHelp = new System.Windows.Forms.LinkLabel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.pnlLog = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.pnlPingPong = new System.Windows.Forms.Panel();
            this.lblPingPong = new System.Windows.Forms.Label();
            this.pnlLogCopyToClipboard = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClipboardCopy = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.volumeMeterTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.grpDevices.SuspendLayout();
            this.grpLag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).BeginInit();
            this.grpVolume.SuspendLayout();
            this.pnlVolumeMeter.SuspendLayout();
            this.pnlVolumeAllButtons.SuspendLayout();
            this.tabPageOptions.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.pnlResetSettings.SuspendLayout();
            this.pnlOptionsCheckBoxes.SuspendLayout();
            this.pnlOptionsComboBoxes.SuspendLayout();
            this.pnlOptionsComboBoxesLabels.SuspendLayout();
            this.pnlOptionsComboBoxesRight.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.pnlLog.SuspendLayout();
            this.pnlPingPong.SuspendLayout();
            this.pnlLogCopyToClipboard.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageMain);
            this.tabControl.Controls.Add(this.tabPageOptions);
            this.tabControl.Controls.Add(this.tabPageLog);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(16, 20);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1119, 781);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.grpDevices);
            this.tabPageMain.Controls.Add(this.grpLag);
            this.tabPageMain.Controls.Add(this.grpVolume);
            this.tabPageMain.Location = new System.Drawing.Point(4, 29);
            this.tabPageMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(20, 25, 20, 25);
            this.tabPageMain.Size = new System.Drawing.Size(1111, 748);
            this.tabPageMain.TabIndex = 1;
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // grpDevices
            // 
            this.grpDevices.Controls.Add(this.pnlDevices);
            this.grpDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDevices.Location = new System.Drawing.Point(20, 250);
            this.grpDevices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpDevices.Name = "grpDevices";
            this.grpDevices.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.grpDevices.Size = new System.Drawing.Size(1071, 473);
            this.grpDevices.TabIndex = 13;
            this.grpDevices.TabStop = false;
            this.grpDevices.Text = "Devices (click name to start streaming)";
            // 
            // pnlDevices
            // 
            this.pnlDevices.AutoScroll = true;
            this.pnlDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDevices.Location = new System.Drawing.Point(10, 32);
            this.pnlDevices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlDevices.Name = "pnlDevices";
            this.pnlDevices.Size = new System.Drawing.Size(1051, 429);
            this.pnlDevices.TabIndex = 10;
            // 
            // grpLag
            // 
            this.grpLag.Controls.Add(this.lblLagExperimental);
            this.grpLag.Controls.Add(this.lblLagMin);
            this.grpLag.Controls.Add(this.lblLagMax);
            this.grpLag.Controls.Add(this.trbLag);
            this.grpLag.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpLag.Location = new System.Drawing.Point(20, 109);
            this.grpLag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpLag.Name = "grpLag";
            this.grpLag.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            this.grpLag.Size = new System.Drawing.Size(1071, 141);
            this.grpLag.TabIndex = 12;
            this.grpLag.TabStop = false;
            this.grpLag.Text = "Lag Control";
            this.grpLag.Visible = false;
            // 
            // lblLagExperimental
            // 
            this.lblLagExperimental.BackColor = System.Drawing.SystemColors.Control;
            this.lblLagExperimental.Location = new System.Drawing.Point(23, 104);
            this.lblLagExperimental.Name = "lblLagExperimental";
            this.lblLagExperimental.Size = new System.Drawing.Size(726, 21);
            this.lblLagExperimental.TabIndex = 4;
            this.lblLagExperimental.Text = "Experimental feature: Try to keep the buffer on the device as small as possible w" +
    "ithout hearing gaps.";
            this.lblLagExperimental.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLagMin
            // 
            this.lblLagMin.AutoSize = true;
            this.lblLagMin.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblLagMin.Location = new System.Drawing.Point(20, 32);
            this.lblLagMin.Name = "lblLagMin";
            this.lblLagMin.Size = new System.Drawing.Size(192, 20);
            this.lblLagMin.TabIndex = 2;
            this.lblLagMin.Text = "minimum lag / poor quality";
            // 
            // lblLagMax
            // 
            this.lblLagMax.AutoSize = true;
            this.lblLagMax.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblLagMax.Location = new System.Drawing.Point(860, 32);
            this.lblLagMax.Name = "lblLagMax";
            this.lblLagMax.Size = new System.Drawing.Size(191, 20);
            this.lblLagMax.TabIndex = 1;
            this.lblLagMax.Text = "maximum lag / best quality";
            this.lblLagMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // trbLag
            // 
            this.trbLag.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trbLag.LargeChange = 10;
            this.trbLag.Location = new System.Drawing.Point(20, 73);
            this.trbLag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trbLag.Maximum = 1000;
            this.trbLag.Minimum = 1;
            this.trbLag.Name = "trbLag";
            this.trbLag.Size = new System.Drawing.Size(1031, 56);
            this.trbLag.SmallChange = 5;
            this.trbLag.TabIndex = 0;
            this.trbLag.TickFrequency = 100;
            this.trbLag.Value = 1000;
            this.trbLag.Scroll += new System.EventHandler(this.TrbLag_Scroll);
            // 
            // grpVolume
            // 
            this.grpVolume.Controls.Add(this.pnlVolumeMeter);
            this.grpVolume.Controls.Add(this.pnlVolumeAllButtons);
            this.grpVolume.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpVolume.Location = new System.Drawing.Point(20, 25);
            this.grpVolume.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpVolume.Name = "grpVolume";
            this.grpVolume.Padding = new System.Windows.Forms.Padding(10, 2, 2, 4);
            this.grpVolume.Size = new System.Drawing.Size(1071, 84);
            this.grpVolume.TabIndex = 10;
            this.grpVolume.TabStop = false;
            this.grpVolume.Text = "Volume all devices:";
            // 
            // pnlVolumeMeter
            // 
            this.pnlVolumeMeter.AutoSize = true;
            this.pnlVolumeMeter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlVolumeMeter.Controls.Add(this.lblDb);
            this.pnlVolumeMeter.Controls.Add(this.volumeMeter);
            this.pnlVolumeMeter.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlVolumeMeter.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlVolumeMeter.Location = new System.Drawing.Point(1021, 22);
            this.pnlVolumeMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlVolumeMeter.Name = "pnlVolumeMeter";
            this.pnlVolumeMeter.Size = new System.Drawing.Size(48, 58);
            this.pnlVolumeMeter.TabIndex = 18;
            this.pnlVolumeMeter.WrapContents = false;
            // 
            // lblDb
            // 
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(18, 0);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(27, 20);
            this.lblDb.TabIndex = 20;
            this.lblDb.Text = "dB";
            // 
            // volumeMeter
            // 
            this.volumeMeter.Amplitude = 0F;
            this.volumeMeter.BackColor = System.Drawing.SystemColors.Window;
            this.volumeMeter.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.volumeMeter.Location = new System.Drawing.Point(3, 6);
            this.volumeMeter.Margin = new System.Windows.Forms.Padding(3, 6, 3, 4);
            this.volumeMeter.MaxDb = 18F;
            this.volumeMeter.MinDb = -60F;
            this.volumeMeter.Name = "volumeMeter";
            this.volumeMeter.Size = new System.Drawing.Size(9, 50);
            this.volumeMeter.TabIndex = 19;
            // 
            // pnlVolumeAllButtons
            // 
            this.pnlVolumeAllButtons.AutoSize = true;
            this.pnlVolumeAllButtons.Controls.Add(this.btnVolumeUp);
            this.pnlVolumeAllButtons.Controls.Add(this.btnVolumeDown);
            this.pnlVolumeAllButtons.Controls.Add(this.btnVolumeMute);
            this.pnlVolumeAllButtons.Controls.Add(this.btnScan);
            this.pnlVolumeAllButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlVolumeAllButtons.Location = new System.Drawing.Point(10, 22);
            this.pnlVolumeAllButtons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlVolumeAllButtons.Name = "pnlVolumeAllButtons";
            this.pnlVolumeAllButtons.Size = new System.Drawing.Size(551, 58);
            this.pnlVolumeAllButtons.TabIndex = 17;
            this.pnlVolumeAllButtons.WrapContents = false;
            // 
            // btnVolumeUp
            // 
            this.btnVolumeUp.AutoSize = true;
            this.btnVolumeUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVolumeUp.Location = new System.Drawing.Point(3, 4);
            this.btnVolumeUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnVolumeUp.MinimumSize = new System.Drawing.Size(120, 0);
            this.btnVolumeUp.Name = "btnVolumeUp";
            this.btnVolumeUp.Size = new System.Drawing.Size(120, 30);
            this.btnVolumeUp.TabIndex = 15;
            this.btnVolumeUp.Text = "Up";
            this.btnVolumeUp.UseVisualStyleBackColor = true;
            this.btnVolumeUp.Click += new System.EventHandler(this.BtnVolumeUp_Click);
            // 
            // btnVolumeDown
            // 
            this.btnVolumeDown.AutoSize = true;
            this.btnVolumeDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVolumeDown.Location = new System.Drawing.Point(129, 4);
            this.btnVolumeDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnVolumeDown.MinimumSize = new System.Drawing.Size(120, 0);
            this.btnVolumeDown.Name = "btnVolumeDown";
            this.btnVolumeDown.Size = new System.Drawing.Size(120, 30);
            this.btnVolumeDown.TabIndex = 14;
            this.btnVolumeDown.Text = "Down";
            this.btnVolumeDown.UseVisualStyleBackColor = true;
            this.btnVolumeDown.Click += new System.EventHandler(this.BtnVolumeDown_Click);
            // 
            // btnVolumeMute
            // 
            this.btnVolumeMute.AutoSize = true;
            this.btnVolumeMute.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVolumeMute.Location = new System.Drawing.Point(255, 4);
            this.btnVolumeMute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnVolumeMute.MinimumSize = new System.Drawing.Size(120, 0);
            this.btnVolumeMute.Name = "btnVolumeMute";
            this.btnVolumeMute.Size = new System.Drawing.Size(120, 30);
            this.btnVolumeMute.TabIndex = 11;
            this.btnVolumeMute.Text = "Mute";
            this.btnVolumeMute.UseVisualStyleBackColor = true;
            this.btnVolumeMute.Click += new System.EventHandler(this.BtnVolumeMute_Click);
            // 
            // btnScan
            // 
            this.btnScan.AutoSize = true;
            this.btnScan.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnScan.Location = new System.Drawing.Point(381, 4);
            this.btnScan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(167, 30);
            this.btnScan.TabIndex = 11;
            this.btnScan.Text = "Scan again for devices";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.BtnScan_Click);
            // 
            // tabPageOptions
            // 
            this.tabPageOptions.Controls.Add(this.grpOptions);
            this.tabPageOptions.Location = new System.Drawing.Point(4, 29);
            this.tabPageOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageOptions.Name = "tabPageOptions";
            this.tabPageOptions.Padding = new System.Windows.Forms.Padding(20, 25, 20, 25);
            this.tabPageOptions.Size = new System.Drawing.Size(1111, 748);
            this.tabPageOptions.TabIndex = 2;
            this.tabPageOptions.Text = "Options";
            this.tabPageOptions.UseVisualStyleBackColor = true;
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.pnlOptions);
            this.grpOptions.Controls.Add(this.lblNewReleaseAvailable);
            this.grpOptions.Controls.Add(this.linkHelp);
            this.grpOptions.Controls.Add(this.lblVersion);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.grpOptions.Location = new System.Drawing.Point(20, 25);
            this.grpOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Padding = new System.Windows.Forms.Padding(10, 50, 10, 12);
            this.grpOptions.Size = new System.Drawing.Size(1071, 698);
            this.grpOptions.TabIndex = 14;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoScroll = true;
            this.pnlOptions.AutoSize = true;
            this.pnlOptions.Controls.Add(this.flowLayoutPanel1);
            this.pnlOptions.Controls.Add(this.pnlResetSettings);
            this.pnlOptions.Controls.Add(this.pnlOptionsCheckBoxes);
            this.pnlOptions.Controls.Add(this.pnlOptionsComboBoxes);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(10, 67);
            this.pnlOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(1051, 567);
            this.pnlOptions.TabIndex = 41;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 517);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1030, 10);
            this.flowLayoutPanel1.TabIndex = 43;
            // 
            // pnlResetSettings
            // 
            this.pnlResetSettings.Controls.Add(this.btnResetSettings);
            this.pnlResetSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlResetSettings.Location = new System.Drawing.Point(0, 527);
            this.pnlResetSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlResetSettings.Name = "pnlResetSettings";
            this.pnlResetSettings.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pnlResetSettings.Size = new System.Drawing.Size(1030, 71);
            this.pnlResetSettings.TabIndex = 36;
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.AutoSize = true;
            this.btnResetSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnResetSettings.Location = new System.Drawing.Point(15, 4);
            this.btnResetSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(114, 28);
            this.btnResetSettings.TabIndex = 46;
            this.btnResetSettings.Text = "Reset Settings";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.BtnResetSettings_Click);
            // 
            // pnlOptionsCheckBoxes
            // 
            this.pnlOptionsCheckBoxes.AutoSize = true;
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkDarkMode);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkConvertMultiChannelToStereo);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkMinimizeToTray);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkAutoMute);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkLogDeviceCommunication);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkShowLagControl);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkAutoRestart);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkAutoStartLastUsed);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkAutoStart);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkStartApplicationWhenWindowsStarts);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkShowWindowOnStart);
            this.pnlOptionsCheckBoxes.Controls.Add(this.chkHook);
            this.pnlOptionsCheckBoxes.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptionsCheckBoxes.Location = new System.Drawing.Point(0, 203);
            this.pnlOptionsCheckBoxes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlOptionsCheckBoxes.Name = "pnlOptionsCheckBoxes";
            this.pnlOptionsCheckBoxes.Padding = new System.Windows.Forms.Padding(15, 25, 20, 25);
            this.pnlOptionsCheckBoxes.Size = new System.Drawing.Size(1030, 314);
            this.pnlOptionsCheckBoxes.TabIndex = 35;
            // 
            // chkDarkMode
            // 
            this.chkDarkMode.AutoSize = true;
            this.chkDarkMode.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkDarkMode.Location = new System.Drawing.Point(15, 267);
            this.chkDarkMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkDarkMode.Name = "chkDarkMode";
            this.chkDarkMode.Size = new System.Drawing.Size(995, 22);
            this.chkDarkMode.TabIndex = 47;
            this.chkDarkMode.Text = "Dark mode";
            this.chkDarkMode.UseVisualStyleBackColor = true;
            this.chkDarkMode.CheckedChanged += new System.EventHandler(this.chkDarkMode_CheckedChanged);
            // 
            // chkConvertMultiChannelToStereo
            // 
            this.chkConvertMultiChannelToStereo.AutoSize = true;
            this.chkConvertMultiChannelToStereo.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkConvertMultiChannelToStereo.Location = new System.Drawing.Point(15, 245);
            this.chkConvertMultiChannelToStereo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkConvertMultiChannelToStereo.Name = "chkConvertMultiChannelToStereo";
            this.chkConvertMultiChannelToStereo.Size = new System.Drawing.Size(995, 22);
            this.chkConvertMultiChannelToStereo.TabIndex = 46;
            this.chkConvertMultiChannelToStereo.Text = "Convert multi-channel audio to stereo output";
            this.chkConvertMultiChannelToStereo.UseVisualStyleBackColor = true;
            // 
            // chkMinimizeToTray
            // 
            this.chkMinimizeToTray.AutoSize = true;
            this.chkMinimizeToTray.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkMinimizeToTray.Location = new System.Drawing.Point(15, 223);
            this.chkMinimizeToTray.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkMinimizeToTray.Name = "chkMinimizeToTray";
            this.chkMinimizeToTray.Size = new System.Drawing.Size(995, 22);
            this.chkMinimizeToTray.TabIndex = 45;
            this.chkMinimizeToTray.Text = "Minimize to tray when closing";
            this.chkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // chkAutoMute
            // 
            this.chkAutoMute.AutoSize = true;
            this.chkAutoMute.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAutoMute.Location = new System.Drawing.Point(15, 201);
            this.chkAutoMute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkAutoMute.Name = "chkAutoMute";
            this.chkAutoMute.Size = new System.Drawing.Size(995, 22);
            this.chkAutoMute.TabIndex = 44;
            this.chkAutoMute.Text = "Auto-mute/unmute desktop audio";
            this.chkAutoMute.UseVisualStyleBackColor = true;
            this.chkAutoMute.CheckedChanged += new System.EventHandler(this.ChkAutoMute_CheckedChanged);
            // 
            // chkLogDeviceCommunication
            // 
            this.chkLogDeviceCommunication.AutoSize = true;
            this.chkLogDeviceCommunication.Checked = true;
            this.chkLogDeviceCommunication.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogDeviceCommunication.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkLogDeviceCommunication.Location = new System.Drawing.Point(15, 179);
            this.chkLogDeviceCommunication.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkLogDeviceCommunication.Name = "chkLogDeviceCommunication";
            this.chkLogDeviceCommunication.Size = new System.Drawing.Size(995, 22);
            this.chkLogDeviceCommunication.TabIndex = 43;
            this.chkLogDeviceCommunication.Text = "Log device communication";
            this.chkLogDeviceCommunication.UseVisualStyleBackColor = true;
            this.chkLogDeviceCommunication.CheckedChanged += new System.EventHandler(this.ChkLogDeviceCommunication_CheckedChanged);
            // 
            // chkShowLagControl
            // 
            this.chkShowLagControl.AutoSize = true;
            this.chkShowLagControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkShowLagControl.Location = new System.Drawing.Point(15, 157);
            this.chkShowLagControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowLagControl.Name = "chkShowLagControl";
            this.chkShowLagControl.Size = new System.Drawing.Size(995, 22);
            this.chkShowLagControl.TabIndex = 42;
            this.chkShowLagControl.Text = "Show lag control (experimental)";
            this.chkShowLagControl.UseVisualStyleBackColor = true;
            this.chkShowLagControl.Visible = false;
            this.chkShowLagControl.CheckedChanged += new System.EventHandler(this.ChkShowLagControl_CheckedChanged);
            // 
            // chkAutoRestart
            // 
            this.chkAutoRestart.AutoSize = true;
            this.chkAutoRestart.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAutoRestart.Location = new System.Drawing.Point(15, 135);
            this.chkAutoRestart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkAutoRestart.Name = "chkAutoRestart";
            this.chkAutoRestart.Size = new System.Drawing.Size(995, 22);
            this.chkAutoRestart.TabIndex = 41;
            this.chkAutoRestart.Text = "Automatically restart when the stream is closed";
            this.chkAutoRestart.UseVisualStyleBackColor = true;
            this.chkAutoRestart.CheckedChanged += new System.EventHandler(this.ChkAutoRestart_CheckedChanged);
            // 
            // chkAutoStartLastUsed
            // 
            this.chkAutoStartLastUsed.AutoSize = true;
            this.chkAutoStartLastUsed.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAutoStartLastUsed.Location = new System.Drawing.Point(15, 113);
            this.chkAutoStartLastUsed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkAutoStartLastUsed.Name = "chkAutoStartLastUsed";
            this.chkAutoStartLastUsed.Size = new System.Drawing.Size(995, 22);
            this.chkAutoStartLastUsed.TabIndex = 40;
            this.chkAutoStartLastUsed.Text = "Automatically start last used devices and groups at startup";
            this.chkAutoStartLastUsed.UseVisualStyleBackColor = true;
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAutoStart.Location = new System.Drawing.Point(15, 91);
            this.chkAutoStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(995, 22);
            this.chkAutoStart.TabIndex = 39;
            this.chkAutoStart.Text = "Automatically start devices at startup";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // chkStartApplicationWhenWindowsStarts
            // 
            this.chkStartApplicationWhenWindowsStarts.AutoSize = true;
            this.chkStartApplicationWhenWindowsStarts.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkStartApplicationWhenWindowsStarts.Location = new System.Drawing.Point(15, 69);
            this.chkStartApplicationWhenWindowsStarts.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkStartApplicationWhenWindowsStarts.Name = "chkStartApplicationWhenWindowsStarts";
            this.chkStartApplicationWhenWindowsStarts.Size = new System.Drawing.Size(995, 22);
            this.chkStartApplicationWhenWindowsStarts.TabIndex = 38;
            this.chkStartApplicationWhenWindowsStarts.Text = "Start application when Windows starts";
            this.chkStartApplicationWhenWindowsStarts.UseVisualStyleBackColor = true;
            this.chkStartApplicationWhenWindowsStarts.CheckedChanged += new System.EventHandler(this.ChkStartApplicationWhenWindowsStarts_CheckedChanged);
            // 
            // chkShowWindowOnStart
            // 
            this.chkShowWindowOnStart.AutoSize = true;
            this.chkShowWindowOnStart.Checked = true;
            this.chkShowWindowOnStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowWindowOnStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkShowWindowOnStart.Location = new System.Drawing.Point(15, 47);
            this.chkShowWindowOnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowWindowOnStart.Name = "chkShowWindowOnStart";
            this.chkShowWindowOnStart.Size = new System.Drawing.Size(995, 22);
            this.chkShowWindowOnStart.TabIndex = 24;
            this.chkShowWindowOnStart.Text = "Show window at startup";
            this.chkShowWindowOnStart.UseVisualStyleBackColor = true;
            // 
            // chkHook
            // 
            this.chkHook.AutoSize = true;
            this.chkHook.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkHook.Location = new System.Drawing.Point(15, 25);
            this.chkHook.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkHook.Name = "chkHook";
            this.chkHook.Size = new System.Drawing.Size(995, 22);
            this.chkHook.TabIndex = 15;
            this.chkHook.Text = "Use Keyboard shortcuts: Up = Ctrl+Alt+U; Down = Ctrl+Alt+D; (Un)Mute = Ctrl+Alt+M" +
    "";
            this.chkHook.UseVisualStyleBackColor = true;
            this.chkHook.CheckedChanged += new System.EventHandler(this.ChkHook_CheckedChanged);
            // 
            // pnlOptionsComboBoxes
            // 
            this.pnlOptionsComboBoxes.AutoSize = true;
            this.pnlOptionsComboBoxes.BackColor = System.Drawing.Color.Transparent;
            this.pnlOptionsComboBoxes.Controls.Add(this.pnlOptionsComboBoxesLabels);
            this.pnlOptionsComboBoxes.Controls.Add(this.pnlOptionsComboBoxesRight);
            this.pnlOptionsComboBoxes.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptionsComboBoxes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.pnlOptionsComboBoxes.Location = new System.Drawing.Point(0, 0);
            this.pnlOptionsComboBoxes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlOptionsComboBoxes.Name = "pnlOptionsComboBoxes";
            this.pnlOptionsComboBoxes.Size = new System.Drawing.Size(1030, 203);
            this.pnlOptionsComboBoxes.TabIndex = 42;
            this.pnlOptionsComboBoxes.WrapContents = false;
            // 
            // pnlOptionsComboBoxesLabels
            // 
            this.pnlOptionsComboBoxesLabels.AutoSize = true;
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblBufferInSeconds);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblFilterDevices);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblStreamFormat);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblIpAddressUsed);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblLanguage);
            this.pnlOptionsComboBoxesLabels.Controls.Add(this.lblDevice);
            this.pnlOptionsComboBoxesLabels.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlOptionsComboBoxesLabels.Location = new System.Drawing.Point(3, 4);
            this.pnlOptionsComboBoxesLabels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlOptionsComboBoxesLabels.Name = "pnlOptionsComboBoxesLabels";
            this.pnlOptionsComboBoxesLabels.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
            this.pnlOptionsComboBoxesLabels.Size = new System.Drawing.Size(217, 195);
            this.pnlOptionsComboBoxesLabels.TabIndex = 0;
            // 
            // lblBufferInSeconds
            // 
            this.lblBufferInSeconds.AutoSize = true;
            this.lblBufferInSeconds.Location = new System.Drawing.Point(9, 169);
            this.lblBufferInSeconds.Name = "lblBufferInSeconds";
            this.lblBufferInSeconds.Size = new System.Drawing.Size(180, 18);
            this.lblBufferInSeconds.TabIndex = 34;
            this.lblBufferInSeconds.Text = "Device buffer (in seconds)";
            // 
            // lblFilterDevices
            // 
            this.lblFilterDevices.AutoSize = true;
            this.lblFilterDevices.Location = new System.Drawing.Point(9, 135);
            this.lblFilterDevices.Name = "lblFilterDevices";
            this.lblFilterDevices.Size = new System.Drawing.Size(94, 18);
            this.lblFilterDevices.TabIndex = 32;
            this.lblFilterDevices.Text = "Filter devices";
            // 
            // lblStreamFormat
            // 
            this.lblStreamFormat.AutoSize = true;
            this.lblStreamFormat.Location = new System.Drawing.Point(9, 69);
            this.lblStreamFormat.Name = "lblStreamFormat";
            this.lblStreamFormat.Size = new System.Drawing.Size(103, 18);
            this.lblStreamFormat.TabIndex = 28;
            this.lblStreamFormat.Text = "Stream format";
            // 
            // lblIpAddressUsed
            // 
            this.lblIpAddressUsed.AutoSize = true;
            this.lblIpAddressUsed.Location = new System.Drawing.Point(9, 4);
            this.lblIpAddressUsed.Name = "lblIpAddressUsed";
            this.lblIpAddressUsed.Size = new System.Drawing.Size(126, 18);
            this.lblIpAddressUsed.TabIndex = 19;
            this.lblIpAddressUsed.Text = "IP4 address used:";
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(9, 102);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(72, 18);
            this.lblLanguage.TabIndex = 31;
            this.lblLanguage.Text = "Language";
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(9, 36);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(126, 18);
            this.lblDevice.TabIndex = 21;
            this.lblDevice.Text = "Recording device:";
            // 
            // pnlOptionsComboBoxesRight
            // 
            this.pnlOptionsComboBoxesRight.AutoSize = true;
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbBufferInSeconds);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbFilterDevices);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbLanguage);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbIP4AddressUsed);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbRecordingDevice);
            this.pnlOptionsComboBoxesRight.Controls.Add(this.cmbStreamFormat);
            this.pnlOptionsComboBoxesRight.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlOptionsComboBoxesRight.Location = new System.Drawing.Point(226, 4);
            this.pnlOptionsComboBoxesRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlOptionsComboBoxesRight.Name = "pnlOptionsComboBoxesRight";
            this.pnlOptionsComboBoxesRight.Size = new System.Drawing.Size(448, 195);
            this.pnlOptionsComboBoxesRight.TabIndex = 1;
            // 
            // cmbBufferInSeconds
            // 
            this.cmbBufferInSeconds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBufferInSeconds.FormattingEnabled = true;
            this.cmbBufferInSeconds.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.cmbBufferInSeconds.Location = new System.Drawing.Point(3, 165);
            this.cmbBufferInSeconds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbBufferInSeconds.Name = "cmbBufferInSeconds";
            this.cmbBufferInSeconds.Size = new System.Drawing.Size(442, 26);
            this.cmbBufferInSeconds.TabIndex = 34;
            // 
            // cmbFilterDevices
            // 
            this.cmbFilterDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterDevices.FormattingEnabled = true;
            this.cmbFilterDevices.Location = new System.Drawing.Point(3, 131);
            this.cmbFilterDevices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbFilterDevices.Name = "cmbFilterDevices";
            this.cmbFilterDevices.Size = new System.Drawing.Size(442, 26);
            this.cmbFilterDevices.TabIndex = 33;
            this.cmbFilterDevices.SelectedIndexChanged += new System.EventHandler(this.CmbFilterDevices_SelectedIndexChanged);
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(3, 98);
            this.cmbLanguage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(442, 26);
            this.cmbLanguage.TabIndex = 32;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.CmbLanguage_SelectedIndexChanged);
            // 
            // cmbIP4AddressUsed
            // 
            this.cmbIP4AddressUsed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIP4AddressUsed.FormattingEnabled = true;
            this.cmbIP4AddressUsed.Location = new System.Drawing.Point(3, 0);
            this.cmbIP4AddressUsed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbIP4AddressUsed.Name = "cmbIP4AddressUsed";
            this.cmbIP4AddressUsed.Size = new System.Drawing.Size(442, 26);
            this.cmbIP4AddressUsed.TabIndex = 20;
            // 
            // cmbRecordingDevice
            // 
            this.cmbRecordingDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordingDevice.FormattingEnabled = true;
            this.cmbRecordingDevice.Location = new System.Drawing.Point(3, 33);
            this.cmbRecordingDevice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbRecordingDevice.Name = "cmbRecordingDevice";
            this.cmbRecordingDevice.Size = new System.Drawing.Size(442, 26);
            this.cmbRecordingDevice.TabIndex = 22;
            // 
            // cmbStreamFormat
            // 
            this.cmbStreamFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStreamFormat.FormattingEnabled = true;
            this.cmbStreamFormat.Location = new System.Drawing.Point(3, 66);
            this.cmbStreamFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbStreamFormat.Name = "cmbStreamFormat";
            this.cmbStreamFormat.Size = new System.Drawing.Size(442, 26);
            this.cmbStreamFormat.TabIndex = 29;
            this.cmbStreamFormat.SelectedIndexChanged += new System.EventHandler(this.CmbStreamFormat_SelectedIndexChanged);
            // 
            // lblNewReleaseAvailable
            // 
            this.lblNewReleaseAvailable.AutoSize = true;
            this.lblNewReleaseAvailable.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblNewReleaseAvailable.Location = new System.Drawing.Point(10, 634);
            this.lblNewReleaseAvailable.Name = "lblNewReleaseAvailable";
            this.lblNewReleaseAvailable.Padding = new System.Windows.Forms.Padding(10, 4, 3, 4);
            this.lblNewReleaseAvailable.Size = new System.Drawing.Size(225, 26);
            this.lblNewReleaseAvailable.TabIndex = 40;
            this.lblNewReleaseAvailable.TabStop = true;
            this.lblNewReleaseAvailable.Text = "Version x is available on Github";
            this.lblNewReleaseAvailable.Visible = false;
            this.lblNewReleaseAvailable.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblNewReleaseAvailable_LinkClicked);
            // 
            // linkHelp
            // 
            this.linkHelp.AutoSize = true;
            this.linkHelp.BackColor = System.Drawing.Color.White;
            this.linkHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkHelp.Location = new System.Drawing.Point(22, 28);
            this.linkHelp.Name = "linkHelp";
            this.linkHelp.Size = new System.Drawing.Size(244, 18);
            this.linkHelp.TabIndex = 43;
            this.linkHelp.TabStop = true;
            this.linkHelp.Text = "Information about options on Github";
            this.linkHelp.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkHelp_LinkClicked);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblVersion.Location = new System.Drawing.Point(10, 660);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Padding = new System.Windows.Forms.Padding(10, 4, 3, 4);
            this.lblVersion.Size = new System.Drawing.Size(71, 26);
            this.lblVersion.TabIndex = 39;
            this.lblVersion.Text = "Version";
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.pnlLog);
            this.tabPageLog.Controls.Add(this.pnlPingPong);
            this.tabPageLog.Controls.Add(this.pnlLogCopyToClipboard);
            this.tabPageLog.Location = new System.Drawing.Point(4, 29);
            this.tabPageLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(20, 25, 20, 25);
            this.tabPageLog.Size = new System.Drawing.Size(1111, 748);
            this.tabPageLog.TabIndex = 0;
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // pnlLog
            // 
            this.pnlLog.Controls.Add(this.txtLog);
            this.pnlLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLog.Location = new System.Drawing.Point(20, 25);
            this.pnlLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Size = new System.Drawing.Size(1071, 628);
            this.pnlLog.TabIndex = 6;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Courier New", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1071, 628);
            this.txtLog.TabIndex = 2;
            // 
            // pnlPingPong
            // 
            this.pnlPingPong.AutoSize = true;
            this.pnlPingPong.Controls.Add(this.lblPingPong);
            this.pnlPingPong.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPingPong.Location = new System.Drawing.Point(20, 653);
            this.pnlPingPong.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlPingPong.Name = "pnlPingPong";
            this.pnlPingPong.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.pnlPingPong.Size = new System.Drawing.Size(1071, 32);
            this.pnlPingPong.TabIndex = 5;
            // 
            // lblPingPong
            // 
            this.lblPingPong.AutoSize = true;
            this.lblPingPong.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPingPong.Location = new System.Drawing.Point(5, 6);
            this.lblPingPong.Name = "lblPingPong";
            this.lblPingPong.Size = new System.Drawing.Size(149, 20);
            this.lblPingPong.TabIndex = 3;
            this.lblPingPong.Text = "                                   ";
            // 
            // pnlLogCopyToClipboard
            // 
            this.pnlLogCopyToClipboard.AutoSize = true;
            this.pnlLogCopyToClipboard.Controls.Add(this.btnClipboardCopy);
            this.pnlLogCopyToClipboard.Controls.Add(this.btnClearLog);
            this.pnlLogCopyToClipboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLogCopyToClipboard.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlLogCopyToClipboard.Location = new System.Drawing.Point(20, 685);
            this.pnlLogCopyToClipboard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlLogCopyToClipboard.Name = "pnlLogCopyToClipboard";
            this.pnlLogCopyToClipboard.Size = new System.Drawing.Size(1071, 38);
            this.pnlLogCopyToClipboard.TabIndex = 4;
            // 
            // btnClipboardCopy
            // 
            this.btnClipboardCopy.AutoSize = true;
            this.btnClipboardCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClipboardCopy.Location = new System.Drawing.Point(929, 4);
            this.btnClipboardCopy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClipboardCopy.Name = "btnClipboardCopy";
            this.btnClipboardCopy.Size = new System.Drawing.Size(139, 30);
            this.btnClipboardCopy.TabIndex = 3;
            this.btnClipboardCopy.Text = "Copy to clipboard";
            this.btnClipboardCopy.UseVisualStyleBackColor = true;
            this.btnClipboardCopy.Click += new System.EventHandler(this.BtnClipboardCopy_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.AutoSize = true;
            this.btnClearLog.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClearLog.Location = new System.Drawing.Point(841, 4);
            this.btnClearLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(82, 30);
            this.btnClearLog.TabIndex = 4;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.BtnClearLog_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 821);
            this.Controls.Add(this.tabControl);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(16, 20, 16, 20);
            this.Text = "Chromecast Desktop Audio Streamer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tabControl.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.grpDevices.ResumeLayout(false);
            this.grpLag.ResumeLayout(false);
            this.grpLag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).EndInit();
            this.grpVolume.ResumeLayout(false);
            this.grpVolume.PerformLayout();
            this.pnlVolumeMeter.ResumeLayout(false);
            this.pnlVolumeMeter.PerformLayout();
            this.pnlVolumeAllButtons.ResumeLayout(false);
            this.pnlVolumeAllButtons.PerformLayout();
            this.tabPageOptions.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlResetSettings.ResumeLayout(false);
            this.pnlResetSettings.PerformLayout();
            this.pnlOptionsCheckBoxes.ResumeLayout(false);
            this.pnlOptionsCheckBoxes.PerformLayout();
            this.pnlOptionsComboBoxes.ResumeLayout(false);
            this.pnlOptionsComboBoxes.PerformLayout();
            this.pnlOptionsComboBoxesLabels.ResumeLayout(false);
            this.pnlOptionsComboBoxesLabels.PerformLayout();
            this.pnlOptionsComboBoxesRight.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.pnlLog.ResumeLayout(false);
            this.pnlLog.PerformLayout();
            this.pnlPingPong.ResumeLayout(false);
            this.pnlPingPong.PerformLayout();
            this.pnlLogCopyToClipboard.ResumeLayout(false);
            this.pnlLogCopyToClipboard.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel pnlVolumeAllButtons;
        private System.Windows.Forms.FlowLayoutPanel pnlLogCopyToClipboard;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.GroupBox grpDevices;
        private System.Windows.Forms.FlowLayoutPanel pnlDevices;
        private System.Windows.Forms.Label lblLagExperimental;
        private System.Windows.Forms.Panel pnlLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel pnlPingPong;
        private System.Windows.Forms.Label lblPingPong;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel lblNewReleaseAvailable;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Panel pnlResetSettings;
        private System.Windows.Forms.Button btnResetSettings;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel pnlOptionsCheckBoxes;
        private System.Windows.Forms.CheckBox chkLogDeviceCommunication;
        private System.Windows.Forms.CheckBox chkShowLagControl;
        private System.Windows.Forms.CheckBox chkAutoRestart;
        private System.Windows.Forms.CheckBox chkAutoStartLastUsed;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkStartApplicationWhenWindowsStarts;
        private System.Windows.Forms.CheckBox chkShowWindowOnStart;
        private System.Windows.Forms.CheckBox chkHook;
        private System.Windows.Forms.FlowLayoutPanel pnlOptionsComboBoxes;
        private System.Windows.Forms.Panel pnlOptionsComboBoxesLabels;
        private System.Windows.Forms.Label lblFilterDevices;
        private System.Windows.Forms.Label lblStreamFormat;
        private System.Windows.Forms.Label lblIpAddressUsed;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.Panel pnlOptionsComboBoxesRight;
        private System.Windows.Forms.ComboBox cmbFilterDevices;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.ComboBox cmbIP4AddressUsed;
        private System.Windows.Forms.ComboBox cmbRecordingDevice;
        private System.Windows.Forms.ComboBox cmbStreamFormat;
        private System.Windows.Forms.LinkLabel linkHelp;
        private System.Windows.Forms.FlowLayoutPanel pnlVolumeMeter;
        private NAudio.Gui.VolumeMeter volumeMeter;
        private System.Windows.Forms.ToolTip volumeMeterTooltip;
        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.Label lblBufferInSeconds;
        private System.Windows.Forms.ComboBox cmbBufferInSeconds;
        private System.Windows.Forms.CheckBox chkAutoMute;
        private System.Windows.Forms.CheckBox chkMinimizeToTray;
        private System.Windows.Forms.CheckBox chkConvertMultiChannelToStereo;
        private System.Windows.Forms.CheckBox chkDarkMode;
    }
}

