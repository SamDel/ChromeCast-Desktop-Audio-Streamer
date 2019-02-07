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
        public static void GetDeviceInformation(DiscoveredDevice discoveredDevice, Action<DeviceEureka> callback)
        {
            Task.Run(async () => {
                var http = new HttpClient();
                var response = await http.GetAsync($"http://{discoveredDevice.IPAddress}:8008/setup/eureka_info?params=version,audio,name,build_info,detail,device_info,net,wifi,setup,settings,opt_in,opencast,multizone,proxy,night_mode_params,user_eq,room_equalizer&options=detail");
                var receiveStream = response.Content.ReadAsStreamAsync();
                receiveStream.Wait();
                var readStream = new StreamReader(receiveStream.Result, Encoding.UTF8);
                var eurekaInfo = readStream.ReadToEnd();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var eureka = JsonConvert.DeserializeObject<DeviceEureka>(eurekaInfo);
                    callback?.Invoke(eureka);
                }
            });
        }

        /// <summary>
        /// Check if the device is on.
        /// </summary>
        /// <param name="discoveredDevice"></param>
        /// <param name="callback"></param>
        public static void CheckDeviceIsOn(DiscoveredDevice discoveredDevice, Action<DiscoveredDevice> callback)
        {
            Task.Run(async () => {
                // Check if the device is on.
                var http = new HttpClient();
                var response = await http.GetAsync($"http://{discoveredDevice.IPAddress}:8008/setup/eureka_info?params=version,audio,name,build_info,detail,device_info,net,wifi,setup,settings,opt_in,opencast,multizone,proxy,night_mode_params,user_eq,room_equalizer&options=detail");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    callback?.Invoke(discoveredDevice);
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
        public bool digital { get; set; }
    }

    public class BuildInfo
    {
        public int build_type { get; set; }
        public string cast_build_revision { get; set; }
        public int cast_control_version { get; set; }
        public int preview_channel_state { get; set; }
        public string release_track { get; set; }
        public string system_build_number { get; set; }
    }

    public class IconList
    {
        public int depth { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Locale
    {
        public string display_string { get; set; }
    }

    public class Detail
    {
        public List<IconList> icon_list { get; set; }
        public Locale locale { get; set; }
    }

    public class Capabilities
    {
        public bool audio_hdr_supported { get; set; }
        public bool audio_surround_mode_supported { get; set; }
        public bool ble_supported { get; set; }
        public bool cloudcast_supported { get; set; }
        public bool display_supported { get; set; }
        public bool fdr_supported { get; set; }
        public bool hdmi_prefer_50hz_supported { get; set; }
        public bool hdmi_prefer_high_fps_supported { get; set; }
        public bool hi_res_audio_supported { get; set; }
        public bool hotspot_supported { get; set; }
        public bool https_setup_supported { get; set; }
        public bool keep_hotspot_until_connected_supported { get; set; }
        public bool multizone_supported { get; set; }
        public bool opencast_supported { get; set; }
        public bool preview_channel_supported { get; set; }
        public bool reboot_supported { get; set; }
        public bool setup_supported { get; set; }
        public bool stats_supported { get; set; }
        public bool system_sound_effects_supported { get; set; }
        public bool user_eq_supported { get; set; }
        public bool wifi_auto_save_supported { get; set; }
        public bool wifi_supported { get; set; }
    }

    public class DeviceInfo
    {
        [JsonProperty("4k_blocked")]
        public int __invalid_name__4k_blocked { get; set; }
        public Capabilities capabilities { get; set; }
        public string cloud_device_id { get; set; }
        public string factory_country_code { get; set; }
        public string hotspot_bssid { get; set; }
        public string local_authorization_token_hash { get; set; }
        public string mac_address { get; set; }
        public string manufacturer { get; set; }
        public string model_name { get; set; }
        public string product_name { get; set; }
        public string public_key { get; set; }
        public string ssdp_udn { get; set; }
        public double uptime { get; set; }
    }

    public class Group
    {
        public int cast_port { get; set; }
        public string channel_selection { get; set; }
        public string elected_leader { get; set; }
        public string leader { get; set; }
        public bool multichannel_group { get; set; }
        public string name { get; set; }
        public double stereo_balance { get; set; }
        public string uuid { get; set; }
    }

    public class Multizone
    {
        public double audio_output_delay { get; set; }
        public double audio_output_delay_hdmi { get; set; }
        public double audio_output_delay_oem { get; set; }
        public string aux_in_group { get; set; }
        public List<Group> groups { get; set; }
        public int multichannel_status { get; set; }
    }

    public class Net
    {
        public bool ethernet_connected { get; set; }
        public string ip_address { get; set; }
        public bool online { get; set; }
    }

    public class OptIn
    {
        public bool audio_hdr { get; set; }
        public int audio_surround_mode { get; set; }
        public bool autoplay_on_signal { get; set; }
        public bool cloud_ipc { get; set; }
        public bool hdmi_prefer_50hz { get; set; }
        public bool hdmi_prefer_high_fps { get; set; }
        public bool managed_mode { get; set; }
        public bool opencast { get; set; }
        public bool preview_channel { get; set; }
        public bool remote_ducking { get; set; }
        public bool stats { get; set; }
        public bool ui_flipped { get; set; }
    }

    public class Proxy
    {
        public string mode { get; set; }
    }

    public class ClosedCaption
    {
    }

    public class Settings
    {
        public ClosedCaption closed_caption { get; set; }
        public int control_notifications { get; set; }
        public string country_code { get; set; }
        public string locale { get; set; }
        public int network_standby { get; set; }
        public bool system_sound_effects { get; set; }
        public int time_format { get; set; }
        public int wake_on_cast { get; set; }
    }

    public class Stats
    {
        public int num_check_connectivity { get; set; }
        public int num_connect_wifi { get; set; }
        public int num_connected_wifi_not_saved { get; set; }
        public int num_initial_eureka_info { get; set; }
        public int num_obtain_ip { get; set; }
    }

    public class Setup
    {
        public int setup_state { get; set; }
        public string ssid_suffix { get; set; }
        public Stats stats { get; set; }
        public bool tos_accepted { get; set; }
    }

    public class HighShelf
    {
        public double frequency { get; set; }
        public double gain_db { get; set; }
        public double quality { get; set; }
    }

    public class LowShelf
    {
        public double frequency { get; set; }
        public double gain_db { get; set; }
        public double quality { get; set; }
    }

    public class UserEq
    {
        public HighShelf high_shelf { get; set; }
        public LowShelf low_shelf { get; set; }
        public int max_peaking_eqs { get; set; }
        public List<object> peaking_eqs { get; set; }
    }

    public class Wifi
    {
        public string bssid { get; set; }
        public bool has_changes { get; set; }
        public int noise_level { get; set; }
        public int signal_level { get; set; }
        public string ssid { get; set; }
        public bool wpa_configured { get; set; }
        public int wpa_id { get; set; }
        public int wpa_state { get; set; }
    }

    public class RootObject
    {
        public Audio audio { get; set; }
        public BuildInfo build_info { get; set; }
        public Detail detail { get; set; }
        public DeviceInfo device_info { get; set; }
        public Multizone multizone { get; set; }
        public string name { get; set; }
        public Net net { get; set; }
        public OptIn opt_in { get; set; }
        public Proxy proxy { get; set; }
        public Settings settings { get; set; }
        public Setup setup { get; set; }
        public UserEq user_eq { get; set; }
        public int version { get; set; }
        public Wifi wifi { get; set; }
    }
}
