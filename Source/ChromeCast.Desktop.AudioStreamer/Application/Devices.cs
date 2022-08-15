﻿using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Devices : IDevices
    {
        private readonly List<IDevice> deviceList = new List<IDevice>();
        private Action<Device> onAddDeviceCallback;
        private bool AutoStart;
        private bool StartLastUsedDevices;
        private IMainForm mainForm;
        private IApplicationLogic applicationLogic;
        private readonly ApplicationBuffer applicationBuffer = new ApplicationBuffer();
        private readonly ILogger logger;
        private bool isMuted;
        private List<string> ignoreIpAddresses;

        public Devices()
        {

        }

        public Devices(Logger loggerIn)
        {
            logger = loggerIn;
        }

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

            if (ignoreIpAddresses.Contains(discoveredDevice.IPAddress))
                return;

            if (!discoveredDevice.AddedByDeviceInfo && !discoveredDevice.IsGroup)
            {
                applicationLogic.StartTask(DeviceInformation.GetDeviceInformation(discoveredDevice, SetDeviceInformation, logger));
            }
            else
            {
                lock(deviceList)
                {
                    var existingDevice = GetDevice(discoveredDevice);
                    if (existingDevice == null)
                    {
                        var newDevice = new Device(logger, applicationLogic);
                        newDevice.Initialize(discoveredDevice, SetDeviceInformation, StopGroup, applicationLogic.StartTask, IsGroupStatusBlank, AutoMute);
                        deviceList.Add(newDevice);
                        onAddDeviceCallback?.Invoke(newDevice);

                        var wasPlaying = applicationLogic.WasPlaying(discoveredDevice);
                        if ((AutoStart && !newDevice.IsGroup()) || (StartLastUsedDevices && wasPlaying))
                            newDevice.ResumePlaying();
                    }
                    else
                    {
                        existingDevice.Initialize(discoveredDevice, SetDeviceInformation, StopGroup, applicationLogic.StartTask, IsGroupStatusBlank, AutoMute);
                    }
                }
            }
        }

        /// <summary>
        /// If device is a group, stop devices in the group.
        /// If device is a device, stop the groups the device is in.
        /// </summary>
        private void StopGroup(IDevice deviceIn)
        {
            if (deviceIn == null)
                return;

            if (deviceIn.IsGroup())
            {
                StopGroupDevices(deviceIn);
            }
            else
            {
                StopGroups(deviceIn, true);
            }
        }

        /// <summary>
        /// Stop all groups a device is in.
        /// </summary>
        private void StopGroups(IDevice deviceIn, bool change)
        {
            if (deviceList == null || deviceIn == null)
                return;

            var eurekaIn = deviceIn.GetEureka();
            if (eurekaIn == null || eurekaIn?.Multizone?.Groups == null)
                return;

            foreach (var group in eurekaIn.Multizone.Groups)
            {
                // Stop all devices in the group.
                foreach (var device in deviceList)
                {
                    if (group.Name == device.GetFriendlyName())
                    {
                        device.Stop(true && change);
                    }
                }
            }
        }

        /// <summary>
        /// Stop all devices in a group.
        /// </summary>
        private void StopGroupDevices(IDevice deviceIn)
        {
            if (deviceList == null || deviceIn == null)
                return;

            foreach (var device in deviceList)
            {
                // Check if this device is in the device-group that has to stop.
                var deviceEureka = device.GetEureka();
                if (deviceEureka == null || deviceEureka?.Multizone?.Groups == null)
                    continue;

                foreach (var group in deviceEureka.Multizone.Groups)
                {
                    if (group.Name == deviceIn.GetFriendlyName())
                    {
                        device.Stop(false);
                        StopGroups(device, false);
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
                return deviceList.FirstOrDefault(d => d.GetDiscoveredDevice()?.Id == discoveredDevice.Id);
            else
                return deviceList.FirstOrDefault(d => d.GetDiscoveredDevice()?.Eureka?.GetMacAddress() == discoveredDevice.Eureka?.GetMacAddress());
        }

        /// <summary>
        /// Callback for when the device information is collected.
        /// </summary>
        /// <param name="eurekaIn"></param>
        private void SetDeviceInformation(DeviceEureka eurekaIn)
        {
            var discoveredDevice = new DiscoveredDevice
            {
                IPAddress = eurekaIn.GetIpAddress(),
                MACAddress = eurekaIn.GetMacAddress(),
                Name = eurekaIn.GetName(),
                Port = 8009,
                Protocol = "",
                Usn = null,
                IsGroup = false,
                AddedByDeviceInfo = true,
                Eureka = eurekaIn
            };
            OnDeviceAvailable(discoveredDevice);
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
        public void Stop(bool changeUserMode = false)
        {
            if (deviceList == null || applicationBuffer == null)
                return;

            foreach (var device in deviceList)
            {
                device.Stop(changeUserMode);
            }
            applicationBuffer.ClearBuffer();
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
        public void AddStreamingConnection(Socket socket, string httpRequest, SupportedStreamFormat streamFormatIn)
        {
            if (deviceList == null || socket == null || applicationBuffer == null)
                return;

            var remoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            foreach (var device in deviceList)
            {
                if (device.AddStreamingConnection(remoteAddress, socket))
                {
                    applicationBuffer.SendStartupBuffer(device, streamFormatIn);
                    break;
                }
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
            if (deviceList == null || dataToSend == null || applicationBuffer == null)
                return;

            if (applicationBuffer.IsStartBufferSend())
            {
                foreach (var device in deviceList)
                {
                    device.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold, streamFormat);
                }
            }

            // Keep a buffer
            applicationBuffer.AddToBuffer(dataToSend, format, reduceLagThreshold, streamFormat);
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
                lock(deviceList)
                {
                    for (int i = deviceList.Count - 1; i >= 0; i--)
                    {
                        if (deviceList[i].GetDeviceState() == DeviceState.Disposed)
                            deviceList.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex, "Devices.OnGetStatus");
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
        public void SetSettings(UserSettings settingsIn)
        {
            if (settingsIn == null)
                return;

            AutoStart = settingsIn.AutoStartDevices ?? false;
            StartLastUsedDevices = settingsIn.StartLastUsedDevices ?? false;
            applicationBuffer.SetExtraBufferInSeconds(settingsIn.ExtraBufferInSeconds ?? 4);
        }

        /// <summary>
        /// Dispose all devices.
        /// </summary>
        public void Dispose()
        {
            if (deviceList == null)
                return;

            try
            {
                Stop(true);
                for (int i = deviceList.Count - 1; i >= 0; i--)
                {
                    var device = deviceList[i];
                    device?.SetDeviceState(DeviceState.Disposed);
                    device?.Dispose();
                }
                deviceList.Clear();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// The devices filter has changed.
        /// </summary>
        /// <param name="value">new filter value</param>
        public void SetFilterDevices(FilterDevicesEnum value)
        {
            if (deviceList == null)
                return;

            foreach (var device in deviceList)
            {
                if (device == null ||device.GetDeviceControl() == null || device.GetDeviceControl().IsDisposed)
                    continue;

                device.GetDeviceControl().Visible = FilterDevices.ShowFilterDevices(device, value);
            }
        }

        /// <summary>
        /// Set the device buffer in seconds.
        /// </summary>
        /// <param name="bufferInSeconds">the buffer in seconds</param>
        public void SetExtraBufferInSeconds(int bufferInSeconds)
        {
            applicationBuffer.SetExtraBufferInSeconds(bufferInSeconds);
        }

        /// <summary>
        /// Check if the status text of all devices in a group is blank.
        /// </summary>
        /// <param name="deviceIn">the group device</param>
        private bool IsGroupStatusBlank(IDevice deviceIn)
        {
            if (deviceList == null || deviceIn == null)
                return true;

            // Check the status text of all devices in the group.
            foreach (var device in deviceList)
            {
                var eureka = device.GetEureka();
                if (eureka == null || eureka?.Multizone?.Groups == null)
                    return true;

                foreach (var group in eureka.Multizone.Groups)
                {
                    if (group.Name == deviceIn.GetFriendlyName())
                    {
                        var statusText = device.GetStatusText();
                        if (!device.IsStatusTextBlankCheck(statusText))
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Automute the system volume.
        /// Mute when one device is playng, Unmute when no one is playing.
        /// </summary>
        public void AutoMute(bool playing)
        {
            applicationLogic.StartTask(() =>
            {
                if (mainForm.GetAutoMute())
                {
                    if (playing)
                    {
                        SystemVolume.Mute(true, mainForm);
                        isMuted = true;
                    }
                    else if (!playing && !IsAnyDevicePlaying() && isMuted)
                    {
                        SystemVolume.Mute(false, mainForm);
                        isMuted = false;
                    }
                }
            });
        }

        private bool IsAnyDevicePlaying()
        {
            foreach (var device in deviceList)
            {
                if (device.GetUserMode() == UserMode.Playing)
                    return true;
            }

            return false;
        }

        public List<IDevice> GetDeviceList()
        {
            return deviceList;
        }

        public void SetIgnoreIpAddresses(string ignoreIpAddressesDevicesIn)
        {
            ignoreIpAddresses = new List<string>();
            if (!string.IsNullOrWhiteSpace(ignoreIpAddressesDevicesIn))
            {
                ignoreIpAddresses.AddRange(ignoreIpAddressesDevicesIn.Split(';'));
            }
        }
    }
}
