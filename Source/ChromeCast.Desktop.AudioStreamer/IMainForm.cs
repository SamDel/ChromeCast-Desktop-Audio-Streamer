﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Streaming;

namespace ChromeCast.Desktop.AudioStreamer
{
    public interface IMainForm
    {
        void Log(string message);
        void ToggleFormVisibility(object sender, EventArgs e);
        void SetKeyboardHooks(bool useShortCuts);
        void ShowLagControl(bool showLag);
        void SetLagValue(int lagValue);
        void SetWindowVisibility(bool visible);
        void AddDevice(IDevice device);
        void Dispose();
        void AddRecordingDevices(List<RecordingDevice> devices, RecordingDevice defaultdevice);
        void GetRecordingDevice();
        void SetAutoRestart(bool autoRestart);
        bool GetUseKeyboardShortCuts();
        bool GetAutoStartDevices();
        bool GetShowWindowOnStart();
        bool GetAutoRestart();
        void SetAutoStart(bool autoStart);
        void DoDragDrop(object sender, DragEventArgs e);
        bool? GetShowLagControl();
        int? GetLagValue();
        void SetStreamFormat(SupportedStreamFormat format);
        void GetStreamFormat();
        SupportedStreamFormat GetSelectedStreamFormat();
        void SetCulture(string culture);
        void SetLogDeviceCommunication(bool logDeviceCommunication);
        bool GetLogDeviceCommunication();
        void SetStartApplicationWhenWindowsStarts(bool value);
        bool GetStartApplicationWhenWindowsStarts();
        void SetFilterDevices(FilterDevicesEnum filterDevicesEnum);
        FilterDevicesEnum? GetFilterDevices();
        void SetStartLastUsedDevices(bool value);
        bool? GetStartLastUsedDevices();
        void SetSize(Size size);
        Size GetSize();
        void SetPosition(int? left, int? top);
        int GetLeft();
        int GetTop();
        void ShowWavMeterValue(byte[] data);
        void SetExtraBufferInSeconds(int extraBufferInSeconds);
        int? GetExtraBufferInSeconds();
        void SetRecordingDeviceID(string recordingDeviceID);
        string GetRecordingDeviceID();
        void SetAutoMute(bool autoMute);
        bool GetAutoMute();
        IntPtr GetHandle();
        void RestartRecording();
        void SetMinimizeToTray(bool minimizeToTray);
        bool GetMinimizeToTray();
        void SetConvertMultiChannelToStereo(bool convertMultiChannelToStereo);
        bool GetConvertMultiChannelToStereo();
        void SetIP4AddressUsed(string ip4Address);
        string GetIP4AddressUsed();
        void SetDarkMode(bool darkmode);
        bool GetDarkMode();
    }
}