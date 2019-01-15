using System.Collections.Generic;
using System.Configuration;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
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
        public List<SettingHost> ChromecastHosts
        {
            get
            {
                return (List<SettingHost>)this["ChromecastHosts"];
            }
            set
            {
                this["ChromecastHosts"] = value;
            }
        }
    }

    public class SettingHost
    {
        public string Ip { get; set; }
        public string Name { get; set; }
    }
}
