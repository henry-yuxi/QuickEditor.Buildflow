#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal sealed partial class PlayerSettingsResolver
    {
        internal static void ApplySettings(
            bool graphicsJobs = false,
            bool protectGraphicsMemory = true
            )
        {
            PlayerSettings.graphicsJobs = graphicsJobs;
            PlayerSettings.protectGraphicsMemory = protectGraphicsMemory;
        }

        internal static void SetApiCompatibilityLevel(BuildTargetGroup targetGroup, ApiCompatibilityLevel apiCompatibility)
        {
#if UNITY_5_6_OR_NEWER
            PlayerSettings.SetApiCompatibilityLevel(targetGroup, apiCompatibility);
#else
            PlayerSettings.apiCompatibilityLevel = apiCompatibility;
#endif
        }

        internal static void SetApplicationIdentifier(BuildTargetGroup targetGroup, string bundleIdentifier)
        {
#if UNITY_5_6_OR_NEWER
            if (targetGroup == BuildTargetGroup.Unknown)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleIdentifier);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleIdentifier);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, bundleIdentifier);
            }
            else
            {
                PlayerSettings.SetApplicationIdentifier(targetGroup, bundleIdentifier);
            }

#else
            PlayerSettings.bundleIdentifier = bundleIdentifier;
#endif
        }

        internal static void SetCompanyName(string companyName)
        {
            PlayerSettings.companyName = companyName;
        }

        internal static void SetProductName(string productName)
        {
            PlayerSettings.productName = productName;
        }

#if UNITY_2018_3_OR_NEWER
        internal static void SetManagedStrippingLevel(BuildTargetGroup targetGroup, ManagedStrippingLevel strippingLevel = ManagedStrippingLevel.Disabled)
        {
            PlayerSettings.SetManagedStrippingLevel(targetGroup, strippingLevel);
        }
