using System;
using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Classes;
using CSCore.CoreAudioAPI;

namespace ChromeCast.Desktop.AudioStreamer
{
    public interface IMainForm
    {
        void Log(string message);
        void ToggleVisibility();
        void SetKeyboardHooks(bool useShortCuts);
        void ShowLog(bool boolShowLog);
        void ShowLagControl(bool showLag);
        void SetLagValue(int lagValue);
        void SetWindowVisibility(bool visible);
        void AddDevice(IDevice device);
        bool DoSyncDevices();
        void Dispose();
        void AddRecordingDevices(MMDeviceCollection devices, MMDevice defaultdevice);
        void GetRecordingDevice(Action<MMDevice> startRecordingSetDevice);
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
        void SetCulture(string culture);
        void SetLogDeviceCommunication(bool logDeviceCommunication);
        bool GetLogDeviceCommunication();
    }
}