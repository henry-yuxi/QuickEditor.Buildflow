#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using Debug = LoggerUtils;

    //#if ODIN_INSPECTOR
    //    [Sirenix.OdinInspector.HideMonoScript()]
    //    [Sirenix.OdinInspector.TypeInfoBox("插件SDK配置, 用于自动化打包")]
    //#endif
    [System.Serializable]
    internal class ProjectSDKConfigure : QuickScriptableObject<ProjectSDKConfigure>
    {
#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Channel")]
        [Sirenix.OdinInspector.LabelText("渠道列表")]
        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 5, ShowIndexLabels = true, ListElementLabelName = "presetName")]
#endif
        public List<ChannelPresetSettings> Channels = new List<ChannelPresetSettings>();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Plugin")]
        [Sirenix.OdinInspector.LabelText("插件列表")]
        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 5, ShowIndexLabels = true, ListElementLabelName = "presetName")]
#endif
        public List<PluginPresetSettings> Plugin = new List<PluginPresetSettings>() { };

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(10)]
        [Sirenix.OdinInspector.HorizontalGroup("Group1", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("保存配置", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif

        public void SaveConfigure()
        {
            Current.Save();
        }

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(10)]
        [Sirenix.OdinInspector.HorizontalGroup("Group1", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("Open(Player Settings)", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
        public void OpenPlayerSetting()
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
        }

        [System.Serializable]
        internal class ChannelPresetSettings : AbstractPresetSettings
        {
            public string channel = "default";

            public BuildTarget buildTarget = BuildTarget.NoTarget;

            public string companyName = "DefaultCompany";

            public string productName = "QuickUSDKDemo";

            public string bundleIdentifier = "com.quickusdk.demo";

            //public string defaultIcon;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.ValueDropdown("@GetKeystoreList()", ExcludeExistingValuesInList = true)]
#endif
            public string keystore = "";

            //public string plugin = "";

            protected List<string> GetKeystoreList()
            {
                return ProjectCommonConfigure.Current.Keystores.Select(k => k.group).ToList();
            }

            protected BuildTargetGroup activeTargetGroup { get { return BuildPipeline.GetBuildTargetGroup(buildTarget); } }

            protected ProjectCommonConfigure.KeystoreSettings curKeystoreSettings
            {
                get
                {
                    if (string.IsNullOrEmpty(keystore) || ProjectCommonConfigure.Current == null || ProjectCommonConfigure.Current.Keystores == null)
                    {
                        return null;
                    }
                    return ProjectCommonConfigure.Current.Keystores.Find(k => k.group == keystore);
                }
            }

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(5)]
            [Sirenix.OdinInspector.HorizontalGroup("HorizontalGroup1", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("应用配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void ApplySettings()
            {
                PlayerSettingsResolver.SetCompanyName(companyName);
                PlayerSettingsResolver.SetProductName(productName);
                PlayerSettingsResolver.SetApplicationIdentifier(buildTarget == BuildTarget.NoTarget ? BuildTargetGroup.Unknown : activeTargetGroup, bundleIdentifier);

                if (curKeystoreSettings != null)
                {
                    curKeystoreSettings.ApplySettings();
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(5)]
            [Sirenix.OdinInspector.HorizontalGroup("HorizontalGroup1", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("生成目录", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void GenerateCatalog()
            {
                BuildPipelineCommonTools.FileUtils.CheckDir(BuildPipelineAsset.GetExternalPluginLibsSourcePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel));
                string resPath = BuildPipelineAsset.GetExternalPluginResSourcePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel);
                BuildPipelineCommonTools.FileUtils.CheckDir(Path.Combine(resPath, GlobalToolConfigure.Current.SDK.DefaultIconFilterSearchPath));
                BuildPipelineCommonTools.FileUtils.CheckDir(Path.Combine(resPath, GlobalToolConfigure.Current.SDK.SplashLogosFilterSearchPath));
                BuildPipelineCommonTools.FileUtils.CheckDir(Path.Combine(resPath, GlobalToolConfigure.Current.SDK.SplashScreenFilterSearchPath));
                Debug.Log("插件目录生成成功");
            }

            #region 拷贝资源 应用资源

            private string mExternalPluginsPath, mAssetPluginsPath = string.Empty;

            private string mExternalResPath, mInternalPackagingCacheResPath = string.Empty;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(5)]
            [Sirenix.OdinInspector.HorizontalGroup("HorizontalGroup2", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("拷贝资源", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void CopyResources()
            {
                CopyExternalPluginsFolder();
                CopyExternalResFolder();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            protected void CopyExternalPluginsFolder()
            {
                if (string.IsNullOrEmpty(channel)) { return; }
                mAssetPluginsPath = BuildPipelineAsset.GetInternalPluginLibsCachePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel);
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    mExternalPluginsPath = BuildPipelineAsset.GetExternalPluginLibsSourcePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel);
                    BuildPipelineCommonTools.FileUtils.CopyFolder(mExternalPluginsPath, mAssetPluginsPath);
                }
                else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {
                    mExternalPluginsPath = BuildPipelineAsset.GetExternalPluginLibsSourcePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel);
                    BuildPipelineCommonTools.FileUtils.CopyFolder(mExternalPluginsPath, mAssetPluginsPath);
                }
            }

            protected void CopyExternalResFolder()
            {
                if (string.IsNullOrEmpty(channel)) { return; }
                mInternalPackagingCacheResPath = BuildPipelineAsset.InternalPackagingCacheResPath;
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    mExternalResPath = BuildPipelineAsset.GetExternalPluginResSourcePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel);
                    BuildPipelineCommonTools.FileUtils.CopyFolder(mExternalResPath, mInternalPackagingCacheResPath);
                }
                else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {
                    mExternalResPath = BuildPipelineAsset.GetExternalPluginResSourcePath(EditorUserBuildSettings.activeBuildTarget.ToString(), channel);
                    BuildPipelineCommonTools.FileUtils.CopyFolder(mExternalResPath, mInternalPackagingCacheResPath);
                }
            }

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(5)]
            [Sirenix.OdinInspector.HorizontalGroup("HorizontalGroup2", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("设置资源(Icon,Splash)", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void ApplyResources()
            {
                Current.Save();
                if (GlobalToolConfigure.Current != null && GlobalToolConfigure.Current.SDK != null)
                {
                    BuildPipelineCommonTools.SDKUtils.SetDefaultIcon();
#pragma warning disable 0162
                    if (GlobalToolConfigure.Current.SDK.IsSplashLogos)
                    {
                        BuildPipelineCommonTools.SDKUtils.SetSplashLogos();
                    }
                    else
                    {
                        BuildPipelineCommonTools.SDKUtils.SetSplashScreen();
                    }
                }
            }

            #endregion 拷贝资源 应用资源
        }

        [System.Serializable]
        public class PluginPresetSettings : AbstractPresetSettings
        {
            public string pluginName = "";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.ReadOnly()]
#endif
            public int pluginType = 0;

            public int pluginID = 0;

            public string description = "";

            public string className = "";

            public string version = "";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.ShowInInspector()]
            [Sirenix.OdinInspector.DictionaryDrawerSettings()]
#endif
            public Dictionary<string, string> keyValuePairs = new Dictionary<string, string>() {
                { "appID","" },
                { "appKey","" },
                { "appSecret","" },
            };
        }
    }
}

#endif
