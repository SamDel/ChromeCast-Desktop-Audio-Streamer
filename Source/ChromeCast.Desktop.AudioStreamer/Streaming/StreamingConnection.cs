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

        /// <summary>
        /// Send audio data to the device.
        /// </summary>
        /// <param name="dataToSend">the audio data</param>
        /// <param name="format">the audio format</param>
        /// <param name="reduceLagThreshold">lag control value</param>
        /// <param name="streamFormat">the stream format selected</param>
        public void SendData(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            if (dataToSend == null || dataToSend.Length == 0 || format == null)
                return;

            // Lag control functionality.
            if (reduceLagThreshold < 1000)
            {
                reduceLagCounter++;
                if (reduceLagCounter > reduceLagThreshold)
                {
                    reduceLagCounter = 0;
                    return;
                }
            }

            // Send audio header before the fitrst data.
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

        /// <summary>
        /// Send data.
        /// </summary>
        public void Send(byte[] data)
        {
            if (!IsConnected())
                return;

            try
            {
                Socket.Send(data);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Send the HTTP header.
        /// </summary>
        public void SendStartStreamingResponse()
        {
            var startStreamingResponse = Encoding.ASCII.GetBytes(GetStartStreamingResponse());
            Send(startStreamingResponse);
        }

        /// <summary>
        /// Return the HTTP header.
        /// </summary>
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

        /// <summary>
        /// Is the socket connected?
        /// </summary>
        /// <returns>true if connected, or false</returns>
        public bool IsConnected()
        {
            return Socket != null && Socket.Connected;
        }

        //TODO: Poll this function and change device state.
        private bool Poll()
        {
            return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0);
        }

        /// <summary>
        /// Get the remote endpoint of the socket.
        /// </summary>
        /// <returns>the remote endpoint</returns>
        public string GetRemoteEndPoint()
        {
            if (Socket == null)
                return string.Empty;

            return Socket.RemoteEndPoint?.ToString();
        }

        /// <summary>
        /// Set the socket to use for streaming.
        /// </summary>
        /// <param name="socketIn"></param>
        public void SetSocket(Socket socketIn)
        {
            Socket = socketIn;
        }
    }
}
