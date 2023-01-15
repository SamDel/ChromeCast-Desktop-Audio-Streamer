using System;
using System.Text;
using System.Net.Sockets;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Communication;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class StreamingConnection : IStreamingConnection
    {
        private Socket Socket;
        private IDevice device;
        private ILogger logger;
        private readonly IAudioHeader audioHeader;
        private bool isAudioHeaderSent;
        private int reduceLagCounter = 0;
        private readonly Thread streamThread;
        private BufferBlock bufferCaptured, bufferSend;
        readonly object bufferSwapSync = new();

        public StreamingConnection()
        {
            audioHeader = new AudioHeader();
            isAudioHeaderSent = false;
            bufferCaptured = new BufferBlock() { Data = new byte[10000000] };
            bufferSend = new BufferBlock() { Data = new byte[10000000] };

            streamThread = new Thread(StreamThread)
            {
                Name = "Stream Thread",
                IsBackground = true
            };
            streamThread.Start(new WeakReference<StreamingConnection>(this));
        }

        /// <summary>
        /// Thread for streaming the captured data.
        /// </summary>
        /// <param name="param">the streaming connection</param>
        private static void StreamThread(object param)
        {
            var thisRef = (WeakReference<StreamingConnection>)param;
            try
            {
                while (true)
                {
                    if (!thisRef.TryGetTarget(out StreamingConnection streamer) || streamer == null)
                    {
                        // Instance is dead
                        return;
                    }

                    // Stream the data that is captured.
                    streamer.SwapBuffer();
                    if (streamer.bufferSend.Used > 0)
                    {
                        var bytes = new List<byte>();
                        bytes.AddRange(streamer.bufferSend.Data.Take(streamer.bufferSend.Used).ToArray());
                        streamer.bufferSend.Used = 0;

                        try
                        {
                            streamer.Socket?.Send(bytes.ToArray());
                        }
                        catch (Exception ex)
                        {
                            var deviceState = streamer.device?.GetDeviceState();
                            if (deviceState == DeviceState.Playing ||
                                deviceState == DeviceState.Buffering ||
                                deviceState == DeviceState.Paused)
                            {
                                streamer.Dispose();
                                streamer.logger.Log(ex, $"[{DateTime.Now.ToLongTimeString()}] [{streamer.device.GetHost()}:{streamer.device.GetPort()}] Disconnected Send");
                                streamer.device?.SetDeviceState(DeviceState.ConnectError);
                                streamer.device?.CloseConnection();
                            }
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }

        /// <summary>
        /// Swap the captured and send buffers.
        /// </summary>
        private void SwapBuffer()
        {
            lock (bufferSwapSync)
            {
                var tmp = bufferCaptured;
                bufferCaptured = bufferSend;
                bufferSend = tmp;
            }
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

            // Send audio header before the first data.
            if (!isAudioHeaderSent)
            {
                isAudioHeaderSent = true;
                if (streamFormat.Equals(SupportedStreamFormat.Wav) ||
                    streamFormat.Equals(SupportedStreamFormat.Wav_16bit) ||
                    streamFormat.Equals(SupportedStreamFormat.Wav_24bit) ||
                    streamFormat.Equals(SupportedStreamFormat.Wav_32bit))
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
        /// Add the data to the buffer. 
        /// The actual sending is done in the thread.
        /// </summary>
        public void Send(byte[] data)
        {
            if (!IsConnected() || device == null || logger == null)
                return;

            lock (bufferSwapSync)
            {
                var currentBuffer = bufferCaptured;
                currentBuffer.Add(data, data.Length);
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
        private static string GetStartStreamingResponse()
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

        public void Dispose()
        {
            Socket?.Close();
            Socket?.Dispose();
            Socket = null;
        }

        /// <summary>
        /// Get the remote endpoint of the socket.
        /// </summary>
        /// <returns>the remote endpoint</returns>
        public string GetRemoteEndPoint()
        {
            if (Socket == null)
                return string.Empty;

            try
            {
                return Socket?.RemoteEndPoint?.ToString();
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Set the socket to use for streaming.
        /// </summary>
        /// <param name="socketIn"></param>
        public void SetDependencies(Socket socketIn, IDevice deviceIn, ILogger loggerIn)
        {
            device = deviceIn;
            logger = loggerIn;
            Socket = socketIn;
            Socket.SendTimeout = 10000;
        }
    }
}
