using System;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
        private ushort Port = 8009;
        private IAsyncResult currentAynchResult;
        private bool connecting = false;
        private byte[] sendBuffer;

        public DeviceConnection(ILogger loggerIn, IDeviceReceiveBuffer deviceReceiveBufferIn)
        {
            logger = loggerIn;
            deviceReceiveBuffer = deviceReceiveBufferIn;
            deviceReceiveBuffer.SetCallback(OnReceiveMessage);
        }

        private void Connect()
        {
            if (connecting)
                return;

            try
            {
                if (!(tcpClient != null &&
                    tcpClient.Client != null &&
                    tcpClient.Connected &&
                    state == DeviceConnectionState.Connected))
                {
                    connecting = true;
                    var host = getHost();
                    state = DeviceConnectionState.Connecting;

                    tcpClient = new TcpClient();
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    currentAynchResult = tcpClient.BeginConnect(host, Port, new AsyncCallback(ConnectCallback), tcpClient);
                    WaitHandle wh = currentAynchResult.AsyncWaitHandle;
                    try
                    {
                        if (!currentAynchResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), false))
                        {
                            tcpClient.Close();
                            throw new TimeoutException();
                        }
                    }
                    finally
                    {
                        wh.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    state = DeviceConnectionState.Error;
                    setDeviceState?.Invoke(DeviceState.ConnectError, null);
                    var host = getHost?.Invoke();
                    logger.Log($"ex [{host}]: {ex.Message}");
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Connect:{innerEx.Message}");
                }
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (ar == currentAynchResult)
                {
                    connecting = false;
                    tcpClient.EndConnect(ar);
                    sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(DontValidateServerCertificate), null);
                    var host = getHost?.Invoke();
                    sslStream.AuthenticateAsClient(host, new X509CertificateCollection(), SslProtocols.Tls12, false);
                    StartReceive();
                    DoSendMessage();
                    state = DeviceConnectionState.Connected;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    state = DeviceConnectionState.Error;
                    setDeviceState?.Invoke(DeviceState.ConnectError, null);
                    var host = getHost?.Invoke();
                    logger.Log($"ex [{host}]: {ex.Message}");
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"ConnectCallback:{innerEx.Message}");
                }
            }
        }

        public bool IsConnected()
        {
            return state.Equals(DeviceConnectionState.Connected);
        }

        public void SendMessage(byte[] send)
        {
            sendBuffer = send;
            if (tcpClient != null &&
                tcpClient.Client != null &&
                tcpClient.Connected &&
                state == DeviceConnectionState.Connected)
                DoSendMessage();
            else
                Connect();
        }

        private void DoSendMessage()
        {
            if (tcpClient != null && 
                tcpClient.Client != null && 
                tcpClient.Connected &&
                state == DeviceConnectionState.Connected &&
                sendBuffer != null)
            {
                try
                {
                    sslStream.Write(sendBuffer);
                    sslStream.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DoSendMessage: {ex.Message}");
                }
                finally
                {
                    sendBuffer = null;
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
                Console.WriteLine($"StartReceive: {ex.Message}");
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

        public void SetPort(ushort portIn)
        {
            Port = portIn;
        }

        public ushort GetPort()
        {
            return Port;
        }
    }
}
