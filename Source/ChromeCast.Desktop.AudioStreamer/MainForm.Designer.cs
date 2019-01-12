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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grpLag = new System.Windows.Forms.GroupBox();
            this.lblLag = new System.Windows.Forms.Label();
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
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkShowWindowOnStart = new System.Windows.Forms.CheckBox();
            this.cmbRecordingDevice = new System.Windows.Forms.ComboBox();
            this.lblDevice = new System.Windows.Forms.Label();
            this.cmbIP4AddressUsed = new System.Windows.Forms.ComboBox();
            this.lblIpAddressUsed = new System.Windows.Forms.Label();
            this.chkAutoRestart = new System.Windows.Forms.CheckBox();
            this.chkHook = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnClipboardCopy = new System.Windows.Forms.Button();
            this.labelPingPong = new System.Windows.Forms.Label();
            this.textLog = new System.Windows.Forms.TextBox();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpLag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).BeginInit();
            this.grpDevices.SuspendLayout();
            this.grpVolume.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(25, 13);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1100, 632);
            this.tabControl.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.grpLag);
            this.tabPage2.Controls.Add(this.grpDevices);
            this.tabPage2.Controls.Add(this.grpVolume);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1092, 603);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // grpLag
            // 
            this.grpLag.Controls.Add(this.lblLag);
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
            // lblLag
            // 
            this.lblLag.AutoSize = true;
            this.lblLag.Location = new System.Drawing.Point(346, 86);
            this.lblLag.Name = "lblLag";
            this.lblLag.Size = new System.Drawing.Size(641, 17);
            this.lblLag.TabIndex = 3;
            this.lblLag.Text = "Experimental feature: Try to keep the buffer on the device as small as possible w" +
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
            this.grpDevices.Location = new System.Drawing.Point(44, 150);
            this.grpDevices.Name = "grpDevices";
            this.grpDevices.Size = new System.Drawing.Size(1010, 321);
            this.grpDevices.TabIndex = 11;
            this.grpDevices.TabStop = false;
            this.grpDevices.Text = "Devices (click name to start streaming)";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(785, 288);
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
            this.pnlDevices.Size = new System.Drawing.Size(969, 265);
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
            this.grpVolume.Size = new System.Drawing.Size(1010, 134);
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
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1092, 603);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Options";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnResetSettings);
            this.groupBox1.Controls.Add(this.lblVersion);
            this.groupBox1.Controls.Add(this.chkAutoStart);
            this.groupBox1.Controls.Add(this.chkShowWindowOnStart);
            this.groupBox1.Controls.Add(this.cmbRecordingDevice);
            this.groupBox1.Controls.Add(this.lblDevice);
            this.groupBox1.Controls.Add(this.cmbIP4AddressUsed);
            this.groupBox1.Controls.Add(this.lblIpAddressUsed);
            this.groupBox1.Controls.Add(this.chkAutoRestart);
            this.groupBox1.Controls.Add(this.chkHook);
            this.groupBox1.Location = new System.Drawing.Point(27, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1038, 553);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
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
            this.chkAutoStart.Location = new System.Drawing.Point(15, 75);
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
            this.chkShowWindowOnStart.Location = new System.Drawing.Point(15, 49);
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
            this.cmbRecordingDevice.Location = new System.Drawing.Point(153, 156);
            this.cmbRecordingDevice.Name = "cmbRecordingDevice";
            this.cmbRecordingDevice.Size = new System.Drawing.Size(429, 24);
            this.cmbRecordingDevice.TabIndex = 22;
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(12, 159);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(122, 17);
            this.lblDevice.TabIndex = 21;
            this.lblDevice.Text = "Recording device:";
            // 
            // cmbIP4AddressUsed
            // 
            this.cmbIP4AddressUsed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIP4AddressUsed.FormattingEnabled = true;
            this.cmbIP4AddressUsed.Location = new System.Drawing.Point(153, 126);
            this.cmbIP4AddressUsed.Name = "cmbIP4AddressUsed";
            this.cmbIP4AddressUsed.Size = new System.Drawing.Size(429, 24);
            this.cmbIP4AddressUsed.TabIndex = 20;
            // 
            // lblIpAddressUsed
            // 
            this.lblIpAddressUsed.AutoSize = true;
            this.lblIpAddressUsed.Location = new System.Drawing.Point(12, 129);
            this.lblIpAddressUsed.Name = "lblIpAddressUsed";
            this.lblIpAddressUsed.Size = new System.Drawing.Size(122, 17);
            this.lblIpAddressUsed.TabIndex = 19;
            this.lblIpAddressUsed.Text = "IP4 address used:";
            // 
            // chkAutoRestart
            // 
            this.chkAutoRestart.AutoSize = true;
            this.chkAutoRestart.Location = new System.Drawing.Point(15, 101);
            this.chkAutoRestart.Name = "chkAutoRestart";
            this.chkAutoRestart.Size = new System.Drawing.Size(325, 21);
            this.chkAutoRestart.TabIndex = 18;
            this.chkAutoRestart.Text = "Automatically restart when the stream is closed";
            this.chkAutoRestart.UseVisualStyleBackColor = true;
            // 
            // chkHook
            // 
            this.chkHook.AutoSize = true;
            this.chkHook.Location = new System.Drawing.Point(15, 23);
            this.chkHook.Name = "chkHook";
            this.chkHook.Size = new System.Drawing.Size(556, 21);
            this.chkHook.TabIndex = 14;
            this.chkHook.Text = "Use Keyboard shortcuts: Up = Ctrl+Alt+U; Down = Ctrl+Alt+D; (Un)Mute = Ctrl+Alt+M" +
    "";
            this.chkHook.UseVisualStyleBackColor = true;
            this.chkHook.CheckedChanged += new System.EventHandler(this.chkHook_CheckedChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnClipboardCopy);
            this.tabPage1.Controls.Add(this.labelPingPong);
            this.tabPage1.Controls.Add(this.textLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1092, 603);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            this.Text = "ChromeCast Desktop Audio Streamer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.grpLag.ResumeLayout(false);
            this.grpLag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbLag)).EndInit();
            this.grpDevices.ResumeLayout(false);
            this.grpVolume.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox grpLag;
        private System.Windows.Forms.TrackBar trbLag;
        private System.Windows.Forms.GroupBox grpDevices;
        private System.Windows.Forms.FlowLayoutPanel pnlDevices;
        private System.Windows.Forms.GroupBox grpVolume;
        private System.Windows.Forms.Button btnVolumeMute;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label labelPingPong;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Label lblLagMin;
        private System.Windows.Forms.Label lblLagMax;
        private System.Windows.Forms.Label lblLag;
        private System.Windows.Forms.Button btnVolumeUp;
        private System.Windows.Forms.Button btnVolumeDown;
        private System.Windows.Forms.Button btnSyncDevices;
        private System.Windows.Forms.Button btnClipboardCopy;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
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
    }
}

