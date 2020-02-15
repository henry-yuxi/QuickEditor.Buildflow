#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Debug = UnityEngine.Debug;

    public static class AdbRequest
    {
        private const string _EDITOR_ANDROID_SDK = "AndroidSdkRoot";
        private const string _ERROR_MATCH = @"(adb: error:\s+|Failure\s+\[.*\])";
        private const string _DEVICE_MATCH = @"(.*.\w+)\s+device\b";

        public static void InstallToDevice(string path, Action<bool> OnDone)
        {
            InstallToDevice(path, null, OnDone);
        }

        public static void InstallToDevice(string path, string deviceId, Action<bool> OnDone)
        {
            var request = CreateRequestAdb(string.Format("{0} install -r \"{1}\"", ForDevicePart(deviceId), path));
            request.Execute(
                onExited: success =>
                {
                    if (OnDone != null) OnDone(success);
                },
                onOutput: status =>
                {
                    if (IsWasError(status))
                    {
                        request.Abort();
                    }
                    else
                    {
                    }
                },
                onError: error => !error.StartsWith("success", true, CultureInfo.InvariantCulture));
        }

        public static void RunOnDevice(string package)
        {
            RunOnDevice(package, null);
        }

        public static void RunOnDevice(string package, string deviceId)
        {
            Debug.Log("Running apk...");
            CreateRequestAdb(string.Format("{0} shell monkey -p {1} -c android.intent.category.LAUNCHER 1 ",
                ForDevicePart(deviceId), package))
            .Execute();
        }

        public static List<string> GetDevices()
        {
            var output = CreateRequestAdb("devices").Execute();
            var devices = new List<string>();
            foreach (Match match in Regex.Matches(output, _DEVICE_MATCH))
            {
                devices.Add(match.Groups[1].Value);
            }
            return devices;
        }

        private static string ForDevicePart(string deviceId)
        {
            if (deviceId == null)
                return "";
            return string.Format("-s \"{0}\"", deviceId);
        }

        private static bool IsWasError(string output)
        {
            var match = Regex.Match(output, _ERROR_MATCH, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                Debug.LogError(output);
                return true;
            }
            return false;
        }

        private static ProcRunner CreateRequestAdb(string args = "")
        {
            var androidSDK = UnityEditor.EditorPrefs.GetString(_EDITOR_ANDROID_SDK);
            var adbPath = string.IsNullOrEmpty(androidSDK)
                ? "adb"
                : androidSDK + "/platform-tools/adb";
            return ProcessUtils.CreateProcRunner(adbPath, args);
        }
    }
}

#endif