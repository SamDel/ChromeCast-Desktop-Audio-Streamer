using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rssdp;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class ApplicationLogic : IApplicationLogic
    {
        private IDevices devices;
        private IMainForm mainForm;
        private IConfiguration configuration;
        private ILoopbackRecorder loopbackRecorder;
        private IStreamingRequestsListener streamingRequestListener;
        private IDiscoverDevices discoverDevices;
        private IDeviceStatusTimer deviceStatusTimer;
        private NotifyIcon notifyIcon;
        private const int trbLagMaximumValue = 1000;
        private int reduceLagThreshold = trbLagMaximumValue;
        private string streamingUrl = string.Empty;

        public ApplicationLogic(IDevices devicesIn, IDiscoverDevices discoverDevicesIn
            , ILoopbackRecorder loopbackRecorderIn, IConfiguration configurationIn
            , IStreamingRequestsListener streamingRequestListenerIn, IDeviceStatusTimer deviceStatusTimerIn)
        {
            devices = devicesIn;
            devices.SetCallback(OnAddDevice);
            discoverDevices = discoverDevicesIn;
            loopbackRecorder = loopbackRecorderIn;
            configuration = configurationIn;
            streamingRequestListener = streamingRequestListenerIn;
            deviceStatusTimer = deviceStatusTimerIn;
        }

        public void Start()
        {
            Task.Run(() => { streamingRequestListener.StartListening(OnStreamingRequestsListen, OnStreamingRequestConnect); });
            AddNotifyIcon();
            configuration.Load(SetConfiguration);
            discoverDevices.Discover(devices.OnDeviceAvailable);
            deviceStatusTimer.StartPollingDevice(devices.OnGetStatus);
        }

        private void ToggleFormVisibility(object sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MouseEventArgs)))
            {
                if (((MouseEventArgs)e).Button != MouseButtons.Left) return;
            }

            mainForm.ToggleVisibility();
        }

        public void OnStreamingRequestsListen(string host, int port)
        {
            Console.WriteLine(string.Format("Streaming from {0}:{1}", host, port));
            streamingUrl = string.Format("http://{0}:{1}/", host, port);
        }

        public void OnStreamingRequestConnect(Socket socket, string httpRequest)
        {
            Console.WriteLine(string.Format("Connection added from {0}", socket.RemoteEndPoint));

            loopbackRecorder.StartRecording(OnRecordingDataAvailable);
            devices.AddStreamingConnection(socket, httpRequest);
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format)
        {
            devices.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold);
        }

        public void OnSetHooks(bool setHooks)
        {
            if (setHooks)
                SetWindowsHook.Start(devices);
            else
                SetWindowsHook.Stop();
        }

        public void OnAddDevice(IDevice device)
        {
            var menuItem = new MenuItem();
            menuItem.Text = device.GetFriendlyName();
            menuItem.Click += device.OnClickDeviceButton;
            notifyIcon.ContextMenu.MenuItems.Add(notifyIcon.ContextMenu.MenuItems.Count - 1, menuItem);
            device.SetMenuItem(menuItem);

            mainForm.AddDevice(device);
        }

        private void AddNotifyIcon()
        {
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Index = 0;
            menuItem.Text = "Close";
            menuItem.Click += new EventHandler(CloseApplication);
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            notifyIcon = new NotifyIcon();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            notifyIcon.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            notifyIcon.Visible = true;
            notifyIcon.Text = "ChromeCast Desktop Streamer";
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Click += ToggleFormVisibility;
        }

        public string GetStreamingUrl()
        {
            return streamingUrl;
        }

        public void SetLagThreshold(int lagThreshold)
        {
            reduceLagThreshold = lagThreshold;
        }

        public void SetConfiguration(bool useShortCuts, bool showLog, bool showLagControl, int lagValue, bool autoStart, string ipAddressesDevices)
        {
            mainForm.SetKeyboardHooks(useShortCuts);
            mainForm.ShowLog(showLog);
            mainForm.ShowLagControl(showLagControl);
            mainForm.SetLagValue(lagValue);
            devices.SetAutoStart(autoStart);

            if (!string.IsNullOrWhiteSpace(ipAddressesDevices))
            {
                var ipDevices = ipAddressesDevices.Split(';');
                foreach (var ipDevice in ipDevices)
                {
                    var arrDevice = ipDevice.Split(',');
                    devices.OnDeviceAvailable(
                            new DiscoveredSsdpDevice { DescriptionLocation = new Uri($"http://{arrDevice[0]}") },
                            new SsdpRootDevice { FriendlyName = arrDevice[1] }
                        );
                }
            }
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            SetWindowsHook.Stop();
            loopbackRecorder.StopRecording();
            devices.Dispose();
            streamingRequestListener.StopListening();
            notifyIcon.Visible = false;
            mainForm.Dispose();
        }

        public void SetDependencies(MainForm mainFormIn)
        {
            mainForm = mainFormIn;
        }
    }
}
