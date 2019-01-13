using System.Windows.Forms;
using ChromeCast.Desktop.AudioStreamer.Application;
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
        MMDevice GetRecordingDevice();
        void SetAutoRestart(bool autoRestart);
        bool GetUseKeyboardShortCuts();
        bool GetAutoStartDevices();
        bool GetShowWindowOnStart();
        bool GetAutoRestart();
        void SetAutoStart(bool autoStart);
        void DoDragDrop(object sender, DragEventArgs e);
    }
}