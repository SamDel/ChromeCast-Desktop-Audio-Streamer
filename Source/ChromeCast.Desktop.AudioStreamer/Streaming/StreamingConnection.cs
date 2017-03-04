using System;
using System.Text;
using System.Net.Sockets;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class StreamingConnection
    {
        public Socket Socket;
        public CastDeviceCapabilities CastDeviceCapabilities;
        private Device device;
        private bool isRiffHeaderSent;
        private long bytesSendAfterHeader;
        private int reduceLagCounter = 0;

        public StreamingConnection(Device deviceIn, Socket socket, CastDeviceCapabilities capabilities)
        {
            this.device = deviceIn;
            Socket = socket;
            CastDeviceCapabilities = capabilities;
            isRiffHeaderSent = false;
        }

        public void SendData(byte[] dataToSend, WaveFormat format)
        {
            if (!isRiffHeaderSent)
            {
                isRiffHeaderSent = true;
                Send(new Riff().GetRiffHeader(format));

                if (bytesSendAfterHeader > uint.MaxValue)
                    bytesSendAfterHeader = bytesSendAfterHeader - uint.MaxValue;
            }
            Send(dataToSend);
            bytesSendAfterHeader += dataToSend.Length;
        }

        public void Send(byte[] data)
        {
            if (Socket != null && Socket.Connected)
            {
                try
                {
                    Socket.Send(data);
                }
                catch (Exception)
                {
                }
            }
        }

        public bool IsMaxWavSizeReached(int length)
        {
            if (bytesSendAfterHeader + length > uint.MaxValue)
                return true;

            return false;
        }

        public void SendStartStreamingResponse()
        {
            var startStreamingResponse = Encoding.ASCII.GetBytes(GetStartStreamingResponse());
            Send(startStreamingResponse);
        }

        private string GetStartStreamingResponse()
        {
            var httpStartStreamingReply = new StringBuilder();

            httpStartStreamingReply.Append("HTTP/1.0 200 OK\r\n");
            httpStartStreamingReply.Append("Content-Disposition: inline; filename=\"stream.wav\"\r\n");
            httpStartStreamingReply.Append("Content-Type: audio/wav\r\n");
            httpStartStreamingReply.Append("Connection: keep-alive\r\n");
            httpStartStreamingReply.Append("\r\n");

            return httpStartStreamingReply.ToString();
        }
    }
}