#endif

        internal static void SetScreenOrientation(UIOrientation defaultOrientation, bool allowedPortrait, bool allowedPortraitUpsideDown, bool allowedLandscapeRight, bool allowedLandscapeLeft)
        {
            PlayerSettings.defaultInterfaceOrientation = defaultOrientation;
            PlayerSettings.allowedAutorotateToPortrait = allowedPortrait;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = allowedPortraitUpsideDown;
            PlayerSettings.allowedAutorotateToLandscapeRight = allowedLandscapeRight;
            PlayerSettings.allowedAutorotateToLandscapeLeft = allowedLandscapeLeft;
        }

        internal static void SetSplashScreen(Color backgroundColor,
            bool show = true, bool showUnityLogo = false,
            PlayerSettings.SplashScreen.DrawMode drawMode = PlayerSettings.SplashScreen.DrawMode.AllSequential,
            PlayerSettings.SplashScreen.UnityLogoStyle unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark)
        {
            PlayerSettings.SplashScreen.show = show;
            PlayerSettings.SplashScreen.backgroundColor = backgroundColor;
            PlayerSettings.SplashScreen.drawMode = drawMode;
            PlayerSettings.SplashScreen.showUnityLogo = showUnityLogo;
        }

        internal static void SetSplashScreen(PlayerSettings.SplashScreenLogo[] logos)
        {
            PlayerSettings.SplashScreen.logos = logos;
        }

        internal static void SetSplashScreen(Sprite background, Sprite backgroundPortrait = null)
        {
            PlayerSettings.SplashScreen.background = background;
            PlayerSettings.SplashScreen.backgroundPortrait = backgroundPortrait;
        }

        internal sealed partial class Android
        {
            internal static void ApplyExportSettings(bool exportAsGoogleAndroidProject = false, bool buildApkPerCpuArchitecture = false, bool buildAppBundle = false, bool useAPKExpansionFiles = false)
            {
                PlayerSettings.Android.useAPKExpansionFiles = useAPKExpansionFiles;//是否使用obb分离模式
                PlayerSettings.Android.buildApkPerCpuArchitecture = buildApkPerCpuArchitecture;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = exportAsGoogleAndroidProject;
#if UNITY_2018_3_OR_NEWER
                EditorUserBuildSettings.buildAppBundle = buildAppBundle;
#endif
            }

            internal static void ApplyKeystoreSettings(string keystoreFile, string keystorePassword, string keystoreAliasName, string keystoreAliasPassword)
            {
                PlayerSettings.Android.keystoreName = keystoreFile;
                PlayerSettings.Android.keystorePass = keystorePassword;
                PlayerSettings.Android.keyaliasName = keystoreAliasName;
                PlayerSettings.Android.keyaliasPass = keystoreAliasPassword;
            }

#if UNITY_2018_1_OR_NEWER

            internal static void ApplySettings(AndroidArchitecture targetArchitectures = AndroidArchitecture.All)
            {
                PlayerSettings.Android.targetArchitectures = targetArchitectures;
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
                PlayerSettings.Android.androidIsGame = androidIsGame;
                PlayerSettings.Android.minSdkVersion = minSdkVersion;
                PlayerSettings.Android.targetSdkVersion = targetSdkVersion;
                PlayerSettings.Android.preferredInstallLocation = preferredInstallLocation;
                PlayerSettings.Android.forceInternetPermission = forceInternetPermission;
                PlayerSettings.Android.forceSDCardPermission = forceSDCardPermission;
            }
        }

        internal sealed partial class iOS
        {
            internal static void ApplySigningSettings(bool automaticSigning, string appleDeveloperTeamID, string provisioningProfileID, ProvisioningProfileType provisioningProfileType)
            {
                PlayerSettings.iOS.appleEnableAutomaticSigning = automaticSigning;
                PlayerSettings.iOS.appleDeveloperTeamID = appleDeveloperTeamID;
                PlayerSettings.iOS.iOSManualProvisioningProfileID = provisioningProfileID;
                PlayerSettings.iOS.iOSManualProvisioningProfileType = provisioningProfileType;
            }

            internal static void ApplySettings(iOSSdkVersion targetSDK = iOSSdkVersion.DeviceSDK, iOSTargetDevice targetDevice = iOSTargetDevice.iPhoneAndiPad, string targetOSVersionString = "8.0")
            {
                PlayerSettings.iOS.sdkVersion = targetSDK;
                PlayerSettings.iOS.targetDevice = targetDevice;
                PlayerSettings.iOS.targetOSVersionString = targetOSVersionString;
            }

            internal static void SetiPhoneLaunchScreenType(iOSLaunchScreenType iOSLaunchScreenType = iOSLaunchScreenType.None)
            {
                PlayerSettings.iOS.SetiPhoneLaunchScreenType(iOSLaunchScreenType);
            }

        }

        internal sealed partial class EditorBuildSettings
        {
            internal static void SwitchActiveBuildTarget(BuildTargetGroup targetGroup, BuildTarget target, Action action)
            {
                if (EditorUserBuildSettings.activeBuildTarget == target)
                {
                    if (action != null) { action(); }
                }
                else
                {
                    if (EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target))
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
                        SetiOSPlayerSettings();
                        break;
#endif
                    case BuildTarget.Android:
                        SetAndroidPlayerSettings();
                        break;

                }
            }

            private static void SetiOSPlayerSettings()
            {
                if (!string.IsNullOrEmpty(PrettyVersion))
                {
                    PlayerSettings.bundleVersion = PrettyVersion;
                }
#if UNITY_5_3_OR_NEWER
                if (BuildNumber != null)
                {
                    PlayerSettings.iOS.buildNumber = BuildNumber.Value.ToString();
                }
#endif
            }

            private static void SetAndroidPlayerSettings()
            {
                if (!string.IsNullOrEmpty(PrettyVersion))
                {
                    PlayerSettings.bundleVersion = PrettyVersion;
                }
                if (BuildNumber != null)
                {
                    PlayerSettings.Android.bundleVersionCode = BuildNumber.Value;
                }
            }
        }
    }

}

#endif
