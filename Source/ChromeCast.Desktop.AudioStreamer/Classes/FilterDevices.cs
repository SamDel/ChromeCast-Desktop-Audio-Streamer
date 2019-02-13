using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public static class FilterDevices
    {
        /// <summary>
        /// Determine if the device is shown, depending on the filter.
        /// </summary>
        public static bool ShowFilterDevices(IDevice device, FilterDevicesEnum value)
        {
            switch (value)
            {
                case FilterDevicesEnum.ShowAll:
                    return true;
                case FilterDevicesEnum.DevicesOnly:
                    return !device.IsGroup();
                case FilterDevicesEnum.GroupsOnly:
                    return device.IsGroup();
                default:
                    return true;
            }
        }
    }

    public enum FilterDevicesEnum
    {
        ShowAll,
        DevicesOnly,
        GroupsOnly
    }
}
