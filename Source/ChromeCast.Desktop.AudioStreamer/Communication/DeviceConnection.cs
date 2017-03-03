using System;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceConnection
    {
        private const int bufferSize = 2048;
        private TcpClient tcpClient;
        private SslStream sslStream;
        private byte[] receiveBuffer;
        private DeviceReceiveBuffer deviceReceiveBuffer;
        private ApplicationLogic application;
        private DeviceConnectionState state;
        private Device device;
        private string host;

        public DeviceConnection(Device deviceIn, ApplicationLogic app)
        {
            application = app;
            device = deviceIn;
            host = device.DiscoveredSsdpDevice.DescriptionLocation.Host;
            deviceReceiveBuffer = new DeviceReceiveBuffer(device);
        }

        private void Connect()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                try
                {
                    state = DeviceConnectionState.Connecting;

                    tcpClient = new TcpClient();
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    tcpClient.Connect(host, 8009);

                    sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(DontValidateServerCertificate), null);
                    sslStream.AuthenticateAsClient(host, new X509CertificateCollection(), SslProtocols.Tls12, false);
                    StartReceive();

                    state = DeviceConnectionState.Connected;
                }
                catch (Exception ex)
                {
                    state = DeviceConnectionState.Error;
                    device.SetDeviceState(DeviceState.ConnectError);
                    application.Log(string.Format("ex [{0}]: {1}", host, ex.Message));
                }
            }
        }

        public bool IsConnected()
        {
            return state.Equals(DeviceConnectionState.Connected);
        }

        public void SendMessage(byte[] send)
        {
            Connect();

            if (tcpClient != null && tcpClient.Client != null && tcpClient.Connected)
            {
                try
                {
                    sslStream.Write(send);
                    sslStream.Flush();
                }
                catch (Exception)
                {
                }
            }
        }

        private void StartReceive()
        {
            try
            {
                receiveBuffer = new byte[bufferSize];
                sslStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, DataReceived, sslStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Dispose();
            }
        }

        private void DataReceived(IAsyncResult ar)
        {
            SslStream stream = (SslStream)ar.AsyncState;
            int byteCount = -1;
            try
            {
                byteCount = stream.EndRead(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Dispose();
            }
            if (byteCount > 0)
            {
                deviceReceiveBuffer.OnReceive(receiveBuffer.Take(byteCount).ToArray());
            }
            StartReceive();
        }


        public void Dispose()
        {
            if (tcpClient != null) tcpClient.Close();
            if (sslStream != null) sslStream.Close();
        }

        public bool DontValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
