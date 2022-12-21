﻿namespace ChromeCast.Desktop.AudioStreamer.UserControls
{
    partial class DeviceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDevice = new System.Windows.Forms.Button();
            this.trbVolume = new System.Windows.Forms.TrackBar();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.picturePlayPause = new System.Windows.Forms.PictureBox();
            this.pictureGroup = new System.Windows.Forms.PictureBox();
            this.pictureVolumeMute = new System.Windows.Forms.PictureBox();
            this.toolTipGroup = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picturePlayPause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVolumeMute)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblStatus.Location = new System.Drawing.Point(0, 112);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(315, 26);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "status";
            this.lblStatus.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            this.lblStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DeviceChildControl_MouseDown);
            // 
            // btnDevice
            // 
            this.btnDevice.AutoSize = true;
            this.btnDevice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDevice.FlatAppearance.BorderSize = 0;
            this.btnDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDevice.Location = new System.Drawing.Point(43, 0);
            this.btnDevice.Margin = new System.Windows.Forms.Padding(0);
            this.btnDevice.Name = "btnDevice";
            this.btnDevice.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnDevice.Size = new System.Drawing.Size(272, 52);
            this.btnDevice.TabIndex = 3;
            this.btnDevice.Text = "name";
            this.btnDevice.UseVisualStyleBackColor = true;
            this.btnDevice.Click += new System.EventHandler(this.BtnDevicePlay_Click);
            this.btnDevice.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // trbVolume
            // 
            this.trbVolume.Enabled = false;
            this.trbVolume.Location = new System.Drawing.Point(47, 52);
            this.trbVolume.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trbVolume.Maximum = 100;
            this.trbVolume.MinimumSize = new System.Drawing.Size(250, 0);
            this.trbVolume.Name = "trbVolume";
            this.trbVolume.Size = new System.Drawing.Size(268, 56);
            this.trbVolume.SmallChange = 5;
            this.trbVolume.TabIndex = 5;
            this.trbVolume.TickFrequency = 5;
            this.trbVolume.Scroll += new System.EventHandler(this.TrbVolume_Scroll);
            this.trbVolume.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.picturePlayPause);
            this.flowLayoutPanel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 4);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(252, 40);
            this.flowLayoutPanel1.TabIndex = 10;
            this.flowLayoutPanel1.Click += new System.EventHandler(this.BtnDevicePlay_Click);
            // 
            // picturePlayPause
            // 
            this.picturePlayPause.BackColor = System.Drawing.Color.White;
            this.picturePlayPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picturePlayPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picturePlayPause.Image = global::ChromeCast.Desktop.AudioStreamer.Properties.Resources.Play;
            this.picturePlayPause.Location = new System.Drawing.Point(5, 6);
            this.picturePlayPause.Margin = new System.Windows.Forms.Padding(0);
            this.picturePlayPause.Name = "picturePlayPause";
            this.picturePlayPause.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picturePlayPause.Size = new System.Drawing.Size(30, 30);
            this.picturePlayPause.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picturePlayPause.TabIndex = 7;
            this.picturePlayPause.TabStop = false;
            this.picturePlayPause.Click += new System.EventHandler(this.BtnDevicePlay_Click);
            this.picturePlayPause.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // pictureGroup
            // 
            this.pictureGroup.Image = global::ChromeCast.Desktop.AudioStreamer.Properties.Resources.Group;
            this.pictureGroup.Location = new System.Drawing.Point(283, 112);
            this.pictureGroup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureGroup.Name = "pictureGroup";
            this.pictureGroup.Size = new System.Drawing.Size(32, 22);
            this.pictureGroup.TabIndex = 11;
            this.pictureGroup.TabStop = false;
            // 
            // pictureVolumeMute
            // 
            this.pictureVolumeMute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureVolumeMute.Image = global::ChromeCast.Desktop.AudioStreamer.Properties.Resources.Unmute;
            this.pictureVolumeMute.Location = new System.Drawing.Point(8, 52);
            this.pictureVolumeMute.Margin = new System.Windows.Forms.Padding(0);
            this.pictureVolumeMute.Name = "pictureVolumeMute";
            this.pictureVolumeMute.Size = new System.Drawing.Size(30, 38);
            this.pictureVolumeMute.TabIndex = 8;
            this.pictureVolumeMute.TabStop = false;
            this.pictureVolumeMute.Click += new System.EventHandler(this.PictureVolumeMute_Click);
            this.pictureVolumeMute.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // DeviceControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.pictureGroup);
            this.Controls.Add(this.btnDevice);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.pictureVolumeMute);
            this.Controls.Add(this.trbVolume);
            this.Controls.Add(this.lblStatus);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(315, 175);
            this.Name = "DeviceControl";
            this.Size = new System.Drawing.Size(315, 138);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DeviceControl_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picturePlayPause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVolumeMute)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDevice;
        private System.Windows.Forms.TrackBar trbVolume;
        private System.Windows.Forms.PictureBox picturePlayPause;
        private System.Windows.Forms.PictureBox pictureVolumeMute;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureGroup;
        private System.Windows.Forms.ToolTip toolTipGroup;
    }
}
