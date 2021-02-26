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
    public class DeviceConnection : IDeviceConnection, IDisposable
    {
        private Func<string> getHost;
        private Func<int> getPort;
        private Action<DeviceState, string> setDeviceState;
        private Action<CastMessage> onReceiveMessage;
        private Action<Action> startTask;
        private readonly ILogger logger;
        private readonly IDeviceReceiveBuffer deviceReceiveBuffer;
        private const int bufferSize = 2048;
        private TcpClient tcpClient;
        private SslStream sslStream;
        private byte[] receiveBuffer;
        private DeviceConnectionState state;
        private IAsyncResult currentAynchResult;
        private byte[] sendBuffer;
        private bool IsDisposed = false;

        public DeviceConnection(ILogger loggerIn, IDeviceReceiveBuffer deviceReceiveBufferIn)
        {
            logger = loggerIn;
            deviceReceiveBuffer = deviceReceiveBufferIn;
            deviceReceiveBuffer.SetCallback(OnReceiveMessage);
        }

        /// <summary>
        /// Make a connection with the device for the control messages.
        /// </summary>
        private void Connect()
        {
            if (tcpClient != null && tcpClient.Client != null && tcpClient.Connected)
                return;

            Close();

            try
            {
                IsDisposed = false;
                var host = getHost();
                var port = getPort();

                tcpClient = new TcpClient();
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                currentAynchResult = tcpClient.BeginConnect(host, port, new AsyncCallback(ConnectCallback), tcpClient);
                WaitHandle wh = currentAynchResult.AsyncWaitHandle;
                try
                {
                    if (!currentAynchResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), false))
                    {
                        CloseConnection();
                        throw new TimeoutException();
                    }
                }
                finally
                {
                    wh.Close();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    state = DeviceConnectionState.Error;
                    setDeviceState?.Invoke(DeviceState.ConnectError, null);
                    var host = getHost?.Invoke();
                    logger.Log($"ex [{host}]: Connect {ex.Message}");
                    CloseConnection();
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Connect:{innerEx.Message}");
                }
            }
        }

        /// <summary>
        /// Close the connection with the device when it's connected.
        /// </summary>
        private void Close()
        {
            if (tcpClient == null || tcpClient.Client == null || !tcpClient.Connected)
                return;

            try
            {
                if (state != DeviceConnectionState.Connecting)
                {
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connect:{ex.Message}");
            }
        }

        /// <summary>
        /// Setup a ssl stream, start receiving and send pending messages when a connection has been made.
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            if (tcpClient == null)
                return;

            try
            {
                if (ar == currentAynchResult)
                {
                    tcpClient.EndConnect(ar);
                    sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(DontValidateServerCertificate), null);
                    var host = getHost?.Invoke();
                    sslStream.AuthenticateAsClient(host, new X509CertificateCollection(), SslProtocols.Tls12, true);
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
                    logger.Log($"ex [{host}]: ConnectCallback {ex.Message}");
                    CloseConnection();
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"ConnectCallback:{innerEx.Message}");
                }
            }
        }

        /// <summary>
        /// Return true if there's a connection with the device.
        /// </summary>
        public bool IsConnected()
        {
            return state.Equals(DeviceConnectionState.Connected);
        }

        /// <summary>
        /// Send a message to a device. 
        /// If the device is not connected, a connection is made first.
        /// </summary>
        /// <param name="send">the message</param>
        public void SendMessage(byte[] send)
        {
            startTask(() => {
                sendBuffer = send;
                if (tcpClient != null &&
                    tcpClient.Client != null &&
                    tcpClient.Connected &&
                    state == DeviceConnectionState.Connected)
                {
                    DoSendMessage();
                }
                else
                {
                    if (state != DeviceConnectionState.Connecting)
                    {
                        state = DeviceConnectionState.Connecting;
                        Connect();
                    }
                }
            });
        }

        /// <summary>
        /// Do send the message.
        /// </summary>
        private void DoSendMessage()
        {
            if (tcpClient == null || tcpClient.Client == null || !tcpClient.Connected || sendBuffer == null)
                return;

            try
            {
                if (state == DeviceConnectionState.Connected)
                {
                    sslStream.Write(sendBuffer);
                    sslStream.Flush();
                }
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

        /// <summary>
        /// Start receiving messages from the device.
        /// </summary>
        private void StartReceive()
        {
            if (IsDisposed)
                return;

            try
            {
                receiveBuffer = new byte[bufferSize];
                sslStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, DataReceived, sslStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StartReceive: {ex.Message}");
                CloseConnection();
            }
        }

        /// <summary>
        /// Received data from the device.
        /// </summary>
        private void DataReceived(IAsyncResult ar)
        {
            if (ar == null || deviceReceiveBuffer == null || receiveBuffer == null || IsDisposed)
                return;

            SslStream stream = (SslStream)ar.AsyncState;
            int byteCount = -1;
            try
            {
                byteCount = stream.EndRead(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
            if (byteCount > 0)
            {
                deviceReceiveBuffer.OnReceive(receiveBuffer.Take(byteCount).ToArray());
            }
            else
            {
                Thread.Sleep(500);
            }
            StartReceive();
        }

        /// <summary>
        /// Callback for the receive buffer, a complete message is received.
        /// </summary>
        /// <param name="castMessage">the message that's received</param>
        private void OnReceiveMessage(CastMessage castMessage)
        {
            if (IsDisposed)
                return;

            onReceiveMessage?.Invoke(castMessage);
        }

        /// <summary>
        /// Dispose the connection.
        /// </summary>
        private void CloseConnection()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose the connection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        protected virtual void Dispose(bool cleanupAll)
        {
            IsDisposed = true;
            try
            {
                tcpClient?.Close();
                sslStream?.Close();
                sslStream?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dispose:{ex.Message}");
            }
        }

        /// <summary>
        /// Don't validate the ssl certificate.
        /// </summary>
        /// <returns></returns>
        public bool DontValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Set callbacks.
        /// </summary>
        public void SetCallback(Func<string> getHostIn, Func<int> getPortIn, Action<DeviceState, string> setDeviceStateIn, Action<CastMessage> onReceiveMessageIn, Action<Action> startTaskIn)
        {
            getHost = getHostIn;
            getPort = getPortIn;
            setDeviceState = setDeviceStateIn;
            onReceiveMessage = onReceiveMessageIn;
            startTask = startTaskIn;
        }
    }
}
