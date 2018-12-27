using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Classes;
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

    public class StreamingRequestsListener : IStreamingRequestsListener
    {
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private Action<string, int> onListenCallback;
        private Action<Socket, string> onConnectCallback;
        private Socket listener;

        public void StartListening(IPAddress ipAddress, Action<string, int> onListenCallbackIn, Action<Socket, string> onConnectCallbackIn)
        {
            onListenCallback = onListenCallbackIn;
            onConnectCallback = onConnectCallbackIn;
            var localEndPoint = new IPEndPoint(ipAddress, 0);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                onListenCallback?.Invoke(((IPEndPoint)listener.LocalEndPoint).Address.ToString(), ((IPEndPoint)listener.LocalEndPoint).Port);

                while (true)
                {
                    allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

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

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            allDone.Set();

            var listener = (Socket)asyncResult.AsyncState;
            try
            {
                var handlerSocket = listener.EndAccept(asyncResult);
                var state = new StateObject { workSocket = handlerSocket };

                state.buffer = new byte[StateObject.bufferSize];
                handlerSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void ReadCallback(IAsyncResult asyncResult)
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
    }
}