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
                return (bool?)this[nameof(Upgraded)];
            }
            set
            {
                this[nameof(Upgraded)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? AutoStartDevices
        {
            get
            {
                return (bool?)this[nameof(AutoStartDevices)];
            }
            set
            {
                this[nameof(AutoStartDevices)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? UseKeyboardShortCuts
        {
            get
            {
                return (bool?)this[nameof(UseKeyboardShortCuts)];
            }
            set
            {
                this[nameof(UseKeyboardShortCuts)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? AutoRestart
        {
            get
            {
                return (bool?)this[nameof(AutoRestart)];
            }
            set
            {
                this[nameof(AutoRestart)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? ShowWindowOnStart
        {
            get
            {
                return (bool?)this[nameof(ShowWindowOnStart)];
            }
            set
            {
                this[nameof(ShowWindowOnStart)] = value;
            }
        }

        [UserScopedSetting()]
        public string Ip4AddressUsed
        {
            get
            {
                return (string)this[nameof(Ip4AddressUsed)];
            }
            set
            {
                this[nameof(Ip4AddressUsed)] = value;
            }
        }

        [UserScopedSetting()]
        public SupportedStreamFormat? StreamFormat
        {
            get
            {
                return (SupportedStreamFormat?)this[nameof(StreamFormat)];
            }
            set
            {
                this[nameof(StreamFormat)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? ShowLagControl
        {
            get
            {
                return (bool?)this[nameof(ShowLagControl)];
            }
            set
            {
                this[nameof(ShowLagControl)] = value;
            }
        }

        [UserScopedSetting()]
        public int? LagControlValue
        {
            get
            {
                return (int?)this[nameof(LagControlValue)];
            }
            set
            {
                this[nameof(LagControlValue)] = value;
            }
        }

        [UserScopedSetting()]
        public string Culture
        {
            get
            {
                return (string)this[nameof(Culture)];
            }
            set
            {
                this[nameof(Culture)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? LogDeviceCommunication
        {
            get
            {
                return (bool?)this[nameof(LogDeviceCommunication)];
            }
            set
            {
                this[nameof(LogDeviceCommunication)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? StartApplicationWhenWindowsStarts
        {
            get
            {
                return (bool?)this[nameof(StartApplicationWhenWindowsStarts)];
            }
            set
            {
                this[nameof(StartApplicationWhenWindowsStarts)] = value;
            }
        }

        [UserScopedSetting()]
        public FilterDevicesEnum? FilterDevices
        {
            get
            {
                return (FilterDevicesEnum?)this[nameof(FilterDevices)];
            }
            set
            {
                this[nameof(FilterDevices)] = value;
            }
        }

        [UserScopedSetting()]
        public List<DiscoveredDevice> ChromecastDiscoveredDevices
        {
            get
            {
                return (List<DiscoveredDevice>)this[nameof(ChromecastDiscoveredDevices)];
            }
            set
            {
                this[nameof(ChromecastDiscoveredDevices)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? StartLastUsedDevices
        {
            get
            {
                return (bool?)this[nameof(StartLastUsedDevices)];
            }
            set
            {
                this[nameof(StartLastUsedDevices)] = value;
            }
        }

        [UserScopedSetting()]
        public Size? Size
        {
            get
            {
                return (Size?)this[nameof(Size)] == null ? new Size(850, 550) : (Size?)this[nameof(Size)];
            }
            set
            {
                this[nameof(Size)] = value;
            }
        }

        [UserScopedSetting()]
        public int? Left
        {
            get
            {
                return (int?)this[nameof(Left)] == null ? 0 : (int?)this[nameof(Left)];
            }
            set
            {
                this[nameof(Left)] = value;
            }
        }

        [UserScopedSetting()]
        public int? Top
        {
            get
            {
                return (int?)this[nameof(Top)] == null ? 0 : (int?)this[nameof(Top)];
            }
            set
            {
                this[nameof(Top)] = value;
            }
        }

        [UserScopedSetting()]
        public int? ExtraBufferInSeconds
        {
            get
            {
                return (int?)this[nameof(ExtraBufferInSeconds)];
            }
            set
            {
                this[nameof(ExtraBufferInSeconds)] = value;
            }
        }

        [UserScopedSetting()]
        public string RecordingDeviceID
        {
            get
            {
                return (string)this[nameof(RecordingDeviceID)];
            }
            set
            {
                this[nameof(RecordingDeviceID)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? AutoMute
        {
            get
            {
                return (bool?)this[nameof(AutoMute)];
            }
            set
            {
                this[nameof(AutoMute)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? MinimizeToTray
        {
            get
            {
                return (bool?)this[nameof(MinimizeToTray)];
            }
            set
            {
                this[nameof(MinimizeToTray)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? ConvertMultiChannelToStereo
        {
            get
            {
                return (bool?)this[nameof(ConvertMultiChannelToStereo)];
            }
            set
            {
                this[nameof(ConvertMultiChannelToStereo)] = value;
            }
        }

        [UserScopedSetting()]
        public bool? DarkMode
        {
            get
            {
                return (bool?)this[nameof(DarkMode)];
            }
            set
            {
                this[nameof(DarkMode)] = value;
            }
        }

        [UserScopedSetting()]
        public string StreamTitle
        {
            get
            {
                return (string)this[nameof(StreamTitle)];
            }
            set
            {
                this[nameof(StreamTitle)] = value;
            }
        }
    }
}
