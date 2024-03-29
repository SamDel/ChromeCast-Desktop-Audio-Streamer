﻿using System;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            System.Windows.Forms.Application.EnableVisualStyles();
            //TODO: .Net 6.0 replacement necessary? (SetHighDpiMode is not available anymore)
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                //new SingleInstanceController().Run(new string[0]);
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(UnhandledHandler);

                var logger = new Logger();
                var devices = new Devices(logger);
                var mainForm = new MainForm(
                        new ApplicationLogic(devices
                            , new DiscoverDevices()
                            , new Configuration()
                            , new StreamingRequestsListener()
                            , new DeviceStatusTimer()
                            , logger)
                        , devices
                        , new LoopbackRecorder(logger)
                        , logger);
                System.Windows.Forms.Application.Run(mainForm);
            }
            catch (Exception)
            {
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private static void UnhandledHandler(object sender, System.UnhandledExceptionEventArgs e)
        {
#if DEBUG
            Exception exception = (Exception)e.ExceptionObject;
            MessageBox.Show(exception.Message);
#endif
        }

        /// <summary>
        /// Make sure only one instance is running.
        /// </summary>
        public class SingleInstanceController : WindowsFormsApplicationBase
        {
            public SingleInstanceController()
            {
                IsSingleInstance = true;
                StartupNextInstance += MainFormStartupNextInstance;
            }

            void MainFormStartupNextInstance(object sender, StartupNextInstanceEventArgs e)
            {
                var form = MainForm as MainForm;
                if (!form.IsDisposed)
                {
                    form.Show();
                    form.TopMost = true;
                    form.TopMost = false;
                }
            }

            protected override void OnCreateMainForm()
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(UnhandledHandler);

                var logger = new Logger();
                var devices = new Devices(logger);
                MainForm = new MainForm(
                        new ApplicationLogic(devices
                            , new DiscoverDevices()
                            , new Configuration()
                            , new StreamingRequestsListener()
                            , new DeviceStatusTimer()
                            , logger)
                        , devices
                        , new LoopbackRecorder(logger)
                        , logger);
                System.Windows.Forms.Application.Run(MainForm);
            }
        }
    }
}
