#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    internal sealed partial class UnityEditorResolver
    {
        internal sealed partial class BundleVersion
        {
            public static string PrettyVersion { get; private set; }
            public static int? BuildNumber { get; private set; }

            public static void ApplySettings(string version, int? buildNumber)
            {
                PrettyVersion = version;
                BuildNumber = buildNumber;
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
#if UNITY_3 || UNITY_4
                    case BuildTarget.iPhone:
#else
                    case BuildTarget.iOS:
                        SetiOSBundleSettings();
                        break;
#endif
                    case BuildTarget.Android:
                        SetAndroidBundleSettings();
                        break;

                }
            }

            private static void SetiOSBundleSettings()
            {
                if (!string.IsNullOrEmpty(PrettyVersion))
                {
                    UnityEditor.PlayerSettings.bundleVersion = PrettyVersion;
                }
#if UNITY_5_3_OR_NEWER
                if (BuildNumber != null)
                {
                    UnityEditor.PlayerSettings.iOS.buildNumber = BuildNumber.Value.ToString();
                }
#endif
            }

            private static void SetAndroidBundleSettings()
            {
                if (!string.IsNullOrEmpty(PrettyVersion))
                {
                    UnityEditor.PlayerSettings.bundleVersion = PrettyVersion;
                }
                if (BuildNumber != null)
                {
                    UnityEditor.PlayerSettings.Android.bundleVersionCode = BuildNumber.Value;
                }
            }
        }

        internal sealed partial class EditorBuildSettings
        {
            internal static string[] GetEnabledScenes()
            {
                List<string> scenes = new List<string>();
                foreach (EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
                {
                    if (!scene.enabled) continue;
                    scenes.Add(scene.path);
                }
                return scenes.ToArray();
            }

            internal static void SwitchActiveBuildTarget(BuildTargetGroup targetGroup, BuildTarget target, Action action)
            {
                if (EditorUserBuildSettings.activeBuildTarget == target)
                {
                    if (action != null) { action(); }
                }
                else
                {
#if UNITY_2017_1_OR_NEWER
                    if (EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target))
#else
                    if (EditorUserBuildSettings.SwitchActiveBuildTarget(target))
#endif
                    {
                        if (action != null) { action(); }
                    }
                }
            }

            internal static void ApplySettings(bool release = true)
            {
                //EditorUserBuildSettings.allowDebugging = !release;
                EditorUserBuildSettings.connectProfiler = !release;
                EditorUserBuildSettings.development = !release;
            }

            internal static void ApplySettings(AndroidBuildSystem androidBuildSystem = AndroidBuildSystem.Gradle)
            {
                EditorUserBuildSettings.androidBuildSystem = androidBuildSystem;
            }
        }

        internal sealed partial class PlayerSettings
        {
            internal static void ApplySettings(
                bool graphicsJobs = false,
                bool protectGraphicsMemory = true
                )
            {
                UnityEditor.PlayerSettings.graphicsJobs = graphicsJobs;
                UnityEditor.PlayerSettings.protectGraphicsMemory = protectGraphicsMemory;
            }

            internal static void SetApiCompatibilityLevel(BuildTargetGroup targetGroup, ApiCompatibilityLevel apiCompatibility)
            {
#if UNITY_5_6_OR_NEWER
                UnityEditor.PlayerSettings.SetApiCompatibilityLevel(targetGroup, apiCompatibility);
#else
            PlayerSettings.apiCompatibilityLevel = apiCompatibility;
#endif
            }

            internal static void SetApplicationIdentifier(BuildTargetGroup targetGroup, string bundleIdentifier)
            {
#if UNITY_5_6_OR_NEWER
                if (targetGroup == BuildTargetGroup.Unknown)
                {
                    UnityEditor.PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleIdentifier);
                    UnityEditor.PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleIdentifier);
                    UnityEditor.PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, bundleIdentifier);
                }
                else
                {
                    UnityEditor.PlayerSettings.SetApplicationIdentifier(targetGroup, bundleIdentifier);
                }

#else
            PlayerSettings.bundleIdentifier = bundleIdentifier;
#endif
            }

            internal static void SetCompanyName(string companyName)
            {
                UnityEditor.PlayerSettings.companyName = companyName;
            }

            internal static void SetProductName(string productName)
            {
                UnityEditor.PlayerSettings.productName = productName;
            }

#if UNITY_2018_3_OR_NEWER
        internal static void SetManagedStrippingLevel(BuildTargetGroup targetGroup, ManagedStrippingLevel strippingLevel = ManagedStrippingLevel.Disabled)
        {
            PlayerSettings.SetManagedStrippingLevel(targetGroup, strippingLevel);
        }
