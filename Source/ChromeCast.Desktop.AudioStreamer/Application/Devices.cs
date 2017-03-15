using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using Rssdp;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Classes;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Devices : IDevices
    {
        private List<IDevice> deviceList = new List<IDevice>();
        private Action<Device> onAddDeviceCallback;
        private bool AutoStart;

        public void OnDeviceAvailable(DiscoveredSsdpDevice discoveredSsdpDevice, SsdpDevice ssdpDevice)
        {
            AddDevice(discoveredSsdpDevice, ssdpDevice);
        }

        private void AddDevice(DiscoveredSsdpDevice device, SsdpDevice fullDevice)
        {
            if (!deviceList.Any(d => d.GetUsn().Equals(device.Usn)))
            {
                var newDevice = DependencyFactory.Container.Resolve<Device>();
                newDevice.SetDiscoveredDevices(device, fullDevice);
                deviceList.Add(newDevice);
                onAddDeviceCallback?.Invoke(newDevice);

                if (AutoStart)
                    newDevice.OnClickDeviceButton(null, null);
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

        public void AddStreamingConnection(Socket socket, string httpRequest)
        {
            var remoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            foreach (var device in deviceList)
            {
                if (device.AddStreamingConnection(remoteAddress, socket))
                    break;
            }
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold)
        {
            foreach (var device in deviceList)
            {
                device.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold);
            }
        }

        public void OnGetStatus()
        {
            foreach (var device in deviceList)
            {
                if (device.IsConnected())
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
                device.SetDeviceState(DeviceState.Disposed);
            }
        }

        public void SetCallback(Action<Device> onAddDeviceCallbackIn)
        {
            onAddDeviceCallback = onAddDeviceCallbackIn;
        }
    }
}
