using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int bufferSize = 2048;
        public byte[] buffer;
        public StringBuilder receiveBuffer = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private ApplicationLogic application;
        private Socket listener;

        public AsynchronousSocketListener(ApplicationLogic app)
        {
            application = app;
        }

        public void StartListening()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = GetIp4Address(ipHostInfo);
            var localEndPoint = new IPEndPoint(ipAddress, 0);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                application.OnListen(((IPEndPoint)listener.LocalEndPoint).Address.ToString(), ((IPEndPoint)listener.LocalEndPoint).Port);

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
                    application.OnStreamRequestConnect(handlerSocket, state.receiveBuffer.ToString());
                }
                else
                {
                    // Not all data received. Get more.  
                    handlerSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private IPAddress GetIp4Address(IPHostEntry ipHostInfo)
        {
            var ipAddress = ipHostInfo.AddressList[0];
            foreach (var address in ipHostInfo.AddressList)
            {
                if (address.AddressFamily.Equals(AddressFamily.InterNetwork))
                    ipAddress = address;
            }

            return ipAddress;
        }
    }
}