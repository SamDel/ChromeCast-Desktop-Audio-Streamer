using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int bufferSize = 2048;
        public byte[] buffer;
        public StringBuilder receiveBuffer = new StringBuilder();
    }

    public class StreamingRequestsListener : IStreamingRequestsListener, IDisposable
    {
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private Action<Socket, string> onConnectCallback;
        private Socket listener;
        private string ip;
        private int port;
        private ILogger logger;

        public string GetStreamimgUrl()
        {
            return string.Format($"http://{ip}:{port}/");
        }

        /// <summary>
        /// Start listening for new streaming connections.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="onConnectCallbackIn"></param>
        public void StartListening(IPAddress ipAddress, Action<Socket, string> onConnectCallbackIn, ILogger loggerIn)
        {
            if (ipAddress == null || onConnectCallbackIn == null)
                return;

            logger = loggerIn;
            onConnectCallback = onConnectCallbackIn;

            try
            {
                var localEndPoint = new IPEndPoint(ipAddress, 0);
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(100);
                var endPoint = (IPEndPoint)listener.LocalEndPoint;
                if (endPoint != null)
                {
                    ip = endPoint.Address?.ToString();
                    port = endPoint.Port;
                    Console.WriteLine(string.Format("Streaming from {0}:{1}", ip, port));

                    while (true)
                    {
                        allDone.Reset();
                        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                        allDone.WaitOne();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.ToString());
            }
        }

        /// <summary>
        /// Stop listening.
        /// </summary>
        public void StopListening()
        {
            try
            {
                listener.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Accept the connection.
        /// </summary>
        /// <param name="asyncResult"></param>
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            if (asyncResult == null || asyncResult.AsyncState == null)
                return;

            try
            {
                allDone.Set();

                var listener = (Socket)asyncResult.AsyncState;
                var handlerSocket = listener.EndAccept(asyncResult);
                var state = new StateObject { workSocket = handlerSocket };

                state.buffer = new byte[StateObject.bufferSize];
                handlerSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                logger.Log($"ex : {ex.Message}");
            }
        }

        /// <summary>
        /// Read incoming bytes.
        /// </summary>
        private void ReadCallback(IAsyncResult asyncResult)
        {
            if (asyncResult == null || asyncResult.AsyncState == null || onConnectCallback == null)
                return;

            try
            {
                var state = (StateObject)asyncResult.AsyncState;
                var handlerSocket = state.workSocket;

                var bytesRead = handlerSocket.EndReceive(asyncResult);
                if (bytesRead > 0)
                {
                    state.receiveBuffer.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    if (state.receiveBuffer.ToString().IndexOf("\r\n\r\n") >= 0)
                    {
                        onConnectCallback?.Invoke(handlerSocket, state.receiveBuffer.ToString());
                    }
                    else
                    {
                        // Not all data received. Get more.  
                        handlerSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log($"ex : {ex.Message}");
            }
        }

        /// <summary>
        /// Dispose.
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
            if (allDone != null)
                allDone.Dispose();
            if (listener != null)
                listener.Dispose();
        }
    }
}