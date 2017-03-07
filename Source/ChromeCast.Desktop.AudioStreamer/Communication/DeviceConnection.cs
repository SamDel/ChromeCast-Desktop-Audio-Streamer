using System;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceConnection : IDeviceConnection
    {
        private Func<string> getHost;
        private Action<DeviceState, string> setDeviceState;
        private Action<CastMessage> onReceiveMessage;
        private ILogger logger;
        private IDeviceReceiveBuffer deviceReceiveBuffer;
        private const int bufferSize = 2048;
        private TcpClient tcpClient;
        private SslStream sslStream;
        private byte[] receiveBuffer;
        private DeviceConnectionState state;

        public DeviceConnection(ILogger loggerIn, IDeviceReceiveBuffer deviceReceiveBufferIn)
        {
            logger = loggerIn;
            deviceReceiveBuffer = deviceReceiveBufferIn;
            deviceReceiveBuffer.SetCallback(OnReceiveMessage);
        }

        private void Connect()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                var host = getHost?.Invoke();
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
                    setDeviceState?.Invoke(DeviceState.ConnectError, null);
                    logger.Log(string.Format("ex [{0}]: {1}", host, ex.Message));
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

        private void OnReceiveMessage(CastMessage castMessage)
        {
            onReceiveMessage?.Invoke(castMessage);
        }

        public void Dispose()
        {
            tcpClient?.Close();
            sslStream?.Close();
        }

        public bool DontValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void SetCallback(Func<string> getHostIn, Action<DeviceState, string> setDeviceStateIn, Action<CastMessage> onReceiveMessageIn)
        {
            getHost = getHostIn;
            setDeviceState = setDeviceStateIn;
            onReceiveMessage = onReceiveMessageIn;
        }
    }
}
