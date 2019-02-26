using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Discover;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    /// <summary>
    /// Get all device info with: 
    /// http://<ip>:8008/setup/eureka_info?params=version,audio,name,build_info,detail,device_info,net,wifi,setup,settings,opt_in,opencast,multizone,proxy,night_mode_params,user_eq,room_equalizer&options=detail
    /// </summary>
    public static class DeviceInformation
    {
        public static void GetDeviceInformation(DiscoveredDevice discoveredDevice, Action<DeviceEureka> callback, ILogger logger)
        {
            Task.Run(async () => {
                try
                {
                    var http = new HttpClient();
                    var response = await http.GetAsync($"http://{discoveredDevice.IPAddress}:8008/setup/eureka_info?params=version,audio,name,build_info,detail,device_info,net,wifi,setup,settings,opt_in,opencast,multizone,proxy,night_mode_params,user_eq,room_equalizer&options=detail");
                    var receiveStream = await response.Content.ReadAsStreamAsync();
                    var readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    var eurekaInfo = readStream.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var eureka = JsonConvert.DeserializeObject<DeviceEureka>(eurekaInfo);
                        callback?.Invoke(eureka);
                    }
                }
                catch (Exception ex)
                {
                    logger?.Log(ex, "DeviceInformation.GetDeviceInformation");
                }
            });
        }

        /// <summary>
        /// Check if the device is on.
        /// </summary>
        /// <param name="discoveredDevice"></param>
        /// <param name="callback"></param>
        public static void CheckDeviceIsOn(DiscoveredDevice discoveredDevice, Action<DiscoveredDevice> callback, ILogger logger)
        {
            Task.Run(async () => {
                try
                {
                    // Check if the device is on.
                    var http = new HttpClient();
                    var response = await http.GetAsync($"http://{discoveredDevice.IPAddress}:8008/setup/eureka_info?params=version,audio,name,build_info,detail,device_info,net,wifi,setup,settings,opt_in,opencast,multizone,proxy,night_mode_params,user_eq,room_equalizer&options=detail");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        discoveredDevice.AddedByDeviceInfo = false;
                        callback?.Invoke(discoveredDevice);
                    }
                }
                catch (Exception ex)
                {
                    logger?.Log(ex, "DeviceInformation.CheckDeviceIsOn");
                }
            });
        }
    }

    public class DeviceEureka
    {
        public Audio Audio { get; set; }
        [JsonProperty("build_info")]
        public BuildInfo BuildInfo { get; set; }
        public Detail Detail { get; set; }
        [JsonProperty("device_info")]
        public DeviceInfo DeviceInfo { get; set; }
        public Multizone Multizone { get; set; }
        public string Name { get; set; }
        public Net Net { get; set; }
        [JsonProperty("opt_in")]
        public OptIn OptIn { get; set; }
        public Proxy Proxy { get; set; }
        public Settings Settings { get; set; }
        public Setup Setup { get; set; }
        [JsonProperty("user_eq")]
        public UserEq UserEq { get; set; }
        public int Version { get; set; }
        public Wifi Wifi { get; set; }
    }

    public class Audio
    {
        public bool Digital { get; set; }
    }

    public class BuildInfo
    {
        public int Build_type { get; set; }
        public string Cast_build_revision { get; set; }
        public int Cast_control_version { get; set; }
        public int Preview_channel_state { get; set; }
        public string Release_track { get; set; }
        public string System_build_number { get; set; }
    }

    public class IconList
    {
        public int Depth { get; set; }
        public int Height { get; set; }
        public string Mimetype { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Locale
    {
        public string Display_string { get; set; }
    }

    public class Detail
    {
        public List<IconList> Icon_list { get; set; }
        public Locale Locale { get; set; }
    }

    public class Capabilities
    {
        public bool Audio_hdr_supported { get; set; }
        public bool Audio_surround_mode_supported { get; set; }
        public bool Ble_supported { get; set; }
        public bool Cloudcast_supported { get; set; }
        public bool Display_supported { get; set; }
        public bool Fdr_supported { get; set; }
        public bool Hdmi_prefer_50hz_supported { get; set; }
        public bool Hdmi_prefer_high_fps_supported { get; set; }
        public bool Hi_res_audio_supported { get; set; }
        public bool Hotspot_supported { get; set; }
        public bool Https_setup_supported { get; set; }
        public bool Keep_hotspot_until_connected_supported { get; set; }
        public bool Multizone_supported { get; set; }
        public bool Opencast_supported { get; set; }
        public bool Preview_channel_supported { get; set; }
        public bool Reboot_supported { get; set; }
        public bool Setup_supported { get; set; }
        public bool Stats_supported { get; set; }
        public bool System_sound_effects_supported { get; set; }
        public bool User_eq_supported { get; set; }
        public bool Wifi_auto_save_supported { get; set; }
        public bool Wifi_supported { get; set; }
    }

    public class DeviceInfo
    {
        [JsonProperty("4k_blocked")]
        public int __invalid_name__4k_blocked { get; set; }
        public Capabilities Capabilities { get; set; }
        public string Cloud_device_id { get; set; }
        public string Factory_country_code { get; set; }
        public string Hotspot_bssid { get; set; }
        public string Local_authorization_token_hash { get; set; }
        public string Mac_address { get; set; }
        public string Manufacturer { get; set; }
        public string Model_name { get; set; }
        public string Product_name { get; set; }
        public string Public_key { get; set; }
        public string Ssdp_udn { get; set; }
        public double Uptime { get; set; }
    }

    public class Group
    {
        public int Cast_port { get; set; }
        public string Channel_selection { get; set; }
        public string Elected_leader { get; set; }
        public string Leader { get; set; }
        public bool Multichannel_group { get; set; }
        public string Name { get; set; }
        public double Stereo_balance { get; set; }
        public string Uuid { get; set; }
    }

    public class Multizone
    {
        public double Audio_output_delay { get; set; }
        public double Audio_output_delay_hdmi { get; set; }
        public double Audio_output_delay_oem { get; set; }
        public string Aux_in_group { get; set; }
        public List<Group> Groups { get; set; }
        public int Multichannel_status { get; set; }
    }

    public class Net
    {
        public bool Ethernet_connected { get; set; }
        public string Ip_address { get; set; }
        public bool Online { get; set; }
    }

    public class OptIn
    {
        public bool Audio_hdr { get; set; }
        public int Audio_surround_mode { get; set; }
        public bool Autoplay_on_signal { get; set; }
        public bool Cloud_ipc { get; set; }
        public bool Hdmi_prefer_50hz { get; set; }
        public bool Hdmi_prefer_high_fps { get; set; }
        public bool Managed_mode { get; set; }
        public bool Opencast { get; set; }
        public bool Preview_channel { get; set; }
        public bool Remote_ducking { get; set; }
        public bool Stats { get; set; }
        public bool Ui_flipped { get; set; }
    }

    public class Proxy
    {
        public string Mode { get; set; }
    }

    public class ClosedCaption
    {
    }

    public class Settings
    {
        public ClosedCaption Closed_caption { get; set; }
        public int Control_notifications { get; set; }
        public string Country_code { get; set; }
        public string Locale { get; set; }
        public int Network_standby { get; set; }
        public bool System_sound_effects { get; set; }
        public int Time_format { get; set; }
        public int Wake_on_cast { get; set; }
    }

    public class Stats
    {
        public int Num_check_connectivity { get; set; }
        public int Num_connect_wifi { get; set; }
        public int Num_connected_wifi_not_saved { get; set; }
        public int Num_initial_eureka_info { get; set; }
        public int Num_obtain_ip { get; set; }
    }

    public class Setup
    {
        public int Setup_state { get; set; }
        public string Ssid_suffix { get; set; }
        public Stats Stats { get; set; }
        public bool Tos_accepted { get; set; }
    }

    public class HighShelf
    {
        public double Frequency { get; set; }
        public double Gain_db { get; set; }
        public double Quality { get; set; }
    }

    public class LowShelf
    {
        public double Frequency { get; set; }
        public double Gain_db { get; set; }
        public double Quality { get; set; }
    }

    public class UserEq
    {
        public HighShelf High_shelf { get; set; }
        public LowShelf Low_shelf { get; set; }
        public int Max_peaking_eqs { get; set; }
        public List<object> Peaking_eqs { get; set; }
    }

    public class Wifi
    {
        public string Bssid { get; set; }
        public bool Has_changes { get; set; }
        public int Noise_level { get; set; }
        public int Signal_level { get; set; }
        public string Ssid { get; set; }
        public bool Wpa_configured { get; set; }
        public int Wpa_id { get; set; }
        public int Wpa_state { get; set; }
    }
}
