using ChromeCast.Desktop.AudioStreamer.Application;

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
        void AddDevice(IDevice device);
    }
}