using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Classes;
using System.Timers;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Devices : IDevices
    {
        private List<IDevice> deviceList = new List<IDevice>();
        private Action<Device> onAddDeviceCallback;
        private bool AutoStart;
        private IMainForm mainForm;
        private IApplicationLogic applicationLogic;

        /// <summary>
        /// A new device is discoverd. Add the device, or update if it already exists.
        /// </summary>
        /// <param name="discoveredDevice">the discovered device</param>
        public void OnDeviceAvailable(DiscoveredDevice discoveredDevice)
        {
            if (deviceList == null || discoveredDevice == null)
                return;

            var existingDevice = deviceList.FirstOrDefault(
                d => d.GetHost().Equals(discoveredDevice.IPAddress)
                && d.GetPort().Equals(discoveredDevice.Port));
            if (existingDevice == null)
            {
                if (!deviceList.Any(d => d.GetUsn() != null && 
                    d.GetUsn().Equals(discoveredDevice.Usn) && 
                    d.GetPort().Equals(discoveredDevice.Port)))
                {
                    var newDevice = DependencyFactory.Container.Resolve<Device>();
                    newDevice.Initialize(discoveredDevice);
                    deviceList.Add(newDevice);
                    onAddDeviceCallback?.Invoke(newDevice);
                    newDevice.OnGetStatus();

                    if (AutoStart)
                        newDevice.OnClickPlayPause();
                }
            }
            else
            {
                existingDevice.Initialize(discoveredDevice);
                existingDevice.SetDeviceName(existingDevice.GetFriendlyName());
            }
        }

        /// <summary>
        /// Volume up for all devices.
        /// </summary>
        public void VolumeUp()
        {
            foreach (var device in deviceList)
            {
                device.VolumeUp();
            }
        }

        /// <summary>
        /// Volume down for all devices.
        /// </summary>
        public void VolumeDown()
        {
            foreach (var device in deviceList)
            {
                device.VolumeDown();
            }
        }

        /// <summary>
        /// Volume mute for all devices.
        /// </summary>
        public void VolumeMute()
        {
            foreach (var device in deviceList)
            {
                device.VolumeMute();
            }
        }

        /// <summary>
        /// Stop for all devices. 
        /// </summary>
        /// <returns>true if one of the devices was playing, or false</returns>
        public void Stop()
        {
            foreach (var device in deviceList)
            {
                switch (device.GetDeviceState())
                {
                    case DeviceState.Playing:
                    case DeviceState.LoadingMedia:
                    case DeviceState.Buffering:
                    case DeviceState.Paused:
                        device.Stop();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Start all devices.
        /// </summary>
        public void Start()
        {
            foreach (var device in deviceList)
            {
                device.Start();
            }
        }

        public void AddStreamingConnection(Socket socket, string httpRequest)
        {
            var remoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            foreach (var device in deviceList)
            {
                if (device.AddStreamingConnection(remoteAddress, socket))
                    break;
            }
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            foreach (var device in deviceList)
            {
                device.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold, streamFormat);
            }
        }

        public void OnGetStatus()
        {
            try
            {
                for (int i = deviceList.Count - 1; i >= 0; i--)
                {
                    if (deviceList[i].GetDeviceState() == DeviceState.Disposed)
                        deviceList.RemoveAt(i);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Devices.OnGetStatus: {ex.Message}");
            }

            foreach (var device in deviceList)
            {
                device.OnGetStatus();
            }
        }

        public void SetAutoStart(bool autoStartIn)
        {
            AutoStart = autoStartIn;
        }

        public void Dispose()
        {
            Stop();
            foreach (var device in deviceList)
            {
                device.SetDeviceState(DeviceState.Disposed);
            }
        }

        public void SetCallback(Action<Device> onAddDeviceCallbackIn)
        {
            onAddDeviceCallback = onAddDeviceCallbackIn;
        }

        public int Count()
        {
            return deviceList.Count();
        }

        public void Sync()
        {
            if (mainForm.DoSyncDevices())
            {
                mainForm.SetLagValue(2);
                applicationLogic.SetLagThreshold(2);

                var timerReset = new Timer { Interval = 3000, Enabled = true };
                timerReset.Elapsed += new ElapsedEventHandler(ResetLagThreshold);
                timerReset.Start();
            }
        }

        private void ResetLagThreshold(object sender, ElapsedEventArgs e)
        {
            mainForm.SetLagValue(1000);
            applicationLogic.SetLagThreshold(1000);
            ((Timer)sender).Stop();
        }

        public void SetDependencies(MainForm mainFormIn, IApplicationLogic applicationLogicIn)
        {
            mainForm = mainFormIn;
            applicationLogic = applicationLogicIn;
        }

        public List<DiscoveredDevice> GetHosts()
        {
            var hosts = new List<DiscoveredDevice>();
            foreach (var device in deviceList)
            {
                hosts.Add(device.GetDiscoveredDevice());
            }
            return hosts;
        }
    }
}
