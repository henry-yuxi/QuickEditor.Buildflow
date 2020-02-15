#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Debug = LoggerUtils;

    internal sealed partial class BuildPipelineCommonTools
    {
        public sealed class BuildUtils
        {
            public static void SetEnablePluginImport(string[] plugins, bool enable = true)
            {
                foreach (var path in plugins)
                {
                    PluginImporter vrlib = AssetImporter.GetAtPath(path) as PluginImporter;
                    vrlib.SetCompatibleWithPlatform(BuildTarget.Android, enable);
                }
            }

            public static BuildPlayerOptions GetDefaultBuildPlayerOptions()
            {
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

                buildPlayerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
                buildPlayerOptions.options = BuildOptions.None;

                return buildPlayerOptions;
            }

            private static readonly string TIME_FORMAT = "yyyyMMdd.HHmm";

            public static string GetBuildPath(BuildTarget target)
            {
                BuildTargetGroup targetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(target);//buildTarget == BuildTarget.Android ? "Android" : "IOS";
                return Path.Combine(BuildPipelineAsset.ExternalBuildOutputPath, targetGroup.ToString());
            }

            public static string GetExecutableName(BuildTarget target, string buildType, bool needSuffix, string channelName = "default")
            {
                var fileExtension = BuildPipelineCommonTools.BuildUtils.GetSuffix(target);
                string buildTypeName = string.IsNullOrEmpty(buildType) ? "" : "." + buildType;
                string targetDisplayName = BuildPipelineCommonTools.BuildUtils.GetDisplayName(target);
                if (!string.IsNullOrEmpty(fileExtension) && needSuffix)
                {
                    return string.Format("{0}.{1}.{2}.{3}{4}{5}",
                        PlayerSettings.productName, targetDisplayName, channelName, DateTime.Now.ToString(TIME_FORMAT), buildTypeName, fileExtension);
                }
                else
                {
                    return string.Format("{0}.{1}.{2}.{3}{4}",
                        PlayerSettings.productName, targetDisplayName, channelName, DateTime.Now.ToString(TIME_FORMAT), buildTypeName);
                }
            }

            public static string GetLocationPathName(BuildTarget target, string fileName)
            {
                string pathBuild = GetBuildPath(target);
                return GetLocationPathName(target, pathBuild, fileName);
            }

            public static string GetLocationPathName(BuildTarget target, string pathBuild, ProjectBuildPresetSettings.BuildType buildType, bool needSuffix, string channelName = "default")
            {
                string executableName = GetExecutableName(target, buildType.ToString(), needSuffix, channelName);
                return GetLocationPathName(target, pathBuild, executableName);
            }

            public static string GetLocationPathName(BuildTarget target, string pathBuild, string fileName)
            {
                if (string.IsNullOrEmpty(pathBuild)) { pathBuild = GetBuildPath(target); }
                string fileSuffix = BuildPipelineCommonTools.BuildUtils.GetSuffix(target);
                bool withSuffix = Path.HasExtension(fileName);
                if (!withSuffix && !string.IsNullOrEmpty(fileSuffix))
                {
                    return string.Format("{0}/{1}{2}", pathBuild, fileName, fileSuffix);
                }
                else
                {
                    return string.Format("{0}/{1}", pathBuild, fileName);
                }
            }

            public static string[] GetEnabledScenes()
            {
                List<string> scenes = new List<string>();
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (!scene.enabled) continue;
                    scenes.Add(scene.path);
                }
                return scenes.ToArray();
            }

            public static Dictionary<BuildTarget, string> BuildTargetDisplayNames = new Dictionary<BuildTarget, string>()
            {
                { BuildTarget.Android, "Android" },
#if UNITY_3 || UNITY_4
                { BuildTarget.iPhone, "iOS" },
#else
                { BuildTarget.iOS, "iOS" },
#endif
                { BuildTarget.StandaloneWindows, "Windows/x86" },
                { BuildTarget.StandaloneWindows64, "Windows/x64" },
                { BuildTarget.StandaloneLinux, "Linux/x86" },
                { BuildTarget.StandaloneLinux64, "Linux/x64" },
                { BuildTarget.StandaloneLinuxUniversal, "Linux/Universal" },
                { BuildTarget.WebGL, "WebGL" }
            };

            public static string GetDisplayName(BuildTarget target)
            {
                string mDisplayNam;
                if (BuildTargetDisplayNames.TryGetValue(target, out mDisplayNam))
                {
                    return mDisplayNam;
                }
                return target.ToString();
            }

            public static string GetSuffix(BuildTarget target)
            {
                switch (target)
                {
                    case BuildTarget.StandaloneLinux: return ".x86";
                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneLinuxUniversal: return ".x86-64";
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64: return ".exe";
                    case BuildTarget.Android: return ".apk";
#if UNITY_3 || UNITY_4
                    case BuildTarget.iPhone: break;
#else
                    case BuildTarget.iOS: break;
#endif
                    case BuildTarget.WebGL: return ".html";
                    default: break;
                }
                return string.Empty;
                //return ".unknown";
            }

            public static BuildTarget GetBuildTarget(string buildTargetString, BuildTarget defaultBuildTarget = BuildTarget.NoTarget)
            {
                if (string.IsNullOrEmpty(buildTargetString))
                {
                    return defaultBuildTarget;
                }

                BuildTarget buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTargetString, true);
                if (Enum.IsDefined(typeof(BuildTarget), buildTarget))
                {
                    return buildTarget;
                }
                return defaultBuildTarget;
            }

            public static BuildTarget GetBuildTarget(BuildTargetGroup buildTargetGroup)
            {
                switch (buildTargetGroup)
                {
                    case BuildTargetGroup.Android: return BuildTarget.Android;
                    case BuildTargetGroup.iOS: return BuildTarget.iOS;
                    case BuildTargetGroup.Standalone: return BuildTarget.StandaloneWindows64;
                }
                return BuildTarget.NoTarget;
            }

            public static BuildTargetGroup GetBuildTargetGroup(string buildTargetString)
            {
                BuildTarget target = GetBuildTarget(buildTargetString);
                return GetBuildTargetGroup(target);
            }

            public static BuildTargetGroup GetBuildTargetGroup(BuildTarget buildTarget)
            {
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneOSX:
#if UNITY_3 || UNITY_4
                    case BuildTarget.iPhone: return BuildTargetGroup.iPhone;
#else
                    case BuildTarget.iOS: return BuildTargetGroup.iOS;
#endif
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneLinux:
                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneLinuxUniversal: return BuildTargetGroup.Standalone;
                    case BuildTarget.Android: return BuildTargetGroup.Android;
                    case BuildTarget.WebGL: return BuildTargetGroup.WebGL;
                    case BuildTarget.WSAPlayer: return BuildTargetGroup.WSA;
                    case BuildTarget.PSP2: return BuildTargetGroup.PSP2;
                    case BuildTarget.PS4: return BuildTargetGroup.PS4;
                    case BuildTarget.XboxOne: return BuildTargetGroup.XboxOne;
                    case BuildTarget.N3DS: return BuildTargetGroup.N3DS;
                    case BuildTarget.tvOS: return BuildTargetGroup.tvOS;
                    case BuildTarget.Switch: return BuildTargetGroup.Switch;
                        //default: return BuildTargetGroup.Standalone;
                }
                return BuildTargetGroup.Unknown;
            }

            public static List<BuildTarget> GetSupportedTargets()
            {
                var values = Enum.GetValues(typeof(BuildTarget)).Cast<BuildTarget>();
                List<BuildTarget> supported = new List<BuildTarget>();
                foreach (var v in values)
                {
                    if (IsSupported(v)) { supported.Add(v); }
                }
                return supported;
            }

            public static List<BuildTargetGroup> GetSupportedTargets(List<BuildTarget> targets)
            {
                List<BuildTargetGroup> supported = new List<BuildTargetGroup>();
                foreach (var t in targets)
                {
                    var g = GetBuildTargetGroup(t);
                    if (g != BuildTargetGroup.Unknown)
                    {
                        if (!supported.Contains(g)) { supported.Add(g); }
                    }
                }
                return supported;
            }

            private static Type _moduleManager = null;

            private static System.Reflection.MethodInfo _isPlatformSupportLoaded = null;

            private static System.Reflection.MethodInfo _getTargetString = null;

            private static bool IsSupported(BuildTarget target)
            {
                if (_isPlatformSupportLoaded == null)
                {
                    _moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
                    _isPlatformSupportLoaded = _moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    _getTargetString = _moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                }
                return (bool)_isPlatformSupportLoaded.Invoke(null, new object[] { (string)_getTargetString.Invoke(null, new object[] { target }) });
            }

            public static Texture2D FindIcon(BuildTarget target, bool small = false)
            {
                BuildTargetGroup targetGroup = GetBuildTargetGroup(target);
                return FindIcon(targetGroup, small);
            }

            public static Texture2D FindIcon(BuildTargetGroup target, bool small = false)
            {
                string name = string.Empty;
                switch (target)
                {
                    case BuildTargetGroup.iOS: name = "iPhone"; break;
                    default: name = target.ToString(); break;
                }
                if (string.IsNullOrEmpty(name)) { return null; }
                string path = string.Format("BuildSettings.{0}{1}", name, small ? ".small" : "");
                return EditorGUIUtility.FindTexture(path);
            }
        }

        public sealed class FileUtils
        {
            public static bool CheckDir(string url)
            {
                try
                {
                    if (!Directory.Exists(url))
                        Directory.CreateDirectory(url);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static void CopyFolder(string sourcePath, string destinationPath)
            {
                if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath)) { return; }
                DirectoryInfo source = new DirectoryInfo(sourcePath);
                DirectoryInfo target = new DirectoryInfo(destinationPath);
                if (!source.Exists)
                {
                    Debug.LogError("Directory not found : " + sourcePath);
                    return;
                }
                if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Debug.LogError(string.Format("父目录 {0} 不能拷贝到子目录 {1}", sourcePath, destinationPath));
                    return;
                }

                CheckDir(destinationPath);
                foreach (FileSystemInfo fsi in source.GetFileSystemInfos())
                {
                    string destName = Path.Combine(destinationPath, fsi.Name);
                    if (fsi is System.IO.FileInfo)
                    {
                        File.Copy(fsi.FullName, destName, true);
                    }
                    else
                    {
                        Directory.CreateDirectory(destName);
                        CopyFolder(fsi.FullName, destName);
                    }
                }
            }

            public static void DeleteFolder(string dir)
            {
                if (!Directory.Exists(dir))
                    return;
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);
                    }
                    else
                    {
                        DirectoryInfo d1 = new DirectoryInfo(d);
                        if (d1.GetFiles().Length != 0)
                        {
                            DeleteFolder(d1.FullName);////递归删除子文件夹
                        }
                        Directory.Delete(d, true);
                    }
                }
            }
        }

        internal sealed class ScriptingDefineSymbolsUtils
        {
            private static readonly char[] SPLIT_DIVIDER = new char[] { ';' };

            private const string JOIN_SEPARATOR = ";";

            public static void AddScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string addDefines)
            {
                if (string.IsNullOrEmpty(addDefines)) { return; }
                string[] addDefineArray = SplitScriptingDefineSymbols(addDefines);
                if (addDefineArray == null || addDefineArray.Count() == 0) { return; }
                bool changed = false;
                string mScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
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
                string mScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
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
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbos);
            }
        }

        public sealed class SDKUtils
        {
            public static void SetDefaultIcon()
            {
                if (GlobalToolConfigure.Current == null || GlobalToolConfigure.Current.SDK == null)
                {
                    Debug.LogError("GlobalToolConfigure not found, Cannot set default icon");
                    return;
                }
                if (string.IsNullOrEmpty(GlobalToolConfigure.Current.SDK.DefaultIconFilter) || string.IsNullOrEmpty(GlobalToolConfigure.Current.SDK.DefaultIconFilterSearchPath))
                {
                    Debug.LogError("GlobalToolConfigure -> DefaultIconFilter、DefaultIconFilterSearchPath is error, Cannot set default icon");
                    return;
                }

                Debug.Log("Starting set default icon");
                string filter = GlobalToolConfigure.Current.SDK.DefaultIconFilter;
                string filterSearchPath = BuildPipelineAsset.GetChannelFilterSearchPath(GlobalToolConfigure.Current.SDK.DefaultIconFilterSearchPath);
                var folders = new List<string>() { filterSearchPath };
                string[] searchInFolders = folders.ToArray();
                string[] assets = AssetDatabase.FindAssets(filter, searchInFolders);

                if (assets == null || assets.Length == 0)
                {
                    Debug.LogError(string.Format("Dir: {0} 下未找到匹配的资源, Icon设置失败", filterSearchPath));
                    return;
                }

                List<Texture2D> mTextures = new List<Texture2D>();
                mTextures.Clear();
                for (int i = 0; assets != null && i < assets.Length; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                    if (assetPath == null) { continue; }
                    Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (asset == null) { continue; }
                    mTextures.Add(asset);
                }

                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup buildTargetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(buildTarget);
                int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(buildTargetGroup);
                Texture2D[] textureArray = new Texture2D[iconSize.Length];
                for (int i = 0; i < textureArray.Length; i++)
                {
                    textureArray[i] = mTextures[0];
                }
                PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, textureArray);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Set default icon succeeded");
                //MethodInfo getIconFormPlatform = typeof(PlayerSettings).GetMethod("GetIconsForPlatform", BindingFlags.NonPublic | BindingFlags.Static);
                //MethodInfo getIconSizesForPlatform = typeof(PlayerSettings).GetMethod("GetIconSizesForPlatform", BindingFlags.NonPublic | BindingFlags.Static);
                //MethodInfo setIconsForPlatform = typeof(PlayerSettings).GetMethod("SetIconsForPlatform", BindingFlags.NonPublic | BindingFlags.Static);
                //Texture2D[] textureArray = (Texture2D[])getIconFormPlatform.Invoke(null, new object[] { string.Empty });
                //var iconSizesForPlatform = (int[])getIconSizesForPlatform.Invoke(null, new object[] { string.Empty });
                //if (textureArray.Length != iconSizesForPlatform.Length)
                //{
                //    textureArray = new Texture2D[iconSizesForPlatform.Length];
                //    setIconsForPlatform.Invoke(null, new object[] { string.Empty, textureArray });
                //}
                //textureArray[0] = mTextures[0];
                //setIconsForPlatform.Invoke(null, new object[] { string.Empty, textureArray });
                //AssetDatabase.SaveAssets();
            }

            public static void SetSplashLogos()
            {
                if (GlobalToolConfigure.Current == null || GlobalToolConfigure.Current.SDK == null)
                {
                    Debug.LogError("GlobalToolConfigure not found, Cannot set default icon");
                    return;
                }
                if (string.IsNullOrEmpty(GlobalToolConfigure.Current.SDK.DefaultIconFilter) || string.IsNullOrEmpty(GlobalToolConfigure.Current.SDK.DefaultIconFilterSearchPath))
                {
                    Debug.LogError("GlobalToolConfigure -> SplashLogosFilter、SplashLogosFilterSearchPath is error, Cannot set default icon");
                    return;
                }

                RestoreSplashData();
                Debug.Log("Starting set splash logos");
                string filter = GlobalToolConfigure.Current.SDK.SplashLogosFilter;
                string filterSearchPath = BuildPipelineAsset.GetChannelFilterSearchPath(GlobalToolConfigure.Current.SDK.SplashLogosFilterSearchPath);
                var folders = new List<string>() { filterSearchPath };
                string[] searchInFolders = folders.ToArray();
                string[] assets = AssetDatabase.FindAssets(filter, searchInFolders);

                if (assets == null || assets.Length == 0)
                {
                    Debug.LogError(string.Format("Dir: {0} 下未找到匹配的资源, SplashLogos设置失败", filterSearchPath));
                    return;
                }
                PlayerSettingsResolver.SetSplashScreen(backgroundColor: Color.white, show: true, showUnityLogo: false, drawMode: PlayerSettings.SplashScreen.DrawMode.AllSequential);
                List<PlayerSettings.SplashScreenLogo> screenLogos = new List<PlayerSettings.SplashScreenLogo>();
                screenLogos.Clear();
                for (int i = 0; assets != null && i < assets.Length; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                    if (assetPath == null) { continue; }
                    Sprite asset = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (asset == null) { continue; }

                    screenLogos.Add(PlayerSettings.SplashScreenLogo.Create(2f, asset));
                }
                PlayerSettingsResolver.SetSplashScreen(screenLogos.ToArray());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Set splash logos succeeded");
            }

            public static void SetSplashScreen()
            {
                if (GlobalToolConfigure.Current == null || GlobalToolConfigure.Current.SDK == null)
                {
                    Debug.LogError("GlobalToolConfigure not found, Cannot set default icon");
                    return;
                }
                if (string.IsNullOrEmpty(GlobalToolConfigure.Current.SDK.DefaultIconFilter) || string.IsNullOrEmpty(GlobalToolConfigure.Current.SDK.DefaultIconFilterSearchPath))
                {
                    Debug.LogError("GlobalToolConfigure -> SplashScreenFilter、SplashScreenFilterSearchPath is error, Cannot set default icon");
                    return;
                }

                RestoreSplashData();
                Debug.Log("Starting set splash screen");
                string filter = GlobalToolConfigure.Current.SDK.SplashScreenFilter;
                string filterSearchPath = BuildPipelineAsset.GetChannelFilterSearchPath(GlobalToolConfigure.Current.SDK.SplashScreenFilterSearchPath);
                var folders = new List<string>() { filterSearchPath };
                string[] searchInFolders = folders.ToArray();
                string[] assets = AssetDatabase.FindAssets(filter, searchInFolders);

                if (assets == null || assets.Length == 0)
                {
                    Debug.LogError(string.Format("Dir: {0} 下未找到匹配的资源, SplashScreen设置失败", filterSearchPath));
                    return;
                }
                PlayerSettingsResolver.SetSplashScreen(backgroundColor: Color.white, show: true, showUnityLogo: false, drawMode: PlayerSettings.SplashScreen.DrawMode.AllSequential);

                List<Texture2D> mSplashs = new List<Texture2D>();
                mSplashs.Clear();
                for (int i = 0; assets != null && i < assets.Length; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                    if (assetPath == null) { continue; }
                    Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (asset == null) { continue; }
                    mSplashs.Add(asset);
                }

                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                switch (buildTarget)
                {
                    case BuildTarget.iOS:
                        PlayerSettingsResolver.iOS.SetiPhoneLaunchScreenType(iOSLaunchScreenType.ImageAndBackgroundRelative);
                        QuickEditorUtils.SetSplashScreen("iOSLaunchScreenPortrait", mSplashs[0]);
                        QuickEditorUtils.SetSplashScreen("iOSLaunchScreenLandscape", mSplashs[0]);
                        break;

                    case BuildTarget.Android:
                        QuickEditorUtils.SetSplashScreen("androidSplashScreen", mSplashs[0]);
                        break;

                    default:
                        break;
                }
                //PlayerSettings.SplashScreen.background = mSplashs[0];
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Set splash screen succeeded");
            }

            private static void RestoreSplashData()
            {
                PlayerSettingsResolver.SetSplashScreen(logos: null);
                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                switch (buildTarget)
                {
                    case BuildTarget.iOS:
                        PlayerSettingsResolver.iOS.SetiPhoneLaunchScreenType(iOSLaunchScreenType.None);
                        QuickEditorUtils.SetSplashScreen("iOSLaunchScreenPortrait", null);
                        QuickEditorUtils.SetSplashScreen("iOSLaunchScreenLandscape", null);
                        break;

                    case BuildTarget.Android:
                        QuickEditorUtils.SetSplashScreen("androidSplashScreen", null);
                        break;

                    default:
                        break;
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}

#endif
