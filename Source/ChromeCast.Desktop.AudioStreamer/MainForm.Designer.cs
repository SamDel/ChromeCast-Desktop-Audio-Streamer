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
            this.lblLagExperimental = new System.Windows.Forms.Label();
            this.lblLagMin = new System.Windows.Forms.Label();
            this.lblLagMax = new System.Windows.Forms.Label();
            this.trbLag = new System.Windows.Forms.TrackBar();
            this.grpDevices = new System.Windows.Forms.GroupBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.pnlDevices = new System.Windows.Forms.FlowLayoutPanel();
            this.grpVolume = new System.Windows.Forms.GroupBox();
            this.btnSyncDevices = new System.Windows.Forms.Button();
            this.btnVolumeUp = new System.Windows.Forms.Button();
            this.btnVolumeDown = new System.Windows.Forms.Button();
            this.btnVolumeMute = new System.Windows.Forms.Button();
            this.tabPageOptions = new System.Windows.Forms.TabPage();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.lblStreamFormatExtra = new System.Windows.Forms.Label();
            this.cmbStreamFormat = new System.Windows.Forms.ComboBox();
            this.lblStreamFormat = new System.Windows.Forms.Label();
            this.chkShowLagControl = new System.Windows.Forms.CheckBox();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkShowWindowOnStart = new System.Windows.Forms.CheckBox();
            this.cmbRecordingDevice = new System.Windows.Forms.ComboBox();
            this.lblDevice = new System.Windows.Forms.Label();
            this.cmbIP4AddressUsed = new System.Windows.Forms.ComboBox();
            this.lblIpAddressUsed = new System.Windows.Forms.Label();
            this.chkAutoRestart = new System.Windows.Forms.CheckBox();
            this.chkHook = new System.Windows.Forms.CheckBox();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.btnClipboardCopy = new System.Windows.Forms.Button();
            this.labelPingPong = new System.Windows.Forms.Label();
            this.textLog = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.grpLag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).BeginInit();
            this.grpDevices.SuspendLayout();
            this.grpVolume.SuspendLayout();
            this.tabPageOptions.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageMain);
            this.tabControl.Controls.Add(this.tabPageOptions);
            this.tabControl.Controls.Add(this.tabPageLog);
            this.tabControl.Location = new System.Drawing.Point(25, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1100, 632);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.grpLag);
            this.tabPageMain.Controls.Add(this.grpDevices);
            this.tabPageMain.Controls.Add(this.grpVolume);
            this.tabPageMain.Location = new System.Drawing.Point(4, 25);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(1092, 603);
            this.tabPageMain.TabIndex = 1;
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // grpLag
            // 
            this.grpLag.Controls.Add(this.lblLagExperimental);
            this.grpLag.Controls.Add(this.lblLagMin);
            this.grpLag.Controls.Add(this.lblLagMax);
            this.grpLag.Controls.Add(this.trbLag);
            this.grpLag.Location = new System.Drawing.Point(44, 477);
            this.grpLag.Name = "grpLag";
            this.grpLag.Size = new System.Drawing.Size(1010, 113);
            this.grpLag.TabIndex = 12;
            this.grpLag.TabStop = false;
            this.grpLag.Text = "Lag Control";
            // 
            // lblLagExperimental
            // 
            this.lblLagExperimental.AutoSize = true;
            this.lblLagExperimental.Location = new System.Drawing.Point(346, 86);
            this.lblLagExperimental.Name = "lblLagExperimental";
            this.lblLagExperimental.Size = new System.Drawing.Size(641, 17);
            this.lblLagExperimental.TabIndex = 3;
            this.lblLagExperimental.Text = "Experimental feature: Try to keep the buffer on the device as small as possible w" +
    "ithout hearing gaps.";
            // 
            // lblLagMin
            // 
            this.lblLagMin.AutoSize = true;
            this.lblLagMin.Location = new System.Drawing.Point(29, 29);
            this.lblLagMin.Name = "lblLagMin";
            this.lblLagMin.Size = new System.Drawing.Size(172, 17);
            this.lblLagMin.TabIndex = 2;
            this.lblLagMin.Text = "minimum lag / poor quality";
            // 
            // lblLagMax
            // 
            this.lblLagMax.AutoSize = true;
            this.lblLagMax.Location = new System.Drawing.Point(818, 29);
            this.lblLagMax.Name = "lblLagMax";
            this.lblLagMax.Size = new System.Drawing.Size(173, 17);
            this.lblLagMax.TabIndex = 1;
            this.lblLagMax.Text = "maximum lag / best quality";
            // 
            // trbLag
            // 
            this.trbLag.LargeChange = 10;
            this.trbLag.Location = new System.Drawing.Point(22, 47);
            this.trbLag.Maximum = 1000;
            this.trbLag.Minimum = 1;
            this.trbLag.Name = "trbLag";
            this.trbLag.Size = new System.Drawing.Size(969, 56);
            this.trbLag.SmallChange = 5;
            this.trbLag.TabIndex = 0;
            this.trbLag.TickFrequency = 100;
            this.trbLag.Value = 1000;
            this.trbLag.Scroll += new System.EventHandler(this.trbLag_Scroll);
            // 
            // grpDevices
            // 
            this.grpDevices.Controls.Add(this.btnScan);
            this.grpDevices.Controls.Add(this.pnlDevices);
            this.grpDevices.Location = new System.Drawing.Point(44, 124);
            this.grpDevices.Name = "grpDevices";
            this.grpDevices.Size = new System.Drawing.Size(1010, 347);
            this.grpDevices.TabIndex = 11;
            this.grpDevices.TabStop = false;
            this.grpDevices.Text = "Devices (click name to start streaming)";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(781, 320);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(206, 27);
            this.btnScan.TabIndex = 11;
            this.btnScan.Text = "Scan again for devices";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // pnlDevices
            // 
            this.pnlDevices.AutoScroll = true;
            this.pnlDevices.Location = new System.Drawing.Point(22, 34);
            this.pnlDevices.Name = "pnlDevices";
            this.pnlDevices.Size = new System.Drawing.Size(969, 290);
            this.pnlDevices.TabIndex = 10;
            // 
            // grpVolume
            // 
            this.grpVolume.Controls.Add(this.btnSyncDevices);
            this.grpVolume.Controls.Add(this.btnVolumeUp);
            this.grpVolume.Controls.Add(this.btnVolumeDown);
            this.grpVolume.Controls.Add(this.btnVolumeMute);
            this.grpVolume.Location = new System.Drawing.Point(44, 10);
            this.grpVolume.Name = "grpVolume";
            this.grpVolume.Size = new System.Drawing.Size(1010, 108);
            this.grpVolume.TabIndex = 10;
            this.grpVolume.TabStop = false;
            this.grpVolume.Text = "Volume all devices:";
            // 
            // btnSyncDevices
            // 
            this.btnSyncDevices.Location = new System.Drawing.Point(280, 36);
            this.btnSyncDevices.Name = "btnSyncDevices";
            this.btnSyncDevices.Size = new System.Drawing.Size(121, 33);
            this.btnSyncDevices.TabIndex = 16;
            this.btnSyncDevices.Text = "Sync Devices";
            this.btnSyncDevices.UseVisualStyleBackColor = true;
            this.btnSyncDevices.Click += new System.EventHandler(this.btnSyncDevices_Click);
            // 
            // btnVolumeUp
            // 
            this.btnVolumeUp.Location = new System.Drawing.Point(22, 36);
            this.btnVolumeUp.Name = "btnVolumeUp";
            this.btnVolumeUp.Size = new System.Drawing.Size(80, 33);
            this.btnVolumeUp.TabIndex = 15;
            this.btnVolumeUp.Text = "Up";
            this.btnVolumeUp.UseVisualStyleBackColor = true;
            this.btnVolumeUp.Click += new System.EventHandler(this.btnVolumeUp_Click);
            // 
            // btnVolumeDown
            // 
            this.btnVolumeDown.Location = new System.Drawing.Point(108, 36);
            this.btnVolumeDown.Name = "btnVolumeDown";
            this.btnVolumeDown.Size = new System.Drawing.Size(80, 33);
            this.btnVolumeDown.TabIndex = 14;
            this.btnVolumeDown.Text = "Down";
            this.btnVolumeDown.UseVisualStyleBackColor = true;
            this.btnVolumeDown.Click += new System.EventHandler(this.btnVolumeDown_Click);
            // 
            // btnVolumeMute
            // 
            this.btnVolumeMute.Location = new System.Drawing.Point(194, 36);
            this.btnVolumeMute.Name = "btnVolumeMute";
            this.btnVolumeMute.Size = new System.Drawing.Size(80, 33);
            this.btnVolumeMute.TabIndex = 11;
            this.btnVolumeMute.Text = "Mute";
            this.btnVolumeMute.UseVisualStyleBackColor = true;
            this.btnVolumeMute.Click += new System.EventHandler(this.btnVolumeMute_Click);
            // 
            // tabPageOptions
            // 
            this.tabPageOptions.Controls.Add(this.grpOptions);
            this.tabPageOptions.Location = new System.Drawing.Point(4, 25);
            this.tabPageOptions.Name = "tabPageOptions";
            this.tabPageOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOptions.Size = new System.Drawing.Size(1092, 603);
            this.tabPageOptions.TabIndex = 2;
            this.tabPageOptions.Text = "Options";
            this.tabPageOptions.UseVisualStyleBackColor = true;
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.lblStreamFormatExtra);
            this.grpOptions.Controls.Add(this.cmbStreamFormat);
            this.grpOptions.Controls.Add(this.lblStreamFormat);
            this.grpOptions.Controls.Add(this.chkShowLagControl);
            this.grpOptions.Controls.Add(this.btnResetSettings);
            this.grpOptions.Controls.Add(this.lblVersion);
            this.grpOptions.Controls.Add(this.chkAutoStart);
            this.grpOptions.Controls.Add(this.chkShowWindowOnStart);
            this.grpOptions.Controls.Add(this.cmbRecordingDevice);
            this.grpOptions.Controls.Add(this.lblDevice);
            this.grpOptions.Controls.Add(this.cmbIP4AddressUsed);
            this.grpOptions.Controls.Add(this.lblIpAddressUsed);
            this.grpOptions.Controls.Add(this.chkAutoRestart);
            this.grpOptions.Controls.Add(this.chkHook);
            this.grpOptions.Location = new System.Drawing.Point(27, 22);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(1038, 553);
            this.grpOptions.TabIndex = 14;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // lblStreamFormatExtra
            // 
            this.lblStreamFormatExtra.AutoSize = true;
            this.lblStreamFormatExtra.Location = new System.Drawing.Point(262, 83);
            this.lblStreamFormatExtra.Name = "lblStreamFormatExtra";
            this.lblStreamFormatExtra.Size = new System.Drawing.Size(338, 17);
            this.lblStreamFormatExtra.TabIndex = 30;
            this.lblStreamFormatExtra.Text = "The mp3 formats have a long lag and buffering time.";
            // 
            // cmbStreamFormat
            // 
            this.cmbStreamFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStreamFormat.FormattingEnabled = true;
            this.cmbStreamFormat.Location = new System.Drawing.Point(158, 80);
            this.cmbStreamFormat.Name = "cmbStreamFormat";
            this.cmbStreamFormat.Size = new System.Drawing.Size(98, 24);
            this.cmbStreamFormat.TabIndex = 29;
            this.cmbStreamFormat.SelectedIndexChanged += new System.EventHandler(this.cmbStreamFormat_SelectedIndexChanged);
            // 
            // lblStreamFormat
            // 
            this.lblStreamFormat.AutoSize = true;
            this.lblStreamFormat.Location = new System.Drawing.Point(12, 83);
            this.lblStreamFormat.Name = "lblStreamFormat";
            this.lblStreamFormat.Size = new System.Drawing.Size(97, 17);
            this.lblStreamFormat.TabIndex = 28;
            this.lblStreamFormat.Text = "Stream format";
            // 
            // chkShowLagControl
            // 
            this.chkShowLagControl.AutoSize = true;
            this.chkShowLagControl.Location = new System.Drawing.Point(15, 237);
            this.chkShowLagControl.Name = "chkShowLagControl";
            this.chkShowLagControl.Size = new System.Drawing.Size(228, 21);
            this.chkShowLagControl.TabIndex = 27;
            this.chkShowLagControl.Text = "Show lag control (experimental)";
            this.chkShowLagControl.UseVisualStyleBackColor = true;
            this.chkShowLagControl.CheckedChanged += new System.EventHandler(this.chkShowLagControl_CheckedChanged);
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.Location = new System.Drawing.Point(869, 515);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(152, 27);
            this.btnResetSettings.TabIndex = 26;
            this.btnResetSettings.Text = "Reset Settings";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.btnResetSettings_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(12, 520);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(0, 17);
            this.lblVersion.TabIndex = 25;
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(15, 185);
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
            this.chkShowWindowOnStart.Location = new System.Drawing.Point(15, 159);
            this.chkShowWindowOnStart.Name = "chkShowWindowOnStart";
            this.chkShowWindowOnStart.Size = new System.Drawing.Size(177, 21);
            this.chkShowWindowOnStart.TabIndex = 23;
            this.chkShowWindowOnStart.Text = "Show window at startup";
            this.chkShowWindowOnStart.UseVisualStyleBackColor = true;
            // 
            // cmbRecordingDevice
            // 
            this.cmbRecordingDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordingDevice.FormattingEnabled = true;
            this.cmbRecordingDevice.Location = new System.Drawing.Point(158, 51);
            this.cmbRecordingDevice.Name = "cmbRecordingDevice";
            this.cmbRecordingDevice.Size = new System.Drawing.Size(442, 24);
            this.cmbRecordingDevice.TabIndex = 22;
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(12, 54);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(122, 17);
            this.lblDevice.TabIndex = 21;
            this.lblDevice.Text = "Recording device:";
            // 
            // cmbIP4AddressUsed
            // 
            this.cmbIP4AddressUsed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIP4AddressUsed.FormattingEnabled = true;
            this.cmbIP4AddressUsed.Location = new System.Drawing.Point(158, 21);
            this.cmbIP4AddressUsed.Name = "cmbIP4AddressUsed";
            this.cmbIP4AddressUsed.Size = new System.Drawing.Size(442, 24);
            this.cmbIP4AddressUsed.TabIndex = 20;
            // 
            // lblIpAddressUsed
            // 
            this.lblIpAddressUsed.AutoSize = true;
            this.lblIpAddressUsed.Location = new System.Drawing.Point(12, 24);
            this.lblIpAddressUsed.Name = "lblIpAddressUsed";
            this.lblIpAddressUsed.Size = new System.Drawing.Size(122, 17);
            this.lblIpAddressUsed.TabIndex = 19;
            this.lblIpAddressUsed.Text = "IP4 address used:";
            // 
            // chkAutoRestart
            // 
            this.chkAutoRestart.AutoSize = true;
            this.chkAutoRestart.Location = new System.Drawing.Point(15, 211);
            this.chkAutoRestart.Name = "chkAutoRestart";
            this.chkAutoRestart.Size = new System.Drawing.Size(325, 21);
            this.chkAutoRestart.TabIndex = 18;
            this.chkAutoRestart.Text = "Automatically restart when the stream is closed";
            this.chkAutoRestart.UseVisualStyleBackColor = true;
            // 
            // chkHook
            // 
            this.chkHook.AutoSize = true;
            this.chkHook.Location = new System.Drawing.Point(15, 133);
            this.chkHook.Name = "chkHook";
            this.chkHook.Size = new System.Drawing.Size(556, 21);
            this.chkHook.TabIndex = 14;
            this.chkHook.Text = "Use Keyboard shortcuts: Up = Ctrl+Alt+U; Down = Ctrl+Alt+D; (Un)Mute = Ctrl+Alt+M" +
    "";
            this.chkHook.UseVisualStyleBackColor = true;
            this.chkHook.CheckedChanged += new System.EventHandler(this.chkHook_CheckedChanged);
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.btnClipboardCopy);
            this.tabPageLog.Controls.Add(this.labelPingPong);
            this.tabPageLog.Controls.Add(this.textLog);
            this.tabPageLog.Location = new System.Drawing.Point(4, 25);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(1092, 603);
            this.tabPageLog.TabIndex = 0;
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // btnClipboardCopy
            // 
            this.btnClipboardCopy.Location = new System.Drawing.Point(888, 548);
            this.btnClipboardCopy.Name = "btnClipboardCopy";
            this.btnClipboardCopy.Size = new System.Drawing.Size(190, 33);
            this.btnClipboardCopy.TabIndex = 3;
            this.btnClipboardCopy.Text = "Copy to clipboard";
            this.btnClipboardCopy.UseVisualStyleBackColor = true;
            this.btnClipboardCopy.Click += new System.EventHandler(this.btnClipboardCopy_Click);
            // 
            // labelPingPong
            // 
            this.labelPingPong.AutoSize = true;
            this.labelPingPong.Location = new System.Drawing.Point(12, 548);
            this.labelPingPong.Name = "labelPingPong";
            this.labelPingPong.Size = new System.Drawing.Size(148, 17);
            this.labelPingPong.TabIndex = 2;
            this.labelPingPong.Text = "                                   ";
            // 
            // textLog
            // 
            this.textLog.Font = new System.Drawing.Font("Courier New", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLog.Location = new System.Drawing.Point(15, 19);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(1063, 515);
            this.textLog.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 657);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Chromecast Desktop Audio Streamer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.grpLag.ResumeLayout(false);
            this.grpLag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).EndInit();
            this.grpDevices.ResumeLayout(false);
            this.grpVolume.ResumeLayout(false);
            this.tabPageOptions.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.tabPageLog.ResumeLayout(false);
            this.tabPageLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.GroupBox grpLag;
        private System.Windows.Forms.TrackBar trbLag;
        private System.Windows.Forms.GroupBox grpDevices;
        private System.Windows.Forms.FlowLayoutPanel pnlDevices;
        private System.Windows.Forms.GroupBox grpVolume;
        private System.Windows.Forms.Button btnVolumeMute;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Label labelPingPong;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Label lblLagMin;
        private System.Windows.Forms.Label lblLagMax;
        private System.Windows.Forms.Label lblLagExperimental;
        private System.Windows.Forms.Button btnVolumeUp;
        private System.Windows.Forms.Button btnVolumeDown;
        private System.Windows.Forms.Button btnSyncDevices;
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
        private System.Windows.Forms.Label lblStreamFormatExtra;
    }
}

