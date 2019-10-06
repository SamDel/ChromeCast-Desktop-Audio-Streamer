using ChromeCast.Desktop.AudioStreamer.Discover;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    /// <summary>
    /// Class to keep, save and load the user settings.
    /// </summary>
    public class UserSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        public bool? Upgraded
        {
            get
            {
                return (bool?)this["Upgraded"];
            }
            set
            {
                this["Upgraded"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? AutoStartDevices
        {
            get
            {
                return (bool?)this["AutoStartDevices"];
            }
            set
            {
                this["AutoStartDevices"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? UseKeyboardShortCuts
        {
            get
            {
                return (bool?)this["UseKeyboardShortCuts"];
            }
            set
            {
                this["UseKeyboardShortCuts"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? AutoRestart
        {
            get
            {
                return (bool?)this["AutoRestart"];
            }
            set
            {
                this["AutoRestart"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? ShowWindowOnStart
        {
            get
            {
                return (bool?)this["ShowWindowOnStart"];
            }
            set
            {
                this["ShowWindowOnStart"] = value;
            }
        }

        [UserScopedSetting()]
        public SupportedStreamFormat? StreamFormat
        {
            get
            {
                return (SupportedStreamFormat?)this["StreamFormat"];
            }
            set
            {
                this["StreamFormat"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? ShowLagControl
        {
            get
            {
                return (bool?)this["ShowLagControl"];
            }
            set
            {
                this["ShowLagControl"] = value;
            }
        }

        [UserScopedSetting()]
        public int? LagControlValue
        {
            get
            {
                return (int?)this["LagControlValue"];
            }
            set
            {
                this["LagControlValue"] = value;
            }
        }

        [UserScopedSetting()]
        public string Culture
        {
            get
            {
                return (string)this["Culture"];
            }
            set
            {
                this["Culture"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? LogDeviceCommunication
        {
            get
            {
                return (bool?)this["LogDeviceCommunication"];
            }
            set
            {
                this["LogDeviceCommunication"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? StartApplicationWhenWindowsStarts
        {
            get
            {
                return (bool?)this["StartApplicationWhenWindowsStarts"];
            }
            set
            {
                this["StartApplicationWhenWindowsStarts"] = value;
            }
        }

        [UserScopedSetting()]
        public FilterDevicesEnum? FilterDevices
        {
            get
            {
                return (FilterDevicesEnum?)this["FilterDevices"];
            }
            set
            {
                this["FilterDevices"] = value;
            }
        }

        [UserScopedSetting()]
        public List<DiscoveredDevice> ChromecastDiscoveredDevices
        {
            get
            {
                return (List<DiscoveredDevice>)this["ChromecastDiscoveredDevices"];
            }
            set
            {
                this["ChromecastDiscoveredDevices"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? StartLastUsedDevices
        {
            get
            {
                return (bool?)this["StartLastUsedDevices"];
            }
            set
            {
                this["StartLastUsedDevices"] = value;
            }
        }

        [UserScopedSetting()]
        public Size? Size
        {
            get
            {
                return (Size?)this["Size"] == null ? new Size(850, 550) : (Size?)this["Size"];
            }
            set
            {
                this["Size"] = value;
            }
        }

        [UserScopedSetting()]
        public int? Left
        {
            get
            {
                return (int?)this["Left"] == null ? 0 : (int?)this["Left"];
            }
            set
            {
                this["Left"] = value;
            }
        }

        [UserScopedSetting()]
        public int? Top
        {
            get
            {
                return (int?)this["Top"] == null ? 0 : (int?)this["Top"];
            }
            set
            {
                this["Top"] = value;
            }
        }

        [UserScopedSetting()]
        public int? ExtraBufferInSeconds
        {
            get
            {
                return (int?)this["ExtraBufferInSeconds"];
            }
            set
            {
                this["ExtraBufferInSeconds"] = value;
            }
        }

        [UserScopedSetting()]
        public string RecordingDeviceID
        {
            get
            {
                return (string)this["RecordingDeviceID"];
            }
            set
            {
                this["RecordingDeviceID"] = value;
            }
        }

        [UserScopedSetting()]
        public bool? AutoMute
        {
            get
            {
                return (bool?)this["AutoMute"];
            }
            set
            {
                this["AutoMute"] = value;
            }
        }

    }
}
