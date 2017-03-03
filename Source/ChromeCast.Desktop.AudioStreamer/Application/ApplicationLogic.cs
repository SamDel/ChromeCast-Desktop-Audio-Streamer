using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rssdp;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Discover;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class ApplicationLogic
    {
        public bool AutoStart = false;
        private LoopbackRecorder loopbackRecorder;
        private DiscoverServiceSSDP discover;
        private Devices devices;
        private Timer getStatusTimer;
        private Timer discoverTillFoundTimer;
        private int discoverTillFoundTimerCounter = 0;
        private MainForm mainForm;
        private NotifyIcon notifyIcon;
        private AsynchronousSocketListener socketListener;
        private const int trbLagMaximumValue = 1000;
        private int reduceLagThreshold = trbLagMaximumValue;
        private string streamingUrl = string.Empty;

        public ApplicationLogic(MainForm mainFormIn)
        {
            loopbackRecorder = new LoopbackRecorder(this);
            devices = new Devices(this);
            getStatusTimer = new Timer();
            discoverTillFoundTimer = new Timer();
            mainForm = mainFormIn;
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            SetWindowsHook.Stop();
            loopbackRecorder.StopRecording();
            devices.Dispose();
            socketListener.StopListening();
            mainForm.DisposeForm();
            notifyIcon.Visible = false;
        }

        private void ShowApplication(object sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MouseEventArgs)))
            {
                if (((MouseEventArgs)e).Button != MouseButtons.Left) return;
            }

            if (mainForm.Visible)
            {
                mainForm.Hide();
            }
            else
            {
                mainForm.Show();
                mainForm.Activate();
                mainForm.WindowState = FormWindowState.Normal;
            }
        }

        public void Start()
        {
            AddNotifyIcon();
            socketListener = new AsynchronousSocketListener(this);
            Task.Run(() => { socketListener.StartListening(); });

            discover = new DiscoverServiceSSDP(this);
            discover.Discover();

            discoverTillFoundTimer.Interval = 5000;
            discoverTillFoundTimer.Tick += new EventHandler(OnDiscoverTillFoundTimer);
            discoverTillFoundTimer.Start();

            getStatusTimer.Interval = 10000;
            getStatusTimer.Tick += new EventHandler(OnGetStatus);
            getStatusTimer.Start();
        }

        public void OnListen(string host, int port)
        {
            Console.WriteLine(string.Format("Streaming from {0}:{1}", host, port));
            streamingUrl = string.Format("http://{0}:{1}/", host, port);
        }

        public void OnStreamRequestConnect(Socket socket, string httpRequest)
        {
            Console.WriteLine(string.Format("Connection added from {0}", socket.RemoteEndPoint));

            loopbackRecorder.StartRecording();
            devices.AddStreamingConnection(socket, httpRequest);
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format)
        {
            devices.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold);
        }

        private void OnGetStatus(object sender, EventArgs e)
        {
            devices.OnGetStatus();
        }

        private void OnDiscoverTillFoundTimer(object sender, EventArgs e)
        {
            if (devices.CountDiscovered() == 0 && discoverTillFoundTimerCounter <= 5)
                discover.Discover();
            else
                discoverTillFoundTimer.Stop();

            discoverTillFoundTimerCounter++;
        }

        public void OnSetHooks(bool setHooks)
        {
            if (setHooks)
                SetWindowsHook.Start(this);
            else
                SetWindowsHook.Stop();
        }

        public void OnDeviceAvailable(DiscoveredSsdpDevice device, SsdpDevice fullDevice)
        {
            devices.AddDevice(device, fullDevice);
        }

        public void OnAddDevice(Device device)
        {
            var menuItem = new MenuItem();
            menuItem.Text = device.SsdpDevice.FriendlyName;
            menuItem.Click += device.OnClickDeviceButton;
            notifyIcon.ContextMenu.MenuItems.Add(notifyIcon.ContextMenu.MenuItems.Count - 1, menuItem);
            device.MenuItem = menuItem;

            mainForm.AddDevice(device);
        }

        public void VolumeUp()
        {
            devices.VolumeUp();
        }

        public void VolumeDown()
        {
            devices.VolumeDown();
        }

        public void VolumeMute()
        {
            devices.VolumeMute();
        }

        public void VolumeSet(float level)
        {
            devices.VolumeSet(level);
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
            try
            {
                notifyIcon.Icon = new Icon(@"ChromeCast.ico");
            }
            catch (Exception)
            {
            }
            notifyIcon.Visible = true;
            notifyIcon.Text = "ChromeCast Desktop Streamer";
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Click += ShowApplication;
        }

        public string GetStreamingUrl()
        {
            return streamingUrl;
        }

        public void SetLagThreshold(int lagThreshold)
        {
            reduceLagThreshold = lagThreshold;
        }

        public void Log(string message)
        {
            mainForm.Log(message);
        }
    }
}
