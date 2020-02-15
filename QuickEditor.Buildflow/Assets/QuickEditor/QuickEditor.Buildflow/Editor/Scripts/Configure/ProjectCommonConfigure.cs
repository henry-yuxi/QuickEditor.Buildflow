#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Attributes;
    using QuickEditor.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    //#if ODIN_INSPECTOR
    //    [Sirenix.OdinInspector.TypeInfoBox("项目基础配置, 方便快速设置")]
    //#endif
    [System.Serializable]
    public class ProjectCommonConfigure : QuickScriptableObject<ProjectCommonConfigure>
    {
#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Projects")]
        [Sirenix.OdinInspector.LabelText("Project列表")]
        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 2, ShowIndexLabels = true, ListElementLabelName = "presetName")]
#endif
        public List<ProjectPresetSettings> Projects = new List<ProjectPresetSettings>();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Symbols")]
        [Sirenix.OdinInspector.LabelText("Scripting Define Symbols")]
#endif
        public ProjectSymbolConfigure ProjectSymbol = new ProjectSymbolConfigure();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Keystores")]
        [Sirenix.OdinInspector.LabelText("Keystore列表")]
        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 5, ShowIndexLabels = true, ListElementLabelName = "presetName")]
#endif
        public List<KeystoreSettings> Keystores = new List<KeystoreSettings>();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Signings")]
        [Sirenix.OdinInspector.LabelText("Signing列表")]
        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 5, ShowIndexLabels = true, ListElementLabelName = "presetName")]
