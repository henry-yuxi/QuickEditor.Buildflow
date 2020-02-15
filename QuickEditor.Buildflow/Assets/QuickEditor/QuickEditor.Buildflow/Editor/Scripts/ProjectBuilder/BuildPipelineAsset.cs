#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal sealed partial class BuildPipelineAsset
    {
        /// <summary>
        /// 项目根目录 等同于 System.IO.Directory.GetCurrentDirectory() Environment.CurrentDirectory
        /// </summary>
        public static string ProjectPath = Path.GetDirectoryName(Application.dataPath).MakeUnixFormatPath();

        /// <summary>
        /// 打包管道项目外部根目录
        /// </summary>
        public static string ExternalBuildflowPath = Path.Combine(ProjectPath, "Buildflow").MakeUnixFormatPath();

        /// <summary>
        /// 打包管道存放平台安装包的根目录
        /// </summary>
        public static string ExternalBuildOutputPath = Path.Combine(ExternalBuildflowPath, "BuildOutput").MakeUnixFormatPath();

        /// <summary>
        /// 打包管道存放平台Bundles资源的根目录
        /// </summary>
        public static string ExternalBundlesPath = Path.Combine(ExternalBuildflowPath, "Bundles").MakeUnixFormatPath();

        /// <summary>
        /// 外部存放渠道或者插件的根目录
        /// </summary>
        public static string ExternalSDKCachePath = Path.Combine(ExternalBuildflowPath, "SDKCache").MakeUnixFormatPath();

        /// <summary>
        /// 外部存放Unity Package的根目录
        /// </summary>
        public static string ExternalPackagePath = Path.Combine(ExternalBuildflowPath, "Packages").MakeUnixFormatPath();

        /// <summary>
        /// Plugins目录
        /// </summary>
        public static string PluginsPath = Path.Combine(Application.dataPath, "Plugins").MakeUnixFormatPath();

        /// <summary>
        /// 对应平台的Plugins目录
        /// </summary>
        public static string PlatformPluginsPath = Path.Combine(PluginsPath, EditorUserBuildSettings.activeBuildTarget.ToString()).MakeUnixFormatPath();

        public static string InternalBuildflowPath = Path.Combine(PluginsPath, "Buildflow").MakeUnixFormatPath();
        public static string InternalBuildflowDeployPath = Path.Combine(InternalBuildflowPath, "Deploy").MakeUnixFormatPath();
        public static string InternalBuildflowDeployConfigurePath = Path.Combine(InternalBuildflowPath, "Deploy/Configure").MakeUnixFormatPath();
        public static string InternalBuildflowDeployKeystorePath = Path.Combine(InternalBuildflowPath, "Deploy/Keystore").MakeUnixFormatPath();

        /// <summary>
        /// 项目内部用于存放打包所需Icon 闪屏的目录(该目录资源不会被打进包内)  打包流程开始后自动从外部目录拷贝资源存入该目录 打包工具自动设置Icon 闪屏等
        /// </summary>
        public static string InternalPackagingCachePath = Path.Combine(InternalBuildflowPath, "PackagingCache").MakeUnixFormatPath();

        public static string InternalPackagingCachePluginsPath = Path.Combine(InternalPackagingCachePath, "Plugins").MakeUnixFormatPath();
        public static string InternalPackagingCacheResPath = Path.Combine(InternalPackagingCachePath, "Res").MakeUnixFormatPath();

        public static string[] NeedDeletePaths = new string[]
        {
            InternalPackagingCachePath,
        };

        /// <summary>
        /// 获取项目外部完整目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetExternalGlobalPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            return Path.Combine(ProjectPath, path).MakeUnixFormatPath();
        }

        public static string GetExternalPluginLibsSourcePath(string platform, string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return string.Empty;
            }
            string format = "{0}/PluginLibs/{1}/{2}/libs";
            return string.Format(format, ExternalSDKCachePath, platform, pluginName);
        }

        public static string GetExternalPluginResSourcePath(string platform, string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return string.Empty;
            }
            string format = "{0}/Backups/{1}/{2}";
            return string.Format(format, ExternalSDKCachePath, platform, pluginName);
        }

        public static string GetInternalPluginLibsCachePath(string platform, string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return string.Empty;
            }
            string format = "{0}/{1}/{2}";
            return string.Format(format, InternalPackagingCachePluginsPath, pluginName, platform);
        }

        public static string GetChannelFilterSearchPath(string filterSearchPath)
        {
            if (string.IsNullOrEmpty(filterSearchPath))
            {
                return string.Empty;
            }
            return GetRelativeAssetsPath(Path.Combine(BuildPipelineAsset.InternalPackagingCacheResPath, filterSearchPath));
        }

        public static string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }
    }
}

#endif
