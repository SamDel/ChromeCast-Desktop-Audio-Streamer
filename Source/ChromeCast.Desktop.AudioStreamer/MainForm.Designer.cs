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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControl = new System.Windows.Forms.TabControl();
            tabPageMain = new System.Windows.Forms.TabPage();
            grpDevices = new System.Windows.Forms.GroupBox();
            pnlDevices = new System.Windows.Forms.FlowLayoutPanel();
            grpLag = new System.Windows.Forms.GroupBox();
            lblLagExperimental = new System.Windows.Forms.Label();
            lblLagMin = new System.Windows.Forms.Label();
            lblLagMax = new System.Windows.Forms.Label();
            trbLag = new System.Windows.Forms.TrackBar();
            grpVolume = new System.Windows.Forms.GroupBox();
            pnlVolumeMeter = new System.Windows.Forms.FlowLayoutPanel();
            lblDb = new System.Windows.Forms.Label();
            volumeMeter = new NAudio.Gui.VolumeMeter();
            pnlVolumeAllButtons = new System.Windows.Forms.FlowLayoutPanel();
            btnVolumeUp = new System.Windows.Forms.Button();
            btnVolumeDown = new System.Windows.Forms.Button();
            btnVolumeMute = new System.Windows.Forms.Button();
            btnScan = new System.Windows.Forms.Button();
            tabPageOptions = new System.Windows.Forms.TabPage();
            grpOptions = new System.Windows.Forms.GroupBox();
            pnlOptions = new System.Windows.Forms.Panel();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            pnlResetSettings = new System.Windows.Forms.Panel();
            btnResetSettings = new System.Windows.Forms.Button();
            pnlOptionsCheckBoxes = new System.Windows.Forms.Panel();
            chkDarkMode = new System.Windows.Forms.CheckBox();
            chkConvertMultiChannelToStereo = new System.Windows.Forms.CheckBox();
            chkMinimizeToTray = new System.Windows.Forms.CheckBox();
            chkAutoMute = new System.Windows.Forms.CheckBox();
            chkLogDeviceCommunication = new System.Windows.Forms.CheckBox();
            chkShowLagControl = new System.Windows.Forms.CheckBox();
            chkAutoRestart = new System.Windows.Forms.CheckBox();
            chkAutoStartLastUsed = new System.Windows.Forms.CheckBox();
            chkAutoStart = new System.Windows.Forms.CheckBox();
            chkStartApplicationWhenWindowsStarts = new System.Windows.Forms.CheckBox();
            chkShowWindowOnStart = new System.Windows.Forms.CheckBox();
            chkHook = new System.Windows.Forms.CheckBox();
            pnlOptionsComboBoxes = new System.Windows.Forms.FlowLayoutPanel();
            pnlOptionsComboBoxesLabels = new System.Windows.Forms.Panel();
            lblStreamTitle = new System.Windows.Forms.Label();
            lblBufferInSeconds = new System.Windows.Forms.Label();
            lblFilterDevices = new System.Windows.Forms.Label();
            lblStreamFormat = new System.Windows.Forms.Label();
            lblIpAddressUsed = new System.Windows.Forms.Label();
            lblLanguage = new System.Windows.Forms.Label();
            lblDevice = new System.Windows.Forms.Label();
            pnlOptionsComboBoxesRight = new System.Windows.Forms.Panel();
            txtStreamTitle = new System.Windows.Forms.TextBox();
            cmbBufferInSeconds = new System.Windows.Forms.ComboBox();
            cmbFilterDevices = new System.Windows.Forms.ComboBox();
            cmbLanguage = new System.Windows.Forms.ComboBox();
            cmbIP4AddressUsed = new System.Windows.Forms.ComboBox();
            cmbRecordingDevice = new System.Windows.Forms.ComboBox();
            cmbStreamFormat = new System.Windows.Forms.ComboBox();
            lblNewReleaseAvailable = new System.Windows.Forms.LinkLabel();
            linkHelp = new System.Windows.Forms.LinkLabel();
            lblVersion = new System.Windows.Forms.Label();
            tabPageLog = new System.Windows.Forms.TabPage();
            pnlLog = new System.Windows.Forms.Panel();
            txtLog = new System.Windows.Forms.TextBox();
            pnlPingPong = new System.Windows.Forms.Panel();
            lblPingPong = new System.Windows.Forms.Label();
            pnlLogCopyToClipboard = new System.Windows.Forms.FlowLayoutPanel();
            btnClipboardCopy = new System.Windows.Forms.Button();
            btnClearLog = new System.Windows.Forms.Button();
            volumeMeterTooltip = new System.Windows.Forms.ToolTip(components);
            tabControl.SuspendLayout();
            tabPageMain.SuspendLayout();
            grpDevices.SuspendLayout();
            grpLag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trbLag).BeginInit();
            grpVolume.SuspendLayout();
            pnlVolumeMeter.SuspendLayout();
            pnlVolumeAllButtons.SuspendLayout();
            tabPageOptions.SuspendLayout();
            grpOptions.SuspendLayout();
            pnlOptions.SuspendLayout();
            pnlResetSettings.SuspendLayout();
            pnlOptionsCheckBoxes.SuspendLayout();
            pnlOptionsComboBoxes.SuspendLayout();
            pnlOptionsComboBoxesLabels.SuspendLayout();
            pnlOptionsComboBoxesRight.SuspendLayout();
            tabPageLog.SuspendLayout();
            pnlLog.SuspendLayout();
            pnlPingPong.SuspendLayout();
            pnlLogCopyToClipboard.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageMain);
            tabControl.Controls.Add(tabPageOptions);
            tabControl.Controls.Add(tabPageLog);
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Location = new System.Drawing.Point(16, 20);
            tabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(1119, 781);
            tabControl.TabIndex = 3;
            // 
            // tabPageMain
            // 
            tabPageMain.Controls.Add(grpDevices);
            tabPageMain.Controls.Add(grpLag);
            tabPageMain.Controls.Add(grpVolume);
            tabPageMain.Location = new System.Drawing.Point(4, 29);
            tabPageMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabPageMain.Name = "tabPageMain";
            tabPageMain.Padding = new System.Windows.Forms.Padding(20, 25, 20, 25);
            tabPageMain.Size = new System.Drawing.Size(1111, 748);
            tabPageMain.TabIndex = 1;
            tabPageMain.UseVisualStyleBackColor = true;
            // 
            // grpDevices
            // 
            grpDevices.Controls.Add(pnlDevices);
            grpDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            grpDevices.Location = new System.Drawing.Point(20, 250);
            grpDevices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpDevices.Name = "grpDevices";
            grpDevices.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            grpDevices.Size = new System.Drawing.Size(1071, 473);
            grpDevices.TabIndex = 13;
            grpDevices.TabStop = false;
            grpDevices.Text = "Devices (click name to start streaming)";
            // 
            // pnlDevices
            // 
            pnlDevices.AutoScroll = true;
            pnlDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlDevices.Location = new System.Drawing.Point(10, 32);
            pnlDevices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlDevices.Name = "pnlDevices";
            pnlDevices.Size = new System.Drawing.Size(1051, 429);
            pnlDevices.TabIndex = 10;
            // 
            // grpLag
            // 
            grpLag.Controls.Add(lblLagExperimental);
            grpLag.Controls.Add(lblLagMin);
            grpLag.Controls.Add(lblLagMax);
            grpLag.Controls.Add(trbLag);
            grpLag.Dock = System.Windows.Forms.DockStyle.Top;
            grpLag.Location = new System.Drawing.Point(20, 109);
            grpLag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpLag.Name = "grpLag";
            grpLag.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            grpLag.Size = new System.Drawing.Size(1071, 141);
            grpLag.TabIndex = 12;
            grpLag.TabStop = false;
            grpLag.Text = "Lag Control";
            grpLag.Visible = false;
            // 
            // lblLagExperimental
            // 
            lblLagExperimental.BackColor = System.Drawing.SystemColors.Control;
            lblLagExperimental.Location = new System.Drawing.Point(23, 104);
            lblLagExperimental.Name = "lblLagExperimental";
            lblLagExperimental.Size = new System.Drawing.Size(726, 21);
            lblLagExperimental.TabIndex = 4;
            lblLagExperimental.Text = "Experimental feature: Try to keep the buffer on the device as small as possible without hearing gaps.";
            lblLagExperimental.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLagMin
            // 
            lblLagMin.AutoSize = true;
            lblLagMin.Dock = System.Windows.Forms.DockStyle.Left;
            lblLagMin.Location = new System.Drawing.Point(20, 32);
            lblLagMin.Name = "lblLagMin";
            lblLagMin.Size = new System.Drawing.Size(192, 20);
            lblLagMin.TabIndex = 2;
            lblLagMin.Text = "minimum lag / poor quality";
            // 
            // lblLagMax
            // 
            lblLagMax.AutoSize = true;
            lblLagMax.Dock = System.Windows.Forms.DockStyle.Right;
            lblLagMax.Location = new System.Drawing.Point(860, 32);
            lblLagMax.Name = "lblLagMax";
            lblLagMax.Size = new System.Drawing.Size(191, 20);
            lblLagMax.TabIndex = 1;
            lblLagMax.Text = "maximum lag / best quality";
            lblLagMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // trbLag
            // 
            trbLag.Dock = System.Windows.Forms.DockStyle.Bottom;
            trbLag.LargeChange = 10;
            trbLag.Location = new System.Drawing.Point(20, 73);
            trbLag.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            trbLag.Maximum = 1000;
            trbLag.Minimum = 1;
            trbLag.Name = "trbLag";
            trbLag.Size = new System.Drawing.Size(1031, 56);
            trbLag.SmallChange = 5;
            trbLag.TabIndex = 0;
            trbLag.TickFrequency = 100;
            trbLag.Value = 1000;
            trbLag.Scroll += TrbLag_Scroll;
            // 
            // grpVolume
            // 
            grpVolume.Controls.Add(pnlVolumeMeter);
            grpVolume.Controls.Add(pnlVolumeAllButtons);
            grpVolume.Dock = System.Windows.Forms.DockStyle.Top;
            grpVolume.Location = new System.Drawing.Point(20, 25);
            grpVolume.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpVolume.Name = "grpVolume";
            grpVolume.Padding = new System.Windows.Forms.Padding(10, 2, 2, 4);
            grpVolume.Size = new System.Drawing.Size(1071, 84);
            grpVolume.TabIndex = 10;
            grpVolume.TabStop = false;
            grpVolume.Text = "Volume all devices:";
            // 
            // pnlVolumeMeter
            // 
            pnlVolumeMeter.AutoSize = true;
            pnlVolumeMeter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            pnlVolumeMeter.Controls.Add(lblDb);
            pnlVolumeMeter.Controls.Add(volumeMeter);
            pnlVolumeMeter.Dock = System.Windows.Forms.DockStyle.Right;
            pnlVolumeMeter.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            pnlVolumeMeter.Location = new System.Drawing.Point(1021, 22);
            pnlVolumeMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlVolumeMeter.Name = "pnlVolumeMeter";
            pnlVolumeMeter.Size = new System.Drawing.Size(48, 58);
            pnlVolumeMeter.TabIndex = 18;
            pnlVolumeMeter.WrapContents = false;
            // 
            // lblDb
            // 
            lblDb.AutoSize = true;
            lblDb.Location = new System.Drawing.Point(18, 0);
            lblDb.Name = "lblDb";
            lblDb.Size = new System.Drawing.Size(27, 20);
            lblDb.TabIndex = 20;
            lblDb.Text = "dB";
            // 
            // volumeMeter
            // 
            volumeMeter.Amplitude = 0F;
            volumeMeter.BackColor = System.Drawing.SystemColors.Window;
            volumeMeter.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            volumeMeter.Location = new System.Drawing.Point(3, 6);
            volumeMeter.Margin = new System.Windows.Forms.Padding(3, 6, 3, 4);
            volumeMeter.MaxDb = 18F;
            volumeMeter.MinDb = -60F;
            volumeMeter.Name = "volumeMeter";
            volumeMeter.Size = new System.Drawing.Size(9, 50);
            volumeMeter.TabIndex = 19;
            // 
            // pnlVolumeAllButtons
            // 
            pnlVolumeAllButtons.AutoSize = true;
            pnlVolumeAllButtons.Controls.Add(btnVolumeUp);
            pnlVolumeAllButtons.Controls.Add(btnVolumeDown);
            pnlVolumeAllButtons.Controls.Add(btnVolumeMute);
            pnlVolumeAllButtons.Controls.Add(btnScan);
            pnlVolumeAllButtons.Dock = System.Windows.Forms.DockStyle.Left;
            pnlVolumeAllButtons.Location = new System.Drawing.Point(10, 22);
            pnlVolumeAllButtons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlVolumeAllButtons.Name = "pnlVolumeAllButtons";
            pnlVolumeAllButtons.Size = new System.Drawing.Size(551, 58);
            pnlVolumeAllButtons.TabIndex = 17;
            pnlVolumeAllButtons.WrapContents = false;
            // 
            // btnVolumeUp
            // 
            btnVolumeUp.AutoSize = true;
            btnVolumeUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnVolumeUp.Location = new System.Drawing.Point(3, 4);
            btnVolumeUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnVolumeUp.MinimumSize = new System.Drawing.Size(120, 0);
            btnVolumeUp.Name = "btnVolumeUp";
            btnVolumeUp.Size = new System.Drawing.Size(120, 30);
            btnVolumeUp.TabIndex = 15;
            btnVolumeUp.Text = "Up";
            btnVolumeUp.UseVisualStyleBackColor = true;
            btnVolumeUp.Click += BtnVolumeUp_Click;
            // 
            // btnVolumeDown
            // 
            btnVolumeDown.AutoSize = true;
            btnVolumeDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnVolumeDown.Location = new System.Drawing.Point(129, 4);
            btnVolumeDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnVolumeDown.MinimumSize = new System.Drawing.Size(120, 0);
            btnVolumeDown.Name = "btnVolumeDown";
            btnVolumeDown.Size = new System.Drawing.Size(120, 30);
            btnVolumeDown.TabIndex = 14;
            btnVolumeDown.Text = "Down";
            btnVolumeDown.UseVisualStyleBackColor = true;
            btnVolumeDown.Click += BtnVolumeDown_Click;
            // 
            // btnVolumeMute
            // 
            btnVolumeMute.AutoSize = true;
            btnVolumeMute.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnVolumeMute.Location = new System.Drawing.Point(255, 4);
            btnVolumeMute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnVolumeMute.MinimumSize = new System.Drawing.Size(120, 0);
            btnVolumeMute.Name = "btnVolumeMute";
            btnVolumeMute.Size = new System.Drawing.Size(120, 30);
            btnVolumeMute.TabIndex = 11;
            btnVolumeMute.Text = "Mute";
            btnVolumeMute.UseVisualStyleBackColor = true;
            btnVolumeMute.Click += BtnVolumeMute_Click;
            // 
            // btnScan
            // 
            btnScan.AutoSize = true;
            btnScan.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnScan.Location = new System.Drawing.Point(381, 4);
            btnScan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnScan.Name = "btnScan";
            btnScan.Size = new System.Drawing.Size(167, 30);
            btnScan.TabIndex = 11;
            btnScan.Text = "Scan again for devices";
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += BtnScan_Click;
            // 
            // tabPageOptions
            // 
            tabPageOptions.Controls.Add(grpOptions);
            tabPageOptions.Location = new System.Drawing.Point(4, 29);
            tabPageOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabPageOptions.Name = "tabPageOptions";
            tabPageOptions.Padding = new System.Windows.Forms.Padding(20, 25, 20, 25);
            tabPageOptions.Size = new System.Drawing.Size(1111, 748);
            tabPageOptions.TabIndex = 2;
            tabPageOptions.Text = "Options";
            tabPageOptions.UseVisualStyleBackColor = true;
            // 
            // grpOptions
            // 
            grpOptions.Controls.Add(pnlOptions);
            grpOptions.Controls.Add(lblNewReleaseAvailable);
            grpOptions.Controls.Add(linkHelp);
            grpOptions.Controls.Add(lblVersion);
            grpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            grpOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            grpOptions.Location = new System.Drawing.Point(20, 25);
            grpOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            grpOptions.Name = "grpOptions";
            grpOptions.Padding = new System.Windows.Forms.Padding(10, 50, 10, 12);
            grpOptions.Size = new System.Drawing.Size(1071, 698);
            grpOptions.TabIndex = 14;
            grpOptions.TabStop = false;
            grpOptions.Text = "Options";
            // 
            // pnlOptions
            // 
            pnlOptions.AutoScroll = true;
            pnlOptions.AutoSize = true;
            pnlOptions.Controls.Add(flowLayoutPanel1);
            pnlOptions.Controls.Add(pnlResetSettings);
            pnlOptions.Controls.Add(pnlOptionsCheckBoxes);
            pnlOptions.Controls.Add(pnlOptionsComboBoxes);
            pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlOptions.Location = new System.Drawing.Point(10, 67);
            pnlOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlOptions.Name = "pnlOptions";
            pnlOptions.Size = new System.Drawing.Size(1051, 567);
            pnlOptions.TabIndex = 41;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 547);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(1030, 10);
            flowLayoutPanel1.TabIndex = 43;
            // 
            // pnlResetSettings
            // 
            pnlResetSettings.Controls.Add(btnResetSettings);
            pnlResetSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlResetSettings.Location = new System.Drawing.Point(0, 557);
            pnlResetSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlResetSettings.Name = "pnlResetSettings";
            pnlResetSettings.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            pnlResetSettings.Size = new System.Drawing.Size(1030, 71);
            pnlResetSettings.TabIndex = 36;
            // 
            // btnResetSettings
            // 
            btnResetSettings.AutoSize = true;
            btnResetSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnResetSettings.Location = new System.Drawing.Point(15, 4);
            btnResetSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnResetSettings.Name = "btnResetSettings";
            btnResetSettings.Size = new System.Drawing.Size(114, 28);
            btnResetSettings.TabIndex = 46;
            btnResetSettings.Text = "Reset Settings";
            btnResetSettings.UseVisualStyleBackColor = true;
            btnResetSettings.Click += BtnResetSettings_Click;
            // 
            // pnlOptionsCheckBoxes
            // 
            pnlOptionsCheckBoxes.AutoSize = true;
            pnlOptionsCheckBoxes.Controls.Add(chkDarkMode);
            pnlOptionsCheckBoxes.Controls.Add(chkConvertMultiChannelToStereo);
            pnlOptionsCheckBoxes.Controls.Add(chkMinimizeToTray);
            pnlOptionsCheckBoxes.Controls.Add(chkAutoMute);
            pnlOptionsCheckBoxes.Controls.Add(chkLogDeviceCommunication);
            pnlOptionsCheckBoxes.Controls.Add(chkShowLagControl);
            pnlOptionsCheckBoxes.Controls.Add(chkAutoRestart);
            pnlOptionsCheckBoxes.Controls.Add(chkAutoStartLastUsed);
            pnlOptionsCheckBoxes.Controls.Add(chkAutoStart);
            pnlOptionsCheckBoxes.Controls.Add(chkStartApplicationWhenWindowsStarts);
            pnlOptionsCheckBoxes.Controls.Add(chkShowWindowOnStart);
            pnlOptionsCheckBoxes.Controls.Add(chkHook);
            pnlOptionsCheckBoxes.Dock = System.Windows.Forms.DockStyle.Top;
            pnlOptionsCheckBoxes.Location = new System.Drawing.Point(0, 233);
            pnlOptionsCheckBoxes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlOptionsCheckBoxes.Name = "pnlOptionsCheckBoxes";
            pnlOptionsCheckBoxes.Padding = new System.Windows.Forms.Padding(15, 25, 20, 25);
            pnlOptionsCheckBoxes.Size = new System.Drawing.Size(1030, 314);
            pnlOptionsCheckBoxes.TabIndex = 35;
            // 
            // chkDarkMode
            // 
            chkDarkMode.AutoSize = true;
            chkDarkMode.Dock = System.Windows.Forms.DockStyle.Top;
            chkDarkMode.Location = new System.Drawing.Point(15, 267);
            chkDarkMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkDarkMode.Name = "chkDarkMode";
            chkDarkMode.Size = new System.Drawing.Size(995, 22);
            chkDarkMode.TabIndex = 47;
            chkDarkMode.Text = "Dark mode";
            chkDarkMode.UseVisualStyleBackColor = true;
            chkDarkMode.CheckedChanged += chkDarkMode_CheckedChanged;
            // 
            // chkConvertMultiChannelToStereo
            // 
            chkConvertMultiChannelToStereo.AutoSize = true;
            chkConvertMultiChannelToStereo.Dock = System.Windows.Forms.DockStyle.Top;
            chkConvertMultiChannelToStereo.Location = new System.Drawing.Point(15, 245);
            chkConvertMultiChannelToStereo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkConvertMultiChannelToStereo.Name = "chkConvertMultiChannelToStereo";
            chkConvertMultiChannelToStereo.Size = new System.Drawing.Size(995, 22);
            chkConvertMultiChannelToStereo.TabIndex = 46;
            chkConvertMultiChannelToStereo.Text = "Convert multi-channel audio to stereo output";
            chkConvertMultiChannelToStereo.UseVisualStyleBackColor = true;
            // 
            // chkMinimizeToTray
            // 
            chkMinimizeToTray.AutoSize = true;
            chkMinimizeToTray.Dock = System.Windows.Forms.DockStyle.Top;
            chkMinimizeToTray.Location = new System.Drawing.Point(15, 223);
            chkMinimizeToTray.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkMinimizeToTray.Name = "chkMinimizeToTray";
            chkMinimizeToTray.Size = new System.Drawing.Size(995, 22);
            chkMinimizeToTray.TabIndex = 45;
            chkMinimizeToTray.Text = "Minimize to tray when closing";
            chkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // chkAutoMute
            // 
            chkAutoMute.AutoSize = true;
            chkAutoMute.Dock = System.Windows.Forms.DockStyle.Top;
            chkAutoMute.Location = new System.Drawing.Point(15, 201);
            chkAutoMute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkAutoMute.Name = "chkAutoMute";
            chkAutoMute.Size = new System.Drawing.Size(995, 22);
            chkAutoMute.TabIndex = 44;
            chkAutoMute.Text = "Auto-mute/unmute desktop audio";
            chkAutoMute.UseVisualStyleBackColor = true;
            chkAutoMute.CheckedChanged += ChkAutoMute_CheckedChanged;
            // 
            // chkLogDeviceCommunication
            // 
            chkLogDeviceCommunication.AutoSize = true;
            chkLogDeviceCommunication.Checked = true;
            chkLogDeviceCommunication.CheckState = System.Windows.Forms.CheckState.Checked;
            chkLogDeviceCommunication.Dock = System.Windows.Forms.DockStyle.Top;
            chkLogDeviceCommunication.Location = new System.Drawing.Point(15, 179);
            chkLogDeviceCommunication.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkLogDeviceCommunication.Name = "chkLogDeviceCommunication";
            chkLogDeviceCommunication.Size = new System.Drawing.Size(995, 22);
            chkLogDeviceCommunication.TabIndex = 43;
            chkLogDeviceCommunication.Text = "Log device communication";
            chkLogDeviceCommunication.UseVisualStyleBackColor = true;
            chkLogDeviceCommunication.CheckedChanged += ChkLogDeviceCommunication_CheckedChanged;
            // 
            // chkShowLagControl
            // 
            chkShowLagControl.AutoSize = true;
            chkShowLagControl.Dock = System.Windows.Forms.DockStyle.Top;
            chkShowLagControl.Location = new System.Drawing.Point(15, 157);
            chkShowLagControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkShowLagControl.Name = "chkShowLagControl";
            chkShowLagControl.Size = new System.Drawing.Size(995, 22);
            chkShowLagControl.TabIndex = 42;
            chkShowLagControl.Text = "Show lag control (experimental)";
            chkShowLagControl.UseVisualStyleBackColor = true;
            chkShowLagControl.Visible = false;
            chkShowLagControl.CheckedChanged += ChkShowLagControl_CheckedChanged;
            // 
            // chkAutoRestart
            // 
            chkAutoRestart.AutoSize = true;
            chkAutoRestart.Dock = System.Windows.Forms.DockStyle.Top;
            chkAutoRestart.Location = new System.Drawing.Point(15, 135);
            chkAutoRestart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkAutoRestart.Name = "chkAutoRestart";
            chkAutoRestart.Size = new System.Drawing.Size(995, 22);
            chkAutoRestart.TabIndex = 41;
            chkAutoRestart.Text = "Automatically restart when the stream is closed";
            chkAutoRestart.UseVisualStyleBackColor = true;
            chkAutoRestart.CheckedChanged += ChkAutoRestart_CheckedChanged;
            // 
            // chkAutoStartLastUsed
            // 
            chkAutoStartLastUsed.AutoSize = true;
            chkAutoStartLastUsed.Dock = System.Windows.Forms.DockStyle.Top;
            chkAutoStartLastUsed.Location = new System.Drawing.Point(15, 113);
            chkAutoStartLastUsed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkAutoStartLastUsed.Name = "chkAutoStartLastUsed";
            chkAutoStartLastUsed.Size = new System.Drawing.Size(995, 22);
            chkAutoStartLastUsed.TabIndex = 40;
            chkAutoStartLastUsed.Text = "Automatically start last used devices and groups at startup";
            chkAutoStartLastUsed.UseVisualStyleBackColor = true;
            // 
            // chkAutoStart
            // 
            chkAutoStart.AutoSize = true;
            chkAutoStart.Dock = System.Windows.Forms.DockStyle.Top;
            chkAutoStart.Location = new System.Drawing.Point(15, 91);
            chkAutoStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkAutoStart.Name = "chkAutoStart";
            chkAutoStart.Size = new System.Drawing.Size(995, 22);
            chkAutoStart.TabIndex = 39;
            chkAutoStart.Text = "Automatically start devices at startup";
            chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // chkStartApplicationWhenWindowsStarts
            // 
            chkStartApplicationWhenWindowsStarts.AutoSize = true;
            chkStartApplicationWhenWindowsStarts.Dock = System.Windows.Forms.DockStyle.Top;
            chkStartApplicationWhenWindowsStarts.Location = new System.Drawing.Point(15, 69);
            chkStartApplicationWhenWindowsStarts.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkStartApplicationWhenWindowsStarts.Name = "chkStartApplicationWhenWindowsStarts";
            chkStartApplicationWhenWindowsStarts.Size = new System.Drawing.Size(995, 22);
            chkStartApplicationWhenWindowsStarts.TabIndex = 38;
            chkStartApplicationWhenWindowsStarts.Text = "Start application when Windows starts";
            chkStartApplicationWhenWindowsStarts.UseVisualStyleBackColor = true;
            chkStartApplicationWhenWindowsStarts.CheckedChanged += ChkStartApplicationWhenWindowsStarts_CheckedChanged;
            // 
            // chkShowWindowOnStart
            // 
            chkShowWindowOnStart.AutoSize = true;
            chkShowWindowOnStart.Checked = true;
            chkShowWindowOnStart.CheckState = System.Windows.Forms.CheckState.Checked;
            chkShowWindowOnStart.Dock = System.Windows.Forms.DockStyle.Top;
            chkShowWindowOnStart.Location = new System.Drawing.Point(15, 47);
            chkShowWindowOnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkShowWindowOnStart.Name = "chkShowWindowOnStart";
            chkShowWindowOnStart.Size = new System.Drawing.Size(995, 22);
            chkShowWindowOnStart.TabIndex = 24;
            chkShowWindowOnStart.Text = "Show window at startup";
            chkShowWindowOnStart.UseVisualStyleBackColor = true;
            // 
            // chkHook
            // 
            chkHook.AutoSize = true;
            chkHook.Dock = System.Windows.Forms.DockStyle.Top;
            chkHook.Location = new System.Drawing.Point(15, 25);
            chkHook.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chkHook.Name = "chkHook";
            chkHook.Size = new System.Drawing.Size(995, 22);
            chkHook.TabIndex = 15;
            chkHook.Text = "Use Keyboard shortcuts: Up = Ctrl+Alt+U; Down = Ctrl+Alt+D; (Un)Mute = Ctrl+Alt+M";
            chkHook.UseVisualStyleBackColor = true;
            chkHook.CheckedChanged += ChkHook_CheckedChanged;
            // 
            // pnlOptionsComboBoxes
            // 
            pnlOptionsComboBoxes.AutoSize = true;
            pnlOptionsComboBoxes.BackColor = System.Drawing.Color.Transparent;
            pnlOptionsComboBoxes.Controls.Add(pnlOptionsComboBoxesLabels);
            pnlOptionsComboBoxes.Controls.Add(pnlOptionsComboBoxesRight);
            pnlOptionsComboBoxes.Dock = System.Windows.Forms.DockStyle.Top;
            pnlOptionsComboBoxes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            pnlOptionsComboBoxes.Location = new System.Drawing.Point(0, 0);
            pnlOptionsComboBoxes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlOptionsComboBoxes.Name = "pnlOptionsComboBoxes";
            pnlOptionsComboBoxes.Size = new System.Drawing.Size(1030, 233);
            pnlOptionsComboBoxes.TabIndex = 42;
            pnlOptionsComboBoxes.WrapContents = false;
            // 
            // pnlOptionsComboBoxesLabels
            // 
            pnlOptionsComboBoxesLabels.AutoSize = true;
            pnlOptionsComboBoxesLabels.Controls.Add(lblStreamTitle);
            pnlOptionsComboBoxesLabels.Controls.Add(lblBufferInSeconds);
            pnlOptionsComboBoxesLabels.Controls.Add(lblFilterDevices);
            pnlOptionsComboBoxesLabels.Controls.Add(lblStreamFormat);
            pnlOptionsComboBoxesLabels.Controls.Add(lblIpAddressUsed);
            pnlOptionsComboBoxesLabels.Controls.Add(lblLanguage);
            pnlOptionsComboBoxesLabels.Controls.Add(lblDevice);
            pnlOptionsComboBoxesLabels.Dock = System.Windows.Forms.DockStyle.Left;
            pnlOptionsComboBoxesLabels.Location = new System.Drawing.Point(3, 4);
            pnlOptionsComboBoxesLabels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlOptionsComboBoxesLabels.Name = "pnlOptionsComboBoxesLabels";
            pnlOptionsComboBoxesLabels.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
            pnlOptionsComboBoxesLabels.Size = new System.Drawing.Size(217, 225);
            pnlOptionsComboBoxesLabels.TabIndex = 0;
            // 
            // lblStreamTitle
            // 
            lblStreamTitle.AutoSize = true;
            lblStreamTitle.Location = new System.Drawing.Point(9, 198);
            lblStreamTitle.Name = "lblStreamTitle";
            lblStreamTitle.Size = new System.Drawing.Size(82, 18);
            lblStreamTitle.TabIndex = 35;
            lblStreamTitle.Text = "Stream title";
            // 
            // lblBufferInSeconds
            // 
            lblBufferInSeconds.AutoSize = true;
            lblBufferInSeconds.Location = new System.Drawing.Point(9, 168);
            lblBufferInSeconds.Name = "lblBufferInSeconds";
            lblBufferInSeconds.Size = new System.Drawing.Size(180, 18);
            lblBufferInSeconds.TabIndex = 34;
            lblBufferInSeconds.Text = "Device buffer (in seconds)";
            // 
            // lblFilterDevices
            // 
            lblFilterDevices.AutoSize = true;
            lblFilterDevices.Location = new System.Drawing.Point(9, 135);
            lblFilterDevices.Name = "lblFilterDevices";
            lblFilterDevices.Size = new System.Drawing.Size(94, 18);
            lblFilterDevices.TabIndex = 32;
            lblFilterDevices.Text = "Filter devices";
            // 
            // lblStreamFormat
            // 
            lblStreamFormat.AutoSize = true;
            lblStreamFormat.Location = new System.Drawing.Point(9, 69);
            lblStreamFormat.Name = "lblStreamFormat";
            lblStreamFormat.Size = new System.Drawing.Size(103, 18);
            lblStreamFormat.TabIndex = 28;
            lblStreamFormat.Text = "Stream format";
            // 
            // lblIpAddressUsed
            // 
            lblIpAddressUsed.AutoSize = true;
            lblIpAddressUsed.Location = new System.Drawing.Point(9, 4);
            lblIpAddressUsed.Name = "lblIpAddressUsed";
            lblIpAddressUsed.Size = new System.Drawing.Size(126, 18);
            lblIpAddressUsed.TabIndex = 19;
            lblIpAddressUsed.Text = "IP4 address used:";
            // 
            // lblLanguage
            // 
            lblLanguage.AutoSize = true;
            lblLanguage.Location = new System.Drawing.Point(9, 102);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Size = new System.Drawing.Size(72, 18);
            lblLanguage.TabIndex = 31;
            lblLanguage.Text = "Language";
            // 
            // lblDevice
            // 
            lblDevice.AutoSize = true;
            lblDevice.Location = new System.Drawing.Point(9, 36);
            lblDevice.Name = "lblDevice";
            lblDevice.Size = new System.Drawing.Size(126, 18);
            lblDevice.TabIndex = 21;
            lblDevice.Text = "Recording device:";
            // 
            // pnlOptionsComboBoxesRight
            // 
            pnlOptionsComboBoxesRight.AutoSize = true;
            pnlOptionsComboBoxesRight.Controls.Add(txtStreamTitle);
            pnlOptionsComboBoxesRight.Controls.Add(cmbBufferInSeconds);
            pnlOptionsComboBoxesRight.Controls.Add(cmbFilterDevices);
            pnlOptionsComboBoxesRight.Controls.Add(cmbLanguage);
            pnlOptionsComboBoxesRight.Controls.Add(cmbIP4AddressUsed);
            pnlOptionsComboBoxesRight.Controls.Add(cmbRecordingDevice);
            pnlOptionsComboBoxesRight.Controls.Add(cmbStreamFormat);
            pnlOptionsComboBoxesRight.Dock = System.Windows.Forms.DockStyle.Left;
            pnlOptionsComboBoxesRight.Location = new System.Drawing.Point(226, 4);
            pnlOptionsComboBoxesRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlOptionsComboBoxesRight.Name = "pnlOptionsComboBoxesRight";
            pnlOptionsComboBoxesRight.Size = new System.Drawing.Size(448, 225);
            pnlOptionsComboBoxesRight.TabIndex = 1;
            // 
            // txtStreamTitle
            // 
            txtStreamTitle.Location = new System.Drawing.Point(0, 198);
            txtStreamTitle.Name = "txtStreamTitle";
            txtStreamTitle.Size = new System.Drawing.Size(445, 24);
            txtStreamTitle.TabIndex = 49;
            // 
            // cmbBufferInSeconds
            // 
            cmbBufferInSeconds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbBufferInSeconds.FormattingEnabled = true;
            cmbBufferInSeconds.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" });
            cmbBufferInSeconds.Location = new System.Drawing.Point(3, 165);
            cmbBufferInSeconds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbBufferInSeconds.Name = "cmbBufferInSeconds";
            cmbBufferInSeconds.Size = new System.Drawing.Size(442, 26);
            cmbBufferInSeconds.TabIndex = 34;
            // 
            // cmbFilterDevices
            // 
            cmbFilterDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbFilterDevices.FormattingEnabled = true;
            cmbFilterDevices.Location = new System.Drawing.Point(3, 131);
            cmbFilterDevices.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbFilterDevices.Name = "cmbFilterDevices";
            cmbFilterDevices.Size = new System.Drawing.Size(442, 26);
            cmbFilterDevices.TabIndex = 33;
            cmbFilterDevices.SelectedIndexChanged += CmbFilterDevices_SelectedIndexChanged;
            // 
            // cmbLanguage
            // 
            cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbLanguage.FormattingEnabled = true;
            cmbLanguage.Location = new System.Drawing.Point(3, 98);
            cmbLanguage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new System.Drawing.Size(442, 26);
            cmbLanguage.TabIndex = 32;
            cmbLanguage.SelectedIndexChanged += CmbLanguage_SelectedIndexChanged;
            // 
            // cmbIP4AddressUsed
            // 
            cmbIP4AddressUsed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbIP4AddressUsed.FormattingEnabled = true;
            cmbIP4AddressUsed.Location = new System.Drawing.Point(3, 0);
            cmbIP4AddressUsed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbIP4AddressUsed.Name = "cmbIP4AddressUsed";
            cmbIP4AddressUsed.Size = new System.Drawing.Size(442, 26);
            cmbIP4AddressUsed.TabIndex = 20;
            // 
            // cmbRecordingDevice
            // 
            cmbRecordingDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbRecordingDevice.FormattingEnabled = true;
            cmbRecordingDevice.Location = new System.Drawing.Point(3, 33);
            cmbRecordingDevice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbRecordingDevice.Name = "cmbRecordingDevice";
            cmbRecordingDevice.Size = new System.Drawing.Size(442, 26);
            cmbRecordingDevice.TabIndex = 22;
            // 
            // cmbStreamFormat
            // 
            cmbStreamFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbStreamFormat.FormattingEnabled = true;
            cmbStreamFormat.Location = new System.Drawing.Point(3, 66);
            cmbStreamFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmbStreamFormat.Name = "cmbStreamFormat";
            cmbStreamFormat.Size = new System.Drawing.Size(442, 26);
            cmbStreamFormat.TabIndex = 29;
            cmbStreamFormat.SelectedIndexChanged += CmbStreamFormat_SelectedIndexChanged;
            // 
            // lblNewReleaseAvailable
            // 
            lblNewReleaseAvailable.AutoSize = true;
            lblNewReleaseAvailable.Dock = System.Windows.Forms.DockStyle.Bottom;
            lblNewReleaseAvailable.Location = new System.Drawing.Point(10, 634);
            lblNewReleaseAvailable.Name = "lblNewReleaseAvailable";
            lblNewReleaseAvailable.Padding = new System.Windows.Forms.Padding(10, 4, 3, 4);
            lblNewReleaseAvailable.Size = new System.Drawing.Size(225, 26);
            lblNewReleaseAvailable.TabIndex = 40;
            lblNewReleaseAvailable.TabStop = true;
            lblNewReleaseAvailable.Text = "Version x is available on Github";
            lblNewReleaseAvailable.Visible = false;
            lblNewReleaseAvailable.LinkClicked += LblNewReleaseAvailable_LinkClicked;
            // 
            // linkHelp
            // 
            linkHelp.AutoSize = true;
            linkHelp.BackColor = System.Drawing.Color.White;
            linkHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            linkHelp.Location = new System.Drawing.Point(22, 28);
            linkHelp.Name = "linkHelp";
            linkHelp.Size = new System.Drawing.Size(244, 18);
            linkHelp.TabIndex = 43;
            linkHelp.TabStop = true;
            linkHelp.Text = "Information about options on Github";
            linkHelp.TextAlign = System.Drawing.ContentAlignment.TopRight;
            linkHelp.LinkClicked += LinkHelp_LinkClicked;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
            lblVersion.Location = new System.Drawing.Point(10, 660);
            lblVersion.Name = "lblVersion";
            lblVersion.Padding = new System.Windows.Forms.Padding(10, 4, 3, 4);
            lblVersion.Size = new System.Drawing.Size(71, 26);
            lblVersion.TabIndex = 39;
            lblVersion.Text = "Version";
            // 
            // tabPageLog
            // 
            tabPageLog.Controls.Add(pnlLog);
            tabPageLog.Controls.Add(pnlPingPong);
            tabPageLog.Controls.Add(pnlLogCopyToClipboard);
            tabPageLog.Location = new System.Drawing.Point(4, 29);
            tabPageLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabPageLog.Name = "tabPageLog";
            tabPageLog.Padding = new System.Windows.Forms.Padding(20, 25, 20, 25);
            tabPageLog.Size = new System.Drawing.Size(1111, 748);
            tabPageLog.TabIndex = 0;
            tabPageLog.Text = "Log";
            tabPageLog.UseVisualStyleBackColor = true;
            // 
            // pnlLog
            // 
            pnlLog.Controls.Add(txtLog);
            pnlLog.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlLog.Location = new System.Drawing.Point(20, 25);
            pnlLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlLog.Name = "pnlLog";
            pnlLog.Size = new System.Drawing.Size(1071, 628);
            pnlLog.TabIndex = 6;
            // 
            // txtLog
            // 
            txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLog.Font = new System.Drawing.Font("Courier New", 10.2F);
            txtLog.Location = new System.Drawing.Point(0, 0);
            txtLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtLog.Size = new System.Drawing.Size(1071, 628);
            txtLog.TabIndex = 2;
            // 
            // pnlPingPong
            // 
            pnlPingPong.AutoSize = true;
            pnlPingPong.Controls.Add(lblPingPong);
            pnlPingPong.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlPingPong.Location = new System.Drawing.Point(20, 653);
            pnlPingPong.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlPingPong.Name = "pnlPingPong";
            pnlPingPong.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            pnlPingPong.Size = new System.Drawing.Size(1071, 32);
            pnlPingPong.TabIndex = 5;
            // 
            // lblPingPong
            // 
            lblPingPong.AutoSize = true;
            lblPingPong.Dock = System.Windows.Forms.DockStyle.Top;
            lblPingPong.Location = new System.Drawing.Point(5, 6);
            lblPingPong.Name = "lblPingPong";
            lblPingPong.Size = new System.Drawing.Size(149, 20);
            lblPingPong.TabIndex = 3;
            lblPingPong.Text = "                                   ";
            // 
            // pnlLogCopyToClipboard
            // 
            pnlLogCopyToClipboard.AutoSize = true;
            pnlLogCopyToClipboard.Controls.Add(btnClipboardCopy);
            pnlLogCopyToClipboard.Controls.Add(btnClearLog);
            pnlLogCopyToClipboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlLogCopyToClipboard.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            pnlLogCopyToClipboard.Location = new System.Drawing.Point(20, 685);
            pnlLogCopyToClipboard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlLogCopyToClipboard.Name = "pnlLogCopyToClipboard";
            pnlLogCopyToClipboard.Size = new System.Drawing.Size(1071, 38);
            pnlLogCopyToClipboard.TabIndex = 4;
            // 
            // btnClipboardCopy
            // 
            btnClipboardCopy.AutoSize = true;
            btnClipboardCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnClipboardCopy.Location = new System.Drawing.Point(929, 4);
            btnClipboardCopy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClipboardCopy.Name = "btnClipboardCopy";
            btnClipboardCopy.Size = new System.Drawing.Size(139, 30);
            btnClipboardCopy.TabIndex = 3;
            btnClipboardCopy.Text = "Copy to clipboard";
            btnClipboardCopy.UseVisualStyleBackColor = true;
            btnClipboardCopy.Click += BtnClipboardCopy_Click;
            // 
            // btnClearLog
            // 
            btnClearLog.AutoSize = true;
            btnClearLog.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnClearLog.Location = new System.Drawing.Point(841, 4);
            btnClearLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new System.Drawing.Size(82, 30);
            btnClearLog.TabIndex = 4;
            btnClearLog.Text = "Clear Log";
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Click += BtnClearLog_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1151, 821);
            Controls.Add(tabControl);
            HelpButton = true;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "MainForm";
            Padding = new System.Windows.Forms.Padding(16, 20, 16, 20);
            Text = "Chromecast Desktop Audio Streamer";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            tabControl.ResumeLayout(false);
            tabPageMain.ResumeLayout(false);
            grpDevices.ResumeLayout(false);
            grpLag.ResumeLayout(false);
            grpLag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trbLag).EndInit();
            grpVolume.ResumeLayout(false);
            grpVolume.PerformLayout();
            pnlVolumeMeter.ResumeLayout(false);
            pnlVolumeMeter.PerformLayout();
            pnlVolumeAllButtons.ResumeLayout(false);
            pnlVolumeAllButtons.PerformLayout();
            tabPageOptions.ResumeLayout(false);
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            pnlOptions.ResumeLayout(false);
            pnlOptions.PerformLayout();
            pnlResetSettings.ResumeLayout(false);
            pnlResetSettings.PerformLayout();
            pnlOptionsCheckBoxes.ResumeLayout(false);
            pnlOptionsCheckBoxes.PerformLayout();
            pnlOptionsComboBoxes.ResumeLayout(false);
            pnlOptionsComboBoxes.PerformLayout();
            pnlOptionsComboBoxesLabels.ResumeLayout(false);
            pnlOptionsComboBoxesLabels.PerformLayout();
            pnlOptionsComboBoxesRight.ResumeLayout(false);
            pnlOptionsComboBoxesRight.PerformLayout();
            tabPageLog.ResumeLayout(false);
            tabPageLog.PerformLayout();
            pnlLog.ResumeLayout(false);
            pnlLog.PerformLayout();
            pnlPingPong.ResumeLayout(false);
            pnlPingPong.PerformLayout();
            pnlLogCopyToClipboard.ResumeLayout(false);
            pnlLogCopyToClipboard.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.TextBox txtStreamTitle;
        private System.Windows.Forms.Label lblStreamTitle;
    }
}