#endif
        public List<SigningSettings> Signings = new List<SigningSettings>();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(10)]
        [Sirenix.OdinInspector.HorizontalGroup("Group1", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("保存配置", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
        public void SaveConfigure()
        {
            Current.Save();
        }

        [System.Serializable]
        public class ProjectPresetSettings : AbstractPresetSettings
        {
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
#endif
            public string group = "default";

#if ODIN_INSPECTOR

            //[Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.LabelText("Resolution and Presentation")]
#endif
            public ProjectOrientationSettings Orientation = new ProjectOrientationSettings();

#if ODIN_INSPECTOR

            //[Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.LabelText("Android Player Settings")]
#endif
            public AndroidPlayerSettings Android = new AndroidPlayerSettings();

#if ODIN_INSPECTOR

            //[Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.LabelText("iOS Player Settings")]
#endif
            public iOSPlayerSettings iOS = new iOSPlayerSettings();

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(10)]
            [Sirenix.OdinInspector.FoldoutGroup("Options")]
            [Sirenix.OdinInspector.ResponsiveButtonGroup("Options/Group1")]
            [Sirenix.OdinInspector.Button("覆盖配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void DuplicateSettings()
            {
                Current.Save();
            }

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(10)]
            [Sirenix.OdinInspector.FoldoutGroup("Options")]
            [Sirenix.OdinInspector.ResponsiveButtonGroup("Options/Group1")]
            [Sirenix.OdinInspector.Button("应用配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void ApplySettings()
            {
                Orientation.ApplyPlayerSettings();
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    Android.ApplyPlayerSettings();
                }
                else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {
                    iOS.ApplyPlayerSettings();
                }
            }

            [System.Serializable]
            public class ProjectOrientationSettings
            {
                public UIOrientation defaultOrientation = UIOrientation.AutoRotation;
                public bool allowedPortrait = false;
                public bool allowedPortraitUpsideDown = false;
                public bool allowedLandscapeRight = false;
                public bool allowedLandscapeLeft = false;

                public void ApplyPlayerSettings()
                {
                    PlayerSettingsResolver.SetScreenOrientation(defaultOrientation, allowedPortrait, allowedPortraitUpsideDown, allowedLandscapeRight, allowedLandscapeLeft);
                }
            }

            [System.Serializable]
            public class AndroidPlayerSettings : DefaultPlayerSettings
            {
                public AndroidSdkVersions minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
                public AndroidSdkVersions targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
                public AndroidPreferredInstallLocation preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
                public bool forceInternetPermission = true;
                public bool forceSDCardPermission = true;
                public bool androidIsGame = true;

                //public AndroidBuildType androidBuildType = AndroidBuildType.Release;

#if UNITY_2018_1_OR_NEWER
                public AndroidArchitecture targetArchitectures = AndroidArchitecture.All;
#else
                public AndroidTargetDevice targetDevice = AndroidTargetDevice.FAT;
#endif

#if UNITY_2018_3_OR_NEWER
#if ODIN_INSPECTOR

                [Sirenix.OdinInspector.BoxGroup("Other Settings")]
                public ManagedStrippingLevel strippingLevel = ManagedStrippingLevel.Disabled;

#endif
#endif

                public void ApplyPlayerSettings()
                {
                    PlayerSettingsResolver.ApplySettings(graphicsJobs: false, protectGraphicsMemory: true);
                    //PlayerSettingsResolver.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
                    PlayerSettingsResolver.Android.ApplySettings(androidIsGame: androidIsGame, minSdkVersion: minSdkVersion, targetSdkVersion: targetSdkVersion, preferredInstallLocation: preferredInstallLocation, forceInternetPermission: forceInternetPermission, forceSDCardPermission: forceSDCardPermission);
#if UNITY_2018_1_OR_NEWER
                    PlayerSettingsResolver.Android.ApplySettings(targetArchitectures: targetArchitectures);
#else
                    PlayerSettingsResolver.Android.ApplySettings(targetDevice: targetDevice);
#endif

#if UNITY_2018_3_OR_NEWER
                    PlayerSettingsResolver.SetManagedStrippingLevel(BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), strippingLevel);
#endif
                }
            }

            [System.Serializable]
            public class iOSPlayerSettings : DefaultPlayerSettings
            {
                public iOSSdkVersion targetSDK = iOSSdkVersion.DeviceSDK;
                public iOSTargetDevice targetDevice = iOSTargetDevice.iPhoneAndiPad;
                public string targetOSVersionString = "8.0";

                public void ApplyPlayerSettings()
                {
                    PlayerSettingsResolver.iOS.ApplySettings(targetSDK: targetSDK, targetDevice: targetDevice, targetOSVersionString: targetOSVersionString);
                }
            }

            [System.Serializable]
            public class DefaultPlayerSettings
            {
            }
        }

        [System.Serializable]
        public class ProjectSymbolConfigure
        {
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.LabelText("项目宏配置")]
            [Sirenix.OdinInspector.TableList(NumberOfItemsPerPage = 5, ShowIndexLabels = false, DrawScrollView = true)]
#endif
            public List<ProjectSymbolReference> Settings = new List<ProjectSymbolReference> { };

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(5)]
            [Sirenix.OdinInspector.HorizontalGroup("Group1", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("覆盖配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void DuplicateSettings()
            {
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(target);
                string[] symbols = BuildPipelineCommonTools.ScriptingDefineSymbolsUtils.SplitScriptingDefineSymbols(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup));
                foreach (var symbol in symbols)
                {
                    if (symbol == null) { continue; }
                    if (!Settings.Exists(m => m.scriptingDefine == symbol))
                    {
                        Settings.Add(new ProjectSymbolReference()
                        {
                            scriptingDefine = symbol
                        });
                    }
                }
                Current.Save();
            }

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(5)]
            [Sirenix.OdinInspector.HorizontalGroup("Group1", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("应用配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void ApplySettings()
            {
                var array = Settings.FindAll(s => s.include).Select(s => s.scriptingDefine).ToArray();
                if (array == null || array.Length == 0)
                {
                    Debug.LogError("未选择任何ScriptingDefine, 设置失败");
                    return;
                }

                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(target);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, BuildPipelineCommonTools.ScriptingDefineSymbolsUtils.MergeScriptingDefineSymbols(array));//宏定义的设置
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            [System.Serializable]
            public class ProjectSymbolReference
            {
                public string presetName = "New Preset";
                public bool include = true;
                public string scriptingDefine;
            }
        }

        [Serializable]
        public class SigningSettings
        {
            public string presetName = "New Preset";

            public string group = "default";
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Signing Settings")]
#endif
            public bool automaticSigning;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Signing Settings")]
#endif
            public string appleDeveloperTeamID;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Signing Settings")]
#endif
            public string provisioningProfileID;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Signing Settings")]
#endif
            public ProvisioningProfileType provisioningProfileType = ProvisioningProfileType.Automatic;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(10)]
            [Sirenix.OdinInspector.FoldoutGroup("Options")]
            [Sirenix.OdinInspector.ResponsiveButtonGroup("Options/Group1")]
            [Sirenix.OdinInspector.Button("应用配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void ApplySettings()
            {
                PlayerSettingsResolver.iOS.ApplySigningSettings(automaticSigning: automaticSigning, appleDeveloperTeamID: appleDeveloperTeamID, provisioningProfileID: provisioningProfileID, provisioningProfileType: provisioningProfileType);
            }
        }

        [System.Serializable]
        public class KeystoreSettings
        {
            public string presetName = "New Preset";

            public string group = "default";
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Keystore Settings")]
#endif
            [Tooltip("Keystore file path.")]
            [FilePath(PathType.ProjectPath, "Source Files", "keystore", Editable = true)]
            public string keystoreFile = "Assets/Plugins/Buildflow/Deploy/Keystore/quickusdk.keystore";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Keystore Settings")]
#endif
            [Tooltip("Keystore password.")]
            public string keystorePassword = "123456";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Keystore Settings")]
#endif
            [Tooltip("Keystore alias name.")]
            public string keystoreAliasName = "sdk";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Indent()]
            [Sirenix.OdinInspector.FoldoutGroup("Keystore Settings")]
#endif
            [Tooltip("Keystore alias password.")]
            public string keystoreAliasPassword = "123456";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(10)]
            [Sirenix.OdinInspector.FoldoutGroup("Options")]
            [Sirenix.OdinInspector.ResponsiveButtonGroup("Options/Group1")]
            [Sirenix.OdinInspector.Button("应用配置", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void ApplySettings()
            {
                PlayerSettingsResolver.Android.ApplyKeystoreSettings(keystoreFile, keystorePassword, keystoreAliasName, keystoreAliasPassword);
            }
        }
    }
}

#endif
