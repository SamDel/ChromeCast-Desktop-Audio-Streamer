using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Discover;
using ChromeCast.Desktop.AudioStreamer.Test.Classes;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using System.Text.Json;

namespace ChromeCast.Desktop.AudioStreamer.Test.Application
{
    [TestClass]
    public class DevicesTest
    {
        AutoResetEvent asyncEvent;
        Device device;

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestDevices()
        {
            asyncEvent = new AutoResetEvent(false);

            // Add a device
            var devices = new Devices();
            devices.SetCallback(OnAddDeviceCallback);
            var discoveredDevice = new DiscoveredDevice
            {
                IPAddress = "192.168.111.111",
                Usn = "usn",
                Name = "Device_Name",
                Port = 8009
            };
            devices.OnDeviceAvailable(discoveredDevice);

            asyncEvent.WaitOne(100);

            Assert.AreEqual(discoveredDevice.Name, device.GetFriendlyName());
            Assert.AreEqual(discoveredDevice.Usn, device.GetUsn());
            Assert.AreEqual(discoveredDevice.IPAddress, device.GetHost());

            // Not connected, volume messages are not send.
            TestMessagesWhenNotConnected(devices);

            // Connect and launch app
            TestConnectAndLoadMedia(devices);

            // Test the other messages
            TestMessages(devices);
        }

        private void TestConnectAndLoadMedia(Devices devices)
        {
            var testDeviceConnection = (TestDeviceConnection)device.GetDeviceConnection();

            // Connect & Launch
            device.OnClickPlayPause(null, null);

            // Connect & Load Media
            device.OnReceiveMessage(ReceiverStatusMessageFromDevice());

            Assert.AreEqual(4, testDeviceConnection.messagesSend.Count);
            Assert.AreEqual("CONNECT", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[0].PayloadUtf8).type);
            Assert.AreEqual("LAUNCH", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[1].PayloadUtf8).type);
            Assert.AreEqual("CONNECT", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[2].PayloadUtf8).type);
            Assert.AreEqual("LOAD", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[3].PayloadUtf8).type);
        }

        private void TestMessages(Devices devices)
        {
            var testDeviceConnection = (TestDeviceConnection)device.GetDeviceConnection();
            asyncEvent = new AutoResetEvent(false);

            testDeviceConnection.messagesSend.Clear();
            devices.VolumeUp();
            asyncEvent.WaitOne(1000);
            devices.VolumeDown();
            devices.VolumeMute();
            devices.OnGetStatus();

            Assert.AreEqual(4, testDeviceConnection.messagesSend.Count);
            Assert.AreEqual("SET_VOLUME", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[0].PayloadUtf8).type);
            Assert.AreEqual(0.3f, GetMessage<MessageVolume>(testDeviceConnection.messagesSend[0].PayloadUtf8).volume.level);
            Assert.AreEqual("SET_VOLUME", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[1].PayloadUtf8).type);
            Assert.AreEqual(0.25f, GetMessage<MessageVolume>(testDeviceConnection.messagesSend[1].PayloadUtf8).volume.level);
            Assert.AreEqual("SET_VOLUME", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[2].PayloadUtf8).type);
            Assert.AreEqual(true, GetMessage<MessageVolumeMute>(testDeviceConnection.messagesSend[2].PayloadUtf8).volume.muted);
            Assert.AreEqual("GET_STATUS", GetMessage<PayloadMessageBase>(testDeviceConnection.messagesSend[3].PayloadUtf8).type);
        }

        private void TestMessagesWhenNotConnected(Devices devices)
        {
            var testDeviceConnection = (TestDeviceConnection)device.GetDeviceConnection();
            devices.VolumeUp();
            devices.VolumeDown();
            devices.VolumeMute();
            devices.OnGetStatus();
            Assert.AreEqual(0, testDeviceConnection.messagesSend.Count);
        }

        private CastMessage ReceiverStatusMessageFromDevice()
        {
            var receiverStatusMessageFromDevice = new MessageReceiverStatus
            {
                type = "RECEIVER_STATUS",
                status = new ReceiverStatus
                {
                    applications = new List<Communication.Classes.Application> {
                        new Communication.Classes.Application { appId = "CC1AD845", displayName = "test" }
                    },
                    volume = new Volume { controlType = "", level = 0.25f, muted = false, stepInterval = 0.05f }
                },
                requestId = 0
            };
            return new ChromeCastMessages().GetCastMessage(receiverStatusMessageFromDevice, string.Empty);
        }

        private T GetMessage<T>(string message)
        {
            return JsonSerializer.Deserialize<T>(message);
        }

        private void OnAddDeviceCallback(Device deviceIn)
        {
            device = deviceIn;
            asyncEvent.Set();
        }
    }
}