#endif

            internal static void SetScreenOrientation(UIOrientation defaultOrientation, bool allowedPortrait, bool allowedPortraitUpsideDown, bool allowedLandscapeRight, bool allowedLandscapeLeft)
            {
                UnityEditor.PlayerSettings.defaultInterfaceOrientation = defaultOrientation;
                UnityEditor.PlayerSettings.allowedAutorotateToPortrait = allowedPortrait;
                UnityEditor.PlayerSettings.allowedAutorotateToPortraitUpsideDown = allowedPortraitUpsideDown;
                UnityEditor.PlayerSettings.allowedAutorotateToLandscapeRight = allowedLandscapeRight;
                UnityEditor.PlayerSettings.allowedAutorotateToLandscapeLeft = allowedLandscapeLeft;
            }

            //internal static void SetAppIcon(BuildTarget buildTarget, Texture2D icon)
            //{
            //    BuildTargetGroup buildTargetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(buildTarget);
            //    int[] iconSize = UnityEditor.PlayerSettings.GetIconSizesForTargetGroup(buildTargetGroup);
            //    Texture2D[] textureArray = new Texture2D[iconSize.Length];
            //    for (int i = 0; i < textureArray.Length; i++)
            //    {
            //        textureArray[i] = icon;
            //    }
            //    UnityEditor.PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, textureArray);
            //    AssetDatabase.SaveAssets();
            //    AssetDatabase.Refresh();
            //}

            internal static void SetSplashScreen(Color backgroundColor,
                bool show = true, bool showUnityLogo = false,
                UnityEditor.PlayerSettings.SplashScreen.DrawMode drawMode = UnityEditor.PlayerSettings.SplashScreen.DrawMode.AllSequential,
                UnityEditor.PlayerSettings.SplashScreen.UnityLogoStyle unityLogoStyle = UnityEditor.PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark)
            {
                UnityEditor.PlayerSettings.SplashScreen.show = show;
                UnityEditor.PlayerSettings.SplashScreen.backgroundColor = backgroundColor;
                UnityEditor.PlayerSettings.SplashScreen.drawMode = drawMode;
                UnityEditor.PlayerSettings.SplashScreen.showUnityLogo = showUnityLogo;
            }

            internal static void SetSplashScreen(UnityEditor.PlayerSettings.SplashScreenLogo[] logos)
            {
                UnityEditor.PlayerSettings.SplashScreen.logos = logos;
            }

            internal static void SetSplashScreen(Sprite background, Sprite backgroundPortrait = null)
            {
                UnityEditor.PlayerSettings.SplashScreen.background = background;
                UnityEditor.PlayerSettings.SplashScreen.backgroundPortrait = backgroundPortrait;
            }

            internal sealed partial class Android
            {
                internal static void ApplyExportSettings(bool exportAsGoogleAndroidProject = false, bool buildApkPerCpuArchitecture = false, bool buildAppBundle = false, bool useAPKExpansionFiles = false)
                {
                    UnityEditor.PlayerSettings.Android.useAPKExpansionFiles = useAPKExpansionFiles;//是否使用obb分离模式
                    UnityEditor.PlayerSettings.Android.buildApkPerCpuArchitecture = buildApkPerCpuArchitecture;
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = exportAsGoogleAndroidProject;
#if UNITY_2018_3_OR_NEWER
                EditorUserBuildSettings.buildAppBundle = buildAppBundle;
#endif
                }

                internal static void ApplyKeystoreSettings(string keystoreFile, string keystorePassword, string keystoreAliasName, string keystoreAliasPassword)
                {
                    UnityEditor.PlayerSettings.Android.keystoreName = keystoreFile;
                    UnityEditor.PlayerSettings.Android.keystorePass = keystorePassword;
                    UnityEditor.PlayerSettings.Android.keyaliasName = keystoreAliasName;
                    UnityEditor.PlayerSettings.Android.keyaliasPass = keystoreAliasPassword;
                }

#if UNITY_2018_1_OR_NEWER

                internal static void ApplySettings(AndroidArchitecture targetArchitectures = AndroidArchitecture.All)
                {
                    UnityEditor.PlayerSettings.Android.targetArchitectures = targetArchitectures;
                }

#else
            internal static void ApplySettings(AndroidTargetDevice targetDevice = AndroidTargetDevice.FAT)
            {
                PlayerSettings.Android.targetDevice = targetDevice;
            }
#endif

                internal static void ApplySettings(
                    AndroidBuildType androidBuildType = AndroidBuildType.Release,
                    AndroidSdkVersions minSdkVersion = AndroidSdkVersions.AndroidApiLevel18,
                    AndroidSdkVersions targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto,
                    AndroidPreferredInstallLocation preferredInstallLocation = AndroidPreferredInstallLocation.Auto,
                    bool forceInternetPermission = true,
                    bool forceSDCardPermission = true,
                    bool androidIsGame = true
                    )
                {
                    UnityEditor.PlayerSettings.Android.androidIsGame = androidIsGame;
                    UnityEditor.PlayerSettings.Android.minSdkVersion = minSdkVersion;
                    UnityEditor.PlayerSettings.Android.targetSdkVersion = targetSdkVersion;
                    UnityEditor.PlayerSettings.Android.preferredInstallLocation = preferredInstallLocation;
                    UnityEditor.PlayerSettings.Android.forceInternetPermission = forceInternetPermission;
                    UnityEditor.PlayerSettings.Android.forceSDCardPermission = forceSDCardPermission;
                }
            }

            internal sealed partial class iOS
            {
                internal static void ApplySigningSettings(bool automaticSigning, string appleDeveloperTeamID, string provisioningProfileID, ProvisioningProfileType provisioningProfileType)
                {
                    UnityEditor.PlayerSettings.iOS.appleEnableAutomaticSigning = automaticSigning;
                    UnityEditor.PlayerSettings.iOS.appleDeveloperTeamID = appleDeveloperTeamID;
                    UnityEditor.PlayerSettings.iOS.iOSManualProvisioningProfileID = provisioningProfileID;
                    UnityEditor.PlayerSettings.iOS.iOSManualProvisioningProfileType = provisioningProfileType;
                }

                internal static void ApplySettings(iOSSdkVersion targetSDK = iOSSdkVersion.DeviceSDK, iOSTargetDevice targetDevice = iOSTargetDevice.iPhoneAndiPad, string targetOSVersionString = "8.0")
                {
                    UnityEditor.PlayerSettings.iOS.sdkVersion = targetSDK;
                    UnityEditor.PlayerSettings.iOS.targetDevice = targetDevice;
                    UnityEditor.PlayerSettings.iOS.targetOSVersionString = targetOSVersionString;
                }

                internal static void SetiPhoneLaunchScreenType(iOSLaunchScreenType iOSLaunchScreenType = iOSLaunchScreenType.None)
                {
                    UnityEditor.PlayerSettings.iOS.SetiPhoneLaunchScreenType(iOSLaunchScreenType);
                }

            }

        }

        internal sealed partial class ScriptingDefineSymbols
        {
            private static readonly char[] SPLIT_DIVIDER = new char[] { ';' };

            private const string JOIN_SEPARATOR = ";";

            public static void AddScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string addDefines)
            {
                if (string.IsNullOrEmpty(addDefines)) { return; }
                string[] addDefineArray = SplitScriptingDefineSymbols(addDefines);
                if (addDefineArray == null || addDefineArray.Count() == 0) { return; }
                bool changed = false;
                string mScriptingDefineSymbols = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                var mDefineSymbolList = SplitScriptingDefineSymbols(mScriptingDefineSymbols).ToList();
                foreach (var symbol in addDefineArray)
                {
                    if (!mDefineSymbolList.Contains(symbol))
                    {
                        mDefineSymbolList.Add(symbol);
                        changed = true;
                    }
                }
                if (changed)
                {
                    SetScriptingDefineSymbols(buildTargetGroup, mDefineSymbolList.ToArray());
                }
            }

            public static void RemoveScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string removeDefines)
            {
                if (string.IsNullOrEmpty(removeDefines)) { return; }
                string[] removeDefineArray = SplitScriptingDefineSymbols(removeDefines);
                if (removeDefineArray == null || removeDefineArray.Count() == 0) { return; }
                bool changed = false;
                string mScriptingDefineSymbols = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                var mDefineSymbolList = SplitScriptingDefineSymbols(mScriptingDefineSymbols).ToList();
                foreach (var macro in removeDefineArray)
                {
                    if (mDefineSymbolList.Contains(macro))
                    {
                        mDefineSymbolList.RemoveAll((symbol) => symbol == macro);
                        changed = true;
                    }
                }
                if (changed)
                {
                    SetScriptingDefineSymbols(buildTargetGroup, mDefineSymbolList.ToArray());
                }
            }

            public static string MergeScriptingDefineSymbols(string[] defineSymbols)
            {
                string merge = string.Join(JOIN_SEPARATOR, defineSymbols);
                return merge;
            }

            public static string[] SplitScriptingDefineSymbols(string defineSymbols)
            {
                string[] split = defineSymbols.Split(SPLIT_DIVIDER, StringSplitOptions.RemoveEmptyEntries);
                return split;
            }

            public static void SetScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string[] symbols)
            {
                string symbos = MergeScriptingDefineSymbols(symbols);
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbos);
            }
        }
    }

}

#endif
