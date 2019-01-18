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
            this.lblState = new System.Windows.Forms.Label();
            this.btnDevice = new System.Windows.Forms.Button();
            this.trbVolume = new System.Windows.Forms.TrackBar();
            this.pictureVolumeMute = new System.Windows.Forms.PictureBox();
            this.picturePlayPause = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVolumeMute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePlayPause)).BeginInit();
            this.SuspendLayout();
            // 
            // lblState
            // 
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.lblState.Location = new System.Drawing.Point(3, 116);
            this.lblState.Margin = new System.Windows.Forms.Padding(3, 0, 10, 0);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(301, 21);
            this.lblState.TabIndex = 4;
            this.lblState.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            this.lblState.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DeviceChildControl_MouseDown);
            // 
            // btnDevice
            // 
            this.btnDevice.AutoSize = true;
            this.btnDevice.FlatAppearance.BorderSize = 0;
            this.btnDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevice.Location = new System.Drawing.Point(54, 0);
            this.btnDevice.Margin = new System.Windows.Forms.Padding(0);
            this.btnDevice.MaximumSize = new System.Drawing.Size(250, 0);
            this.btnDevice.Name = "btnDevice";
            this.btnDevice.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.btnDevice.Size = new System.Drawing.Size(250, 50);
            this.btnDevice.TabIndex = 3;
            this.btnDevice.Text = "name";
            this.btnDevice.UseVisualStyleBackColor = true;
            this.btnDevice.Click += new System.EventHandler(this.BtnDevice_Click);
            this.btnDevice.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // trbVolume
            // 
            this.trbVolume.Enabled = false;
            this.trbVolume.Location = new System.Drawing.Point(43, 59);
            this.trbVolume.Maximum = 100;
            this.trbVolume.MinimumSize = new System.Drawing.Size(250, 0);
            this.trbVolume.Name = "trbVolume";
            this.trbVolume.Size = new System.Drawing.Size(261, 56);
            this.trbVolume.SmallChange = 5;
            this.trbVolume.TabIndex = 5;
            this.trbVolume.TickFrequency = 5;
            this.trbVolume.Scroll += new System.EventHandler(this.TrbVolume_Scroll);
            this.trbVolume.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // pictureVolumeMute
            // 
            this.pictureVolumeMute.Image = global::ChromeCast.Desktop.AudioStreamer.Properties.Resources.Unmute;
            this.pictureVolumeMute.Location = new System.Drawing.Point(10, 59);
            this.pictureVolumeMute.Margin = new System.Windows.Forms.Padding(0);
            this.pictureVolumeMute.Name = "pictureVolumeMute";
            this.pictureVolumeMute.Size = new System.Drawing.Size(30, 30);
            this.pictureVolumeMute.TabIndex = 8;
            this.pictureVolumeMute.TabStop = false;
            this.pictureVolumeMute.Click += new System.EventHandler(this.PictureVolumeMute_Click);
            this.pictureVolumeMute.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // picturePlayPause
            // 
            this.picturePlayPause.BackColor = System.Drawing.Color.White;
            this.picturePlayPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picturePlayPause.Image = global::ChromeCast.Desktop.AudioStreamer.Properties.Resources.Play;
            this.picturePlayPause.Location = new System.Drawing.Point(10, 9);
            this.picturePlayPause.Name = "picturePlayPause";
            this.picturePlayPause.Padding = new System.Windows.Forms.Padding(1);
            this.picturePlayPause.Size = new System.Drawing.Size(35, 35);
            this.picturePlayPause.TabIndex = 7;
            this.picturePlayPause.TabStop = false;
            this.picturePlayPause.Click += new System.EventHandler(this.BtnDevice_Click);
            this.picturePlayPause.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            // 
            // DeviceControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.pictureVolumeMute);
            this.Controls.Add(this.picturePlayPause);
            this.Controls.Add(this.trbVolume);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.btnDevice);
            this.Name = "DeviceControl";
            this.Size = new System.Drawing.Size(314, 137);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceControl_DragOver);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DeviceControl_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVolumeMute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePlayPause)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Button btnDevice;
        private System.Windows.Forms.TrackBar trbVolume;
        private System.Windows.Forms.PictureBox picturePlayPause;
        private System.Windows.Forms.PictureBox pictureVolumeMute;
    }
}
