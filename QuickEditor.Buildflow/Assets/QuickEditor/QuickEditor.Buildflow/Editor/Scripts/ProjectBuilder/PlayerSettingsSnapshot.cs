#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.IO;
    using UnityEditor;
    using Debug = LoggerUtils;

    internal sealed partial class PlayerSettingsSnapshot
    {
        private const string PROJECT_SETTINGS_PATH = "ProjectSettings/ProjectSettings.asset";
        private const string PROJECT_SETTINGS_TEMP_PATH = "ProjectSettings/ProjectSettings.temp";

        private static bool mSnapshot;

        public static void TakeSnapshot()
        {
            if (!mSnapshot)
            {
                string settingsPath = BuildPipelineAsset.GetExternalGlobalPath(PROJECT_SETTINGS_PATH);
                string settingsPathTemp = BuildPipelineAsset.GetExternalGlobalPath(PROJECT_SETTINGS_TEMP_PATH);
                File.Copy(settingsPath, settingsPathTemp, true);
                mSnapshot = true;
                Debug.Log("Take PlayerSettings Snapshot succeeded");
            }
        }

        public static void ApplySnapshot()
        {
            string settingsPath = BuildPipelineAsset.GetExternalGlobalPath(PROJECT_SETTINGS_PATH);
            string settingsPathTemp = BuildPipelineAsset.GetExternalGlobalPath(PROJECT_SETTINGS_TEMP_PATH);
            if (File.Exists(settingsPathTemp))
            {
                File.Copy(settingsPathTemp, settingsPath, true);
                File.Delete(settingsPathTemp);
                mSnapshot = false;
                Debug.Log("Apply PlayerSettings Snapshot succeeded");
                AssetDatabase.Refresh();
            }
        }

        public static void ApplySnapshotIfFailed(Action buildAction)
        {
            try
            {
                buildAction();
            }
            catch (Exception e)
            {
                ApplySnapshot();
                throw e;
            }
        }
    }
}

#endif
