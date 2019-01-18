using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Rssdp;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Streaming;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Device : IDevice
    {
        private IDeviceCommunication deviceCommunication;
        private IStreamingConnection streamingConnection;
        private DiscoveredSsdpDevice discoveredSsdpDevice;
        private SsdpDevice ssdpDevice;
        private DeviceControl deviceControl;
        private MenuItem menuItem;

        public Device(IDeviceCommunication deviceCommunicationIn)
        {
            deviceCommunication = deviceCommunicationIn;
            deviceCommunication.SetCallback(SetDeviceState, OnVolumeUpdate, GetHost);
        }

        public void SetDiscoveredDevices(DiscoveredSsdpDevice discoveredSsdpDeviceIn, SsdpDevice ssdpDeviceIn)
        {
            discoveredSsdpDevice = discoveredSsdpDeviceIn;
            ssdpDevice = ssdpDeviceIn;
        }

        public void OnClickPlayPause()
        {
            deviceCommunication.OnPlayPause_Click();
        }

        public void Start()
        {
            deviceCommunication.LoadMedia();
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            if (streamingConnection != null)
            {
                if (streamingConnection.IsConnected())
                {
                    var state = deviceCommunication.GetDeviceState();
                    if (state != DeviceState.Paused && state != DeviceState.Closed)
                    {
                        streamingConnection.SendData(dataToSend, format, reduceLagThreshold, streamFormat);

                        //TODO: updates only the shown state, not the communication state!?
                        // After a close message the stream continues sometimes. Keep the device control green then.
                        if (state != DeviceState.Buffering && state != DeviceState.Playing)
                            SetDeviceState(DeviceState.Playing, "");
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Connection closed from {0}", streamingConnection.GetRemoteEndPoint()));
                    streamingConnection = null;
                }
            }
        }

        public void OnGetStatus()
        {
            deviceCommunication.GetMediaStatus();
        }

        public void SetDeviceState(DeviceState state, string text = null)
        {
            deviceControl?.SetStatus(state, text);

            if (state == DeviceState.Buffering || state == DeviceState.Playing)
                menuItem.Checked = true;
            else
                menuItem.Checked = false;

            if (state == DeviceState.Disposed)
                Stop(); //TODO: implement Close!?
        }

        private void OnVolumeUpdate(Volume volume)
        {
            deviceControl?.OnVolumeUpdate(volume);
        }

        public void VolumeSet(float level)
        {
            deviceCommunication.VolumeSet(level);
        }

        public void VolumeUp()
        {
            deviceCommunication.VolumeUp();
        }

        public void VolumeDown()
        {
            deviceCommunication.VolumeDown();
        }

        public void VolumeMute()
        {
            deviceCommunication.VolumeMute();
        }

        public bool Stop()
        {
            return deviceCommunication.Stop();
        }

        public bool AddStreamingConnection(string remoteAddress, Socket socket)
        {
            if (discoveredSsdpDevice.DescriptionLocation.Host.Equals(remoteAddress))
            {
                streamingConnection = DependencyFactory.Container.Resolve<StreamingConnection>();
                streamingConnection.SetSocket(socket);
                streamingConnection.SendStartStreamingResponse();
                return true;
            }

            return false;
        }

        public string GetUsn()
        {
            if (discoveredSsdpDevice != null)
                return discoveredSsdpDevice.Usn;

            return string.Empty;
        }

        public string GetHost()
        {
            if (discoveredSsdpDevice != null)
                return discoveredSsdpDevice.DescriptionLocation.Host;

            return string.Empty;
        }

        public string GetFriendlyName()
        {
            if (ssdpDevice != null)
                return ssdpDevice.FriendlyName;

            return string.Empty;
        }

        public void SetDeviceName(string name)
        {
            deviceControl?.SetDeviceName(name);
            menuItem.Text = name;
        }

        public void SetDeviceControl(DeviceControl deviceControlIn)
        {
            deviceControl = deviceControlIn;
            deviceControl.SetClickCallBack(OnClickPlayPause);
        }

        public void SetMenuItem(MenuItem menuItemIn)
        {
            menuItem = menuItemIn;
            menuItem.Click += OnClickMenuItem;
        }

        private void OnClickMenuItem(object sender, EventArgs e)
        {
            OnClickPlayPause();
        }

        public IDeviceConnection GetDeviceConnection()
        {
            throw new NotImplementedException();
        }

    }
}
