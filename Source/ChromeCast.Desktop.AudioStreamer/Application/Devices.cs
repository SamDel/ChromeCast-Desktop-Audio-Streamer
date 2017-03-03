using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using Rssdp;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Communication;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Devices
    {
        private List<Device> deviceList = new List<Device>();
        private ApplicationLogic application;

        public Devices(ApplicationLogic app)
        {
            application = app;
        }

        public int CountDiscovered()
        {
            return deviceList.Where(d => d.SsdpDevice.Uuid != null).ToList().Count;
        }

        public void AddDevice(DiscoveredSsdpDevice device, SsdpDevice fullDevice)
        {
            if (!deviceList.Any(d => d.DiscoveredSsdpDevice.Usn != null && d.DiscoveredSsdpDevice.Usn.Equals(device.Usn)))
            {
                var newDevice = new Device(application, device, fullDevice);
                deviceList.Add(newDevice);
                application.OnAddDevice(newDevice);
                newDevice.SetDeviceState(newDevice.DeviceState);

                if (application.AutoStart)
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

        public void VolumeSet(float level)
        {
            foreach (var device in deviceList)
            {
                device.VolumeSet(level);
            }
        }

        public void AddStreamingConnection(Socket socket, string httpRequest)
        {
            var remoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            foreach (var device in deviceList)
            {
                if (device.DiscoveredSsdpDevice.DescriptionLocation.Host.Equals(remoteAddress))
                {
                    var streamingConnection = new StreamingConnection(device, socket, 
                        CastDeviceCapabilitiesHelper.GetCastDeviceCapabilities(httpRequest));
                    device.StreamingConnection = streamingConnection;
                    streamingConnection.SendStartStreamingResponse();
                    break;
                }
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

        public void Dispose()
        {
            foreach (var device in deviceList)
            {
                device.SetDeviceState(DeviceState.Disposed);
                if (device.StreamingConnection != null && device.StreamingConnection.Socket != null)
                {
                    device.StreamingConnection.Socket.Disconnect(false);
                }
            }
        }
    }
}
