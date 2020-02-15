#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Attributes;
    using QuickEditor.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_2018_OR_NEWER
    using UnityEditor.iOS.Xcode;
#else

    using UnityEditor.iOS.Xcode.Custom;

#endif

    //#if ODIN_INSPECTOR
    //    [Sirenix.OdinInspector.TypeInfoBox("Xcode配置, 用于打包后自动化设置")]
    //#endif
    [System.Serializable]
    public class ProjectXcodeConfigure : QuickScriptableObject<ProjectXcodeConfigure>
    {
#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Global")]
        [Sirenix.OdinInspector.LabelText("全局配置")]
#endif
        public GlobalXcodePresetSettings Global = new GlobalXcodePresetSettings();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 2, ShowIndexLabels = true, ListElementLabelName = "presetName", Expanded = true)]
        [Sirenix.OdinInspector.TabGroup("SDK")]
        [Sirenix.OdinInspector.LabelText("SDK列表")]
#endif
        public List<XcodeProjectPresetSettings> Settings = new List<XcodeProjectPresetSettings>();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(10)]
        [Sirenix.OdinInspector.HorizontalGroup("Group1", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("保存配置", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif

        public void SaveConfigure()
        {
            Current.Save();
        }
    }

    [System.Serializable]
    public class GlobalXcodePresetSettings
    {
        public bool EnableATS = false; //NSAppTransportSecurity 支持https兼容http
        public bool EnableBitCode = false; //BitCode 是否开启
        public bool EnableCppExceptions = true;
        public bool EnableCppRtti = true;
        public bool EnableObjcExceptions = true;
        public bool EnableGameCenter = false;
        public bool EnableStatusBar = false; //显示进度条
        public bool EnableFullScreen = true;
        public bool NeedToDeleteLaunchiImages = true; //是否删除开场动画
    }

    [System.Serializable]
    public class XcodeProjectPresetSettings : AbstractPresetSettings
    {
        public string group = "default";

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Title("Xcode配置")]
#endif
        public XcodeInfoPlistSettings InfoPlist = new XcodeInfoPlistSettings();

        public XcodePBXProjectSettings PBXProject = new XcodePBXProjectSettings();
        public XcodeCodeDataSettings ModifyCode = new XcodeCodeDataSettings();

        [Serializable]
        public class XcodeInfoPlistSettings
        {
            public List<string> ApplicationQueriesSchemes = new List<string>();//白名单设置
            public List<string> Domains = new List<string>();//域名白名单设置
            public List<string> BackgroundModes = new List<string>(); //后台需要开启的功能
            public List<Bundle> Bundles = new List<Bundle>();
            public List<BundleURLData> BundleURLTypes = new List<BundleURLData>();//第三方平台URL Scheme

            public List<string> PrivacySensiticeData = new List<string>();//iOS10新的特性
            public List<BuildProperty> BuildPropertys = new List<BuildProperty>();
            public List<PermissionSet> Permissions = new List<PermissionSet>();
            public List<string> CopyFiles = new List<string>();

            [Serializable]
            public class BuildProperty
            {
                public string Name;
                public string Value;
            }

            [Serializable]
            public class Bundle
            {
                [Folder(PathType.ProjectPath, ".bundle", Editable = true)]
                public string Path;
            }

            [Serializable]
            public class BundleURLData
            {
                public string BundleTypeRole = "Editor";
                public string BundleURLName;
                public List<string> BundleURLSchmes = new List<string>();
            }

            [Serializable]
            public class PermissionSet
            {
                public string permissionName;
                public string describe;
            }
        }

        [Serializable]
        public class XcodePBXProjectSettings
        {
            public List<CapabilityData> Capabilities = new List<CapabilityData>();
            public List<CompileFile> CompileFiles = new List<CompileFile>();

            public List<CompilerFlagsSet> CompilerFlagsSetList = new List<CompilerFlagsSet>()
            {
                /*new CompilerFlagsSet ("-fno-objc-arc", new List<string> () {"Plugin/Plugin.mm"})*/ //实例，请勿删除
            };

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.LabelText("Frameworks(System)")]
#endif
            public List<Framework> Frameworks = new List<Framework> { /*"Social.framework",*/ };  //系统Framework

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.LabelText("Libs(System)")]
#endif
            public List<Library> Librarys = new List<Library> { /*"libz.tbd,*/ };//系统Library

            public List<Framework> EmbedFrameworks = new List<Framework>() { };

            public List<string> LinkerFlagArray = new List<string>() { /*"-ObjC", "-all_load"*/ };

            [Folder(PathType.ProjectPath, Editable = true)]
            public string CopyDirectoryRootPath = string.Empty;

            public List<string> Folders = new List<string>() { };
            public List<string> FrameworkSearchPaths = new List<string>() { /*"$(inherited)", "$(PROJECT_DIR)/Frameworks"*/ };
            public List<string> LibrarySearchPaths = new List<string>();
            public List<string> HeaderSearchPaths = new List<string>();

            [Serializable]
            public class CompilerFlagsSet
            {
                public string Flags;
                public List<string> TargetPaths;

                public CompilerFlagsSet(string flags, List<string> targetPaths)
                {
                    Flags = flags;
                    TargetPaths = targetPaths;
                }
            }

            [Serializable]
            public class CompileFile
            {
                [FilePath(PathType.ProjectPath, "Source Files", "h,m,mm,cpp,c", Editable = true)]
                public string File;

                [Folder(PathType.ProjectPath, Editable = true)]
                public string HeadersDirectory;

                public string CompileFlags;
            }

            [Serializable]
            public class Framework
            {
                [Folder(PathType.ProjectPath, ".framework", Editable = true)]
                public string Path;

                public LinkStatus Status;

                public bool IsWeak
                {
                    get { return this.Status == LinkStatus.Optional; }
                }
            }

            public enum LinkStatus
            {
                Required, //强引用
                Optional //弱引用
            }

            [Serializable]
            public class Library
            {
                [FilePath(PathType.ProjectPath, "Static Library", "a,dylib,tbd", Editable = true)]
                public string File;
            }

            #region iOS10新的特性

            public enum NValueType
            {
                String,
                Int,
                Bool,
            }

            [Serializable]
            public struct PrivacySensiticeData
            {
                [SerializeField]
                public string key;

                [SerializeField]
                public string value;

                [SerializeField]
                public NValueType type;

                public PrivacySensiticeData(string key, string value, NValueType type)
                {
                    this.key = key;
                    this.value = value;
                    this.type = type;
                }
            }

            #endregion iOS10新的特性
        }

        [Serializable]
        public class XcodeCodeDataSettings
        {
            public List<CodeDataSet> CodeDatas = new List<CodeDataSet>();

            [Serializable]
            public class CodeDataSet
            {
                public string targetFile;
                public string AppendMark;
                public List<string> AppendCodes;
                public string ReplaceMark;
                public List<string> ReplaceCodes;
            }
        }

        #region 相关结构

        public enum CapabilityType
        {
            ApplePay = 1,
            WirelessAccessoryConfiguration = 11,
            Wallet = 21,
            Siri = 31,
            PushNotifications = 41,
            PersonalVPN = 51,
            Maps = 61,
            InterAppAudio = 71,
            InAppPurchase = 81,
            KeychainSharing = 91,
            HomeKit = 101,
            HealthKit = 111,
            GameCenter = 121,
            DataProtection = 131,
            BackgroundModes = 141,
            AssociatedDomains = 151,
            AppGroups = 161,
            iCloud = 171
        }

        [Serializable]
        public class CapabilityData
        {
            public CapabilityType Type;
            public bool Enable;
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.ShowIf("Type", CapabilityType.PushNotifications)]
#endif
            public string EntitlementsFilePath = null;

            public bool AddOptionalFramework = false;

            protected PBXCapabilityType pBX;

            public PBXCapabilityType PBXCapability
            {
                get
                {
                    switch (Type)
                    {
                        case CapabilityType.ApplePay:
                            pBX = PBXCapabilityType.ApplePay;
                            break;

                        case CapabilityType.WirelessAccessoryConfiguration:
                            pBX = PBXCapabilityType.WirelessAccessoryConfiguration;
                            break;

                        case CapabilityType.Wallet:
                            pBX = PBXCapabilityType.Wallet;
                            break;

                        case CapabilityType.Siri:
                            pBX = PBXCapabilityType.Siri;
                            break;

                        case CapabilityType.PushNotifications:
                            pBX = PBXCapabilityType.PushNotifications;
                            break;

                        case CapabilityType.PersonalVPN:
                            pBX = PBXCapabilityType.PersonalVPN;
                            break;

                        case CapabilityType.Maps:
                            pBX = PBXCapabilityType.Maps;
                            break;

                        case CapabilityType.InterAppAudio:
                            pBX = PBXCapabilityType.InterAppAudio;
                            break;

                        case CapabilityType.InAppPurchase:
                            pBX = PBXCapabilityType.InAppPurchase;
                            break;

                        case CapabilityType.KeychainSharing:
                            pBX = PBXCapabilityType.KeychainSharing;
                            break;

                        case CapabilityType.HomeKit:
                            pBX = PBXCapabilityType.HomeKit;
                            break;

                        case CapabilityType.HealthKit:
                            pBX = PBXCapabilityType.HealthKit;
                            break;

                        case CapabilityType.GameCenter:
                            pBX = PBXCapabilityType.GameCenter;
                            break;

                        case CapabilityType.DataProtection:
                            pBX = PBXCapabilityType.DataProtection;
                            break;

                        case CapabilityType.BackgroundModes:
                            pBX = PBXCapabilityType.BackgroundModes;
                            break;

                        case CapabilityType.AssociatedDomains:
                            pBX = PBXCapabilityType.AssociatedDomains;
                            break;

                        case CapabilityType.AppGroups:
                            pBX = PBXCapabilityType.AppGroups;
                            break;

                        case CapabilityType.iCloud:
                            pBX = PBXCapabilityType.iCloud;
                            break;

                        default:
                            break;
                    }
                    return pBX;
                }
            }
        }

        #endregion 相关结构
    }
}

#endif
