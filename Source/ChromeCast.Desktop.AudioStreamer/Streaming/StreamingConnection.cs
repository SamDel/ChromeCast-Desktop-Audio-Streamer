using System;
using System.Text;
using System.Net.Sockets;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class StreamingConnection : IStreamingConnection
    {
        private Socket Socket;
        private IAudioHeader audioHeader;
        private bool isAudioHeaderSent;
        private int reduceLagCounter = 0;

        public StreamingConnection(IAudioHeader audioHeaderIn)
        {
            audioHeader = audioHeaderIn;
            isAudioHeaderSent = false;
        }

        public void SendData(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            if (reduceLagThreshold < 1000)
            {
                reduceLagCounter++;
                if (reduceLagCounter > reduceLagThreshold)
                {
                    reduceLagCounter = 0;
                    return;
                }
            }

            if (!isAudioHeaderSent)
            {
                isAudioHeaderSent = true;
                if (streamFormat.Equals(SupportedStreamFormat.Wav))
                {
                    Send(audioHeader.GetRiffHeader(format));
                }
                else
                {
                    Send(audioHeader.GetMp3Header(format, streamFormat));
                }
            }

            Send(dataToSend);
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

        public bool IsConnected()
        {
            return Socket != null && Socket.Connected;
        }

        public string GetRemoteEndPoint()
        {
            if (Socket == null)
                return string.Empty;

            return Socket.RemoteEndPoint.ToString();
        }

        public void SetSocket(Socket socketIn)
        {
            Socket = socketIn;
        }
    }
}
