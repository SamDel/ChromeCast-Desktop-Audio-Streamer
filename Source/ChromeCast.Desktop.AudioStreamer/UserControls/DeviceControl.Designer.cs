namespace ChromeCast.Desktop.AudioStreamer.UserControls
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDevice = new System.Windows.Forms.Button();
            this.trbVolume = new System.Windows.Forms.TrackBar();
            this.btnVolumeMute = new System.Windows.Forms.Button();
            this.picturePlayPause = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePlayPause)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblStatus.Location = new System.Drawing.Point(3, 116);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(275, 21);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "status";
            // 
            // btnDevice
            // 
            this.btnDevice.AutoSize = true;
            this.btnDevice.FlatAppearance.BorderSize = 0;
            this.btnDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevice.Location = new System.Drawing.Point(0, 0);
            this.btnDevice.Margin = new System.Windows.Forms.Padding(0);
            this.btnDevice.Name = "btnDevice";
            this.btnDevice.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.btnDevice.Size = new System.Drawing.Size(284, 51);
            this.btnDevice.TabIndex = 3;
            this.btnDevice.Text = "name";
            this.btnDevice.UseVisualStyleBackColor = true;
            // 
            // trbVolume
            // 
            this.trbVolume.Enabled = false;
            this.trbVolume.Location = new System.Drawing.Point(34, 59);
            this.trbVolume.Maximum = 100;
            this.trbVolume.MinimumSize = new System.Drawing.Size(250, 0);
            this.trbVolume.Name = "trbVolume";
            this.trbVolume.Size = new System.Drawing.Size(250, 56);
            this.trbVolume.SmallChange = 5;
            this.trbVolume.TabIndex = 5;
            this.trbVolume.TickFrequency = 5;
            this.trbVolume.Scroll += new System.EventHandler(this.trbVolume_Scroll);
            // 
            // btnVolumeMute
            // 
            this.btnVolumeMute.Enabled = false;
            this.btnVolumeMute.Location = new System.Drawing.Point(4, 59);
            this.btnVolumeMute.MaximumSize = new System.Drawing.Size(30, 30);
            this.btnVolumeMute.MinimumSize = new System.Drawing.Size(30, 30);
            this.btnVolumeMute.Name = "btnVolumeMute";
            this.btnVolumeMute.Size = new System.Drawing.Size(30, 30);
            this.btnVolumeMute.TabIndex = 6;
            this.btnVolumeMute.Text = "M";
            this.btnVolumeMute.UseVisualStyleBackColor = true;
            this.btnVolumeMute.Click += new System.EventHandler(this.btnVolumeMute_Click);
            // 
            // picturePlayPause
            // 
            this.picturePlayPause.BackColor = System.Drawing.Color.White;
            this.picturePlayPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picturePlayPause.Image = global::ChromeCast.Desktop.AudioStreamer.Properties.Resources.Play;
            this.picturePlayPause.Location = new System.Drawing.Point(42, 8);
            this.picturePlayPause.Name = "picturePlayPause";
            this.picturePlayPause.Padding = new System.Windows.Forms.Padding(1);
            this.picturePlayPause.Size = new System.Drawing.Size(35, 35);
            this.picturePlayPause.TabIndex = 7;
            this.picturePlayPause.TabStop = false;
            // 
            // DeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.picturePlayPause);
            this.Controls.Add(this.btnVolumeMute);
            this.Controls.Add(this.trbVolume);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnDevice);
            this.Name = "DeviceControl";
            this.Size = new System.Drawing.Size(288, 137);
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePlayPause)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDevice;
        private System.Windows.Forms.TrackBar trbVolume;
        private System.Windows.Forms.Button btnVolumeMute;
        private System.Windows.Forms.PictureBox picturePlayPause;
    }
}
