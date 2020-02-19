#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;

    [System.Serializable]
    public class BuildEnvsOptions
    {
        public string assetBundleManifestPath { get; private set; }
        public string locationPathName { get; private set; }
        public BuildOptions options { get; private set; }
        public string[] scenes { get; private set; }
        public BuildTarget target { get; private set; }
        public BuildTargetGroup targetGroup { get; private set; }
        public int buildNumber { get; private set; }
        public string version { get; private set; }
        public string channel { get; private set; }
        public ProjectBuildPresetSettings.BuildType buildType { get; private set; }
        public bool useAPKExpansionFiles { get; private set; }
        public AndroidBuildSystem androidBuildSystem { get; private set; }
        public ProjectBuildPresetSettings.AndroidExportSystem androidExportSystem { get; private set; }

        public bool IsValid
        {
            get
            {
                // TODO: Check for empty strings for build or level names?
                return true;
            }
        }

        public void ApplySettings()
        {
            ApplyCommonSettings();
            ApplyBuildSettings();
            ApplySDKSettings();
        }

        public void DeletePackagingCacheFolders()
        {
            string[] deletePaths = BuildPipelineAsset.NeedDeletePaths;
            if (deletePaths != null && deletePaths.Length > 0)
            {
                foreach (var assetPath in deletePaths)
                {
                    string fullpath = BuildPipelineAsset.GetExternalGlobalPath(assetPath);
                    if (string.IsNullOrEmpty(fullpath)) { continue; }
                    BuildPipelineCommonTools.FileUtils.DeleteFolder(fullpath);
                }
            }
            AssetDatabase.Refresh();
        }

        public void CopyPackagingResources()
        {
            var channel = Channel;
            if (channel != null)
            {
                channel.CopyResources();
            }
        }

        public void ApplyPackagingResources()
        {
            var channel = Channel;
            if (channel != null)
            {
                channel.ApplyResources();
            }
        }

        protected void ApplyCommonSettings()
        {
            bool release = buildType == ProjectBuildPresetSettings.BuildType.Release;
            if (!release)
            {
                AddBuildOptions(BuildOptions.Development);
            }
            PlayerSettingsResolver.EditorBuildSettings.ApplySettings(androidBuildSystem: androidBuildSystem);
            PlayerSettingsResolver.EditorBuildSettings.ApplySettings(release);
            PlayerSettingsResolver.BundleVersion.ApplySettings(version, buildNumber);
        }

        protected void ApplyBuildSettings()
        {
            if (androidExportSystem == ProjectBuildPresetSettings.AndroidExportSystem.AndroidProject)
            {
                PlayerSettingsResolver.Android.ApplyExportSettings(exportAsGoogleAndroidProject: true, buildApkPerCpuArchitecture: false, buildAppBundle: false, useAPKExpansionFiles: useAPKExpansionFiles);
            }
            else if (androidExportSystem == ProjectBuildPresetSettings.AndroidExportSystem.AppBundle)
            {
                PlayerSettingsResolver.Android.ApplyExportSettings(exportAsGoogleAndroidProject: false, buildApkPerCpuArchitecture: false, buildAppBundle: true, useAPKExpansionFiles: useAPKExpansionFiles);
            }
            else if (androidExportSystem == ProjectBuildPresetSettings.AndroidExportSystem.SingleAPK)
            {
                PlayerSettingsResolver.Android.ApplyExportSettings(exportAsGoogleAndroidProject: false, buildApkPerCpuArchitecture: false, buildAppBundle: false, useAPKExpansionFiles: useAPKExpansionFiles);
            }
            else if (androidExportSystem == ProjectBuildPresetSettings.AndroidExportSystem.SplitArchitectureAPK)
            {
                PlayerSettingsResolver.Android.ApplyExportSettings(exportAsGoogleAndroidProject: false, buildApkPerCpuArchitecture: true, buildAppBundle: false, useAPKExpansionFiles: useAPKExpansionFiles);
            }
        }

        private ProjectSDKConfigure.ChannelPresetSettings Channel
        {
            get
            {
                if (string.IsNullOrEmpty(channel)) { channel = "default"; }
                List<ProjectSDKConfigure.ChannelPresetSettings> presets = ProjectSDKConfigure.Current.Channels;
                return presets.Find(s => s.channel == channel);
            }
        }

        protected void ApplySDKSettings()
        {
            var channel = Channel;
            if (channel != null)
            {
                channel.ApplySettings();
            }
        }

        public BuildPlayerOptions GetBuildPlayerOptions()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = locationPathName;
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.target = target;
            buildPlayerOptions.targetGroup = targetGroup;
            buildPlayerOptions.options = options;
            return buildPlayerOptions;
        }

        #region Internal Methods

        public BuildEnvsOptions()
        {
        }

        public BuildEnvsOptions(BuildEnvsOptions buildOptions)
        {
            this.assetBundleManifestPath = buildOptions.assetBundleManifestPath;
            this.locationPathName = buildOptions.locationPathName;
            this.options = buildOptions.options;
            this.scenes = buildOptions.scenes;
            this.target = buildOptions.target;
            this.targetGroup = buildOptions.targetGroup;
        }

        internal BuildEnvsOptions SetBuildSettings(ProjectBuildPresetSettings presetSettings)
        {
            this.assetBundleManifestPath = null;
            this.locationPathName = presetSettings.locationPathName;
            this.options = presetSettings.buildOptions;
            this.scenes = presetSettings.scenes;
            this.target = presetSettings.activeBuildTarget;
            this.targetGroup = presetSettings.activeTargetGroup;

            this.androidExportSystem = presetSettings.androidExportSystem;
            this.useAPKExpansionFiles = presetSettings.useAPKExpansionFiles;

            this.buildType = presetSettings.buildType;
            this.androidBuildSystem = presetSettings.androidBuildSystem;
            this.buildNumber = presetSettings.bundleVersionCode;
            if (presetSettings.useSystemTime)
            {
                DateTime date = DateTime.Now;
                this.version = "1." + date.Month + "." + date.Day;
            }
            else
            {
                this.version = presetSettings.version;
            }

            return this;
        }

        internal BuildEnvsOptions SetBuildTarget(BuildTarget target)
        {
            this.target = target;
            return this;
        }

        internal BuildEnvsOptions SetBuildTargetGroup(BuildTargetGroup targetGroup)
        {
            this.targetGroup = targetGroup;
            return this;
        }

        internal BuildEnvsOptions SetActiveScenes(string[] scenes)
        {
            this.scenes = scenes;
            return this;
        }

        internal BuildEnvsOptions SetLocationPathName(string locationPathName)
        {
            this.locationPathName = locationPathName;
            return this;
        }

        internal BuildEnvsOptions AddBuildOptions(params BuildOptions[] options)
        {
            BuildOptions merged = BuildOptions.None;
            foreach (var opt in options)
                merged |= opt;
            this.options |= merged;
            return this;
        }

        internal BuildEnvsOptions RemoveBuildOption(BuildOptions options)
        {
            this.options &= ~options;
            return this;
        }

        #endregion Internal Methods
    }

}

#endif
