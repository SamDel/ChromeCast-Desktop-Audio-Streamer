using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace ChromeCast.Desktop.AudioStreamer.Rest
{
    public static class RestApiHandler
    {
        private const string responseOK = "HTTP/1.1 200 OK\r\nContent-Length: {1}\r\nContent-Type: text/json\r\nConnection: Closed\r\n\r\n{0}";
        private const string responseNotOK = "HTTP/1.1 400 Bad Request\r\nContent-Length: {1}\r\nContent-Type: text/json\r\nConnection: Closed\r\n\r\n{0}";
        private const string errorBadRequest = "{\"errors\": { \"status\": \"400 Bad Request\", \"id\": \"1\" } }";
        private const string errorNotSupported = "{\"errors\": { \"status\": \"400 Bad Request\", \"id\": \"2\", \"title\": \"Action not supported\" } }";
        private const string errorDeviceNotFound = "{\"errors\": { \"status\": \"404 Not Found\", \"id\": \"3\", \"title\": \"Device not found\" } }";
        private const string errorWrongVolume = "{\"errors\": { \"status\": \"400 Bad Request\", \"id\": \"3\", \"title\": \"Volume should be an integer between 0 and 100 (/volume/<device>/<volume>)\" } }";
        
        public static void Process(Socket socket, string request, IDevices devices, ILogger logger, IMainForm mainForm)
        {
            if (request == null || socket == null)
                return;

            string response;
            try
            {
                var req = request.Split('\r');
                if (req.Length == 0)
                    return;

                var requestAction = req[0].Split(' ');
                if (requestAction.Length < 2 || string.IsNullOrEmpty(requestAction[1]))
                    return;

                var action = HttpUtility.UrlDecode(requestAction[1].ToLowerInvariant());
                logger.Log($"RestApiHandler: {action}");
                if (action.StartsWith("/start"))
                    response = Start(action.Replace("/start", ""), devices);
                else if (action.StartsWith("/stop"))
                    response = Stop(action.Replace("/stop", ""), devices);
                else if (action.StartsWith("/volume"))
                    response = Volume(action.Replace("/volume", ""), devices);
                else if (action.StartsWith("/togglemute"))
                    response = ToggleMute(action.Replace("/togglemute", ""), devices);
                else if (action.StartsWith("/list"))
                    response = List(devices);
                else if (action.StartsWith("/restartrecording"))
                    response = RestartRecording(mainForm);
                else
                    response = errorNotSupported;

                if (response.IndexOf("{\"errors") == 0)
                    socket.Send(Encoding.UTF8.GetBytes(string.Format(responseNotOK, response, response.Length)));
                else
                    socket.Send(Encoding.UTF8.GetBytes(string.Format(responseOK, response, response.Length)));
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
                response = errorBadRequest;
                socket.Send(Encoding.UTF8.GetBytes(string.Format(responseNotOK, response, response.Length)));
            }
        }

        private static string RestartRecording(IMainForm mainForm)
        {
            mainForm.RestartRecording();
            var response = "{\"data\": { \"type\": \"done\", \"id\": \"1\", \"attributes\": { \"action\": \"/restartrecording\" } } }";
            return response;
        }

        private static string List(IDevices devices)
        {
            var deviceList = devices.GetDeviceList();
            List<string> list = new List<string>();
            foreach (var device in deviceList)
            {
                list.Add("{ \"type\": \"device\", " +
                    "\"attributes\": " +
                    "{ " +
                    "\"name\": \"" + device.GetFriendlyName() + "\", " +
                    "\"state\": \"" + device.GetDeviceState().ToString() + "\", " +
                    "\"volume\": \"" + device.GetVolumeLevel() + "\", " +
                    "\"ip\": \"" + device.GetHost() + "\", " +
                    "\"port\": \"" + device.GetPort() + "\", " +
                    "\"isgroup\": \"" + device.IsGroup() + "\"" +
                    " } }");
            }
            var response = string.Format("{{\"data\": [{0}] }}", string.Join(",", list));
            return response;
        }

        private static string ToggleMute(string action, IDevices devices)
        {
            if (string.IsNullOrEmpty(action.Replace("/", "")))
            {
                var deviceList = devices.GetDeviceList();
                foreach (var device in deviceList)
                {
                    device.VolumeMute();
                }
            }
            else
            {
                var device = GetDevice(devices, action);
                if (device == null)
                    return errorDeviceNotFound;

                device.VolumeMute();
            }

            var response = "{\"data\": { \"type\": \"done\", \"id\": \"1\", \"attributes\": { \"action\": \"/togglemute" + action + "\" } } }";
            return response;
        }

        private static string Volume(string action, IDevices devices)
        {
            var device = GetDevice(devices, action);
            if (device == null)
                return errorDeviceNotFound;

            var deviceVolume = action.Split('/')?.Length > 2 ? action.Split('/')[2] : null;
            if (string.IsNullOrEmpty(deviceVolume))
                return errorWrongVolume;

            if (!int.TryParse(deviceVolume, out int level))
                return errorWrongVolume;

            if (level < 0 || level > 100)
                return errorWrongVolume;

            device.VolumeSet(level / 100.0f);

            var response = "{\"data\": { \"type\": \"done\", \"id\": \"1\", \"attributes\": { \"action\": \"/volume" + action + "\" } } }";
            return response;
        }

        private static string Stop(string action, IDevices devices)
        {
            if (string.IsNullOrEmpty(action.Replace("/", "")))
            {
                var deviceList = devices.GetDeviceList();
                foreach (var device in deviceList)
                {
                    device.Stop(true);
                }
            }
            else
            {
                var device = GetDevice(devices, action);
                if (device == null)
                    return errorDeviceNotFound;

                device.Stop(true);
            }

            var response = "{\"data\": { \"type\": \"done\", \"id\": \"1\", \"attributes\": { \"action\": \"/stop" + action + "\" } } }";
            return response;
        }

        private static string Start(string action, IDevices devices)
        {
            if (string.IsNullOrEmpty(action.Replace("/", "")))
            {
                var deviceList = devices.GetDeviceList();
                foreach (var device in deviceList)
                {
                    device.ResumePlaying();
                }
            }
            else
            {
                var device = GetDevice(devices, action);
                if (device == null)
                    return errorDeviceNotFound;

                device.ResumePlaying();
            }

            var response = "{\"data\": { \"type\": \"done\", \"id\": \"1\", \"attributes\": { \"action\": \"/start" + action + "\" } } }";
            return response;
        }

        private static IDevice GetDevice(IDevices devices, string action)
        {
            if (action.IndexOf("/") != 0)
                return null;

            var deviceName = action.Split('/')?.Length > 0 ? action.Split('/')[1] : null;
            if (string.IsNullOrEmpty(deviceName))
                return null;

            var deviceList = devices.GetDeviceList();
            foreach (var device in deviceList)
            {
                if (device.GetFriendlyName().ToLowerInvariant() == deviceName)
                    return device;
            }

            return null;
        }
    }
}
