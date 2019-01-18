using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using Rssdp;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Classes;
using System.Timers;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Devices : IDevices
    {
        private List<IDevice> deviceList = new List<IDevice>();
        private Action<Device> onAddDeviceCallback;
        private bool AutoStart;
        private IMainForm mainForm;
        private IApplicationLogic applicationLogic;

        public void OnDeviceAvailable(DiscoveredSsdpDevice device, SsdpDevice fullDevice)
        {
            var existingDevice = deviceList.FirstOrDefault(d => d.GetHost().Equals(device.DescriptionLocation.Host));
            if (existingDevice == null)
            {
                if (!deviceList.Any(d => d.GetUsn() != null && d.GetUsn().Equals(device.Usn)))
                {
                    var newDevice = DependencyFactory.Container.Resolve<Device>();
                    newDevice.SetDiscoveredDevices(device, fullDevice);
                    deviceList.Add(newDevice);
                    onAddDeviceCallback?.Invoke(newDevice);

                    if (AutoStart)
                        newDevice.OnClickPlayPause();
                }
            }
            else
            {
                existingDevice.SetDiscoveredDevices(device, fullDevice);
                existingDevice.SetDeviceName(existingDevice.GetFriendlyName());
            }
        }

        public void VolumeUp()
        {
            foreach (var device in deviceList)
            {
                device.VolumeUp();
            }
        }

        public void VolumeDown()
        {
            foreach (var device in deviceList)
            {
                device.VolumeDown();
            }
        }

        public void VolumeMute()
        {
            foreach (var device in deviceList)
            {
                device.VolumeMute();
            }
        }

        public bool Stop()
        {
            var wasPlaying = false;
            foreach (var device in deviceList)
            {
                wasPlaying = wasPlaying || device.Stop();
            }
            return wasPlaying;
        }

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
            foreach (var device in deviceList)
            {
                device.Dispose();
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
            if (deviceList.Count() > 1)
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

        public List<UserSettingHost> GetHosts()
        {
            var hosts = new List<UserSettingHost>();
            foreach (var device in deviceList)
            {
                hosts.Add(new UserSettingHost { Ip = device.GetHost(), Name = device.GetFriendlyName() });
            }
            return hosts;
        }
    }
}
