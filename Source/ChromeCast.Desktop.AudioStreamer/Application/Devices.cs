using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Classes;
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

            if (discoveredDevice.Port == 0 || discoveredDevice.Port == 10001)
                return;

            if (!discoveredDevice.AddedByDeviceInfo)
            {
                if (!discoveredDevice.IsGroup)
                    DeviceInformation.GetDeviceInformation(discoveredDevice, SetDeviceInformation);
            }
            else
            {
                lock(deviceList)
                {
                    var existingDevice = GetDevice(discoveredDevice);
                    if (existingDevice == null)
                    {
                        var newDevice = DependencyFactory.Container.Resolve<Device>();
                        newDevice.Initialize(discoveredDevice, SetDeviceInformation);
                        deviceList.Add(newDevice);
                        onAddDeviceCallback?.Invoke(newDevice);

                        if (AutoStart && !newDevice.IsGroup())
                            newDevice.ResumePlaying();
                    }
                    else
                    {
                        existingDevice.Initialize(discoveredDevice, SetDeviceInformation);
                    }
                }
            }
        }

        /// <summary>
        /// Get the device with the IP and port.
        /// </summary>
        /// <returns></returns>
        private IDevice GetDevice(DiscoveredDevice discoveredDevice)
        {
            if (discoveredDevice == null)
                return null;

            if (discoveredDevice.IsGroup)
                return deviceList.FirstOrDefault(d => d.GetDiscoveredDevice()?.Group?.Uuid == discoveredDevice.Group?.Uuid);
            else
                return deviceList.FirstOrDefault(d => d.GetDiscoveredDevice()?.Eureka?.DeviceInfo?.Mac_address == discoveredDevice.Eureka?.DeviceInfo?.Mac_address);
        }

        /// <summary>
        /// Callback for when the device information is collected.
        /// </summary>
        /// <param name="eurekaIn"></param>
        private void SetDeviceInformation(DeviceEureka eurekaIn)
        {
            if (eurekaIn?.Multizone?.Groups == null)
                return;

            var discoveredDevice = new DiscoveredDevice
            {
                IPAddress = eurekaIn.Net.Ip_address,
                Name = eurekaIn.Name,
                Port = 8009,
                Protocol = "",
                Usn = null,
                IsGroup = false,
                AddedByDeviceInfo = true,
                Eureka = eurekaIn
            };
            OnDeviceAvailable(discoveredDevice);

            foreach (var group in eurekaIn.Multizone.Groups)
            {
                discoveredDevice = new DiscoveredDevice
                {
                    IPAddress = GetIpOfGroup(group, eurekaIn),
                    Name = group.Name,
                    Port = GetPortOfGroup(group, eurekaIn),
                    Protocol = "",
                    Usn = null,
                    IsGroup = true,
                    AddedByDeviceInfo = true,
                    Eureka = eurekaIn,
                    Group = group
                };

                // Add the group.
                if (group.Elected_leader == "self")
                {
                    OnDeviceAvailable(discoveredDevice);
                }

                // Get device information from unknown devices.
                if (!deviceList.Any(x => x.GetHost() == GetIpOfGroup(group, eurekaIn)))
                {
                    DeviceInformation.GetDeviceInformation(discoveredDevice, SetDeviceInformation);
                }
            }
        }

        private string GetIpOfGroup(Group group, DeviceEureka eurekaIn)
        {
            if (group.Elected_leader == null || group.Elected_leader == "self" || group.Elected_leader.IndexOf(":") < 0)
                return eurekaIn.Net.Ip_address;

            return group.Elected_leader.Substring(0, group.Elected_leader.IndexOf(":"));
        }

        private int GetPortOfGroup(Group group, DeviceEureka eurekaIn)
        {
            if (group.Elected_leader == null || group.Elected_leader == "self" || group.Elected_leader.IndexOf(":") < 0)
                return group.Cast_port;

            if (int.TryParse(group.Elected_leader.Substring(group.Elected_leader.IndexOf(":") + 1), out int result))
                return result;

            return 0;
        }

        /// <summary>
        /// Volume up for all devices.
        /// </summary>
        public void VolumeUp()
        {
            if (deviceList == null)
                return;

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
            if (deviceList == null)
                return;

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
            if (deviceList == null)
                return;

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
            if (deviceList == null)
                return;

            foreach (var device in deviceList)
            {
                device.Stop();
            }
        }

        /// <summary>
        /// Start all devices.
        /// </summary>
        public void Start()
        {
            if (deviceList == null)
                return;

            foreach (var device in deviceList)
            {
                device.Start();
            }
        }

        /// <summary>
        /// A device has made a new streaming connection. Add the connection to the right device.
        /// </summary>
        /// <param name="socket">the socket</param>
        /// <param name="httpRequest">the HTTP headers, including the 'CAST-DEVICE-CAPABILITIES' header</param>
        public void AddStreamingConnection(Socket socket, string httpRequest)
        {
            if (deviceList == null || socket == null)
                return;

            var remoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            foreach (var device in deviceList)
            {
                if (device.AddStreamingConnection(remoteAddress, socket))
                    break;
            }
        }

        /// <summary>
        /// New audio data is available.
        /// </summary>
        /// <param name="dataToSend">the data</param>
        /// <param name="format">the wav format that's used</param>
        /// <param name="reduceLagThreshold">value for the lag control</param>
        /// <param name="streamFormat">the stream format</param>
        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            if (deviceList == null || dataToSend == null)
                return;

            foreach (var device in deviceList)
            {
                device.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold, streamFormat);
            }
        }

        /// <summary>
        /// Get the status of a device.
        /// Also used to cleanup disposed devices (groups).
        /// </summary>
        public void OnGetStatus()
        {
            if (deviceList == null)
                return;

            // Cleanup disposed devices first.
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

        /// <summary>
        /// Set the value to auto start a device right after it has been added.
        /// </summary>
        /// <param name="autoStartIn"></param>
        public void SetAutoStart(bool autoStartIn)
        {
            AutoStart = autoStartIn;
        }

        /// <summary>
        /// Dispose all devices.
        /// </summary>
        public void Dispose()
        {
            if (deviceList == null)
                return;

            Stop();
            foreach (var device in deviceList)
            {
                device.SetDeviceState(DeviceState.Disposed);
            }
        }

        /// <summary>
        /// Set the callback for when a device is added.
        /// </summary>
        /// <param name="onAddDeviceCallbackIn"></param>
        public void SetCallback(Action<Device> onAddDeviceCallbackIn)
        {
            onAddDeviceCallback = onAddDeviceCallbackIn;
        }

        /// <summary>
        /// Set the objects this class is depending on.
        /// </summary>
        /// <param name="mainFormIn">the main form</param>
        /// <param name="applicationLogicIn">the application logic</param>
        public void SetDependencies(MainForm mainFormIn, IApplicationLogic applicationLogicIn)
        {
            mainForm = mainFormIn;
            applicationLogic = applicationLogicIn;
        }

        /// <summary>
        /// Return a list of all devices.
        /// </summary>
        public List<DiscoveredDevice> GetHosts()
        {
            if (deviceList == null)
                return new List<DiscoveredDevice>();

            var hosts = new List<DiscoveredDevice>();
            foreach (var device in deviceList)
            {
                hosts.Add(device.GetDiscoveredDevice());
            }
            return hosts;
        }
    }
}
