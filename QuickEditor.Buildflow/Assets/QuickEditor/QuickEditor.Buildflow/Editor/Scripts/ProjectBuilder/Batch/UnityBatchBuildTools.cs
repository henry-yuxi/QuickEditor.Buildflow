#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using UnityEditor;
    using UnityEngine;

    public class UnityBatchBuildTools
    {
        //Unitys command line arguments
        protected const string BATCH_OPT_APPEND_SYMBOLS = "appendSymbols";

        protected const string BATCH_OPT_BUILD_TARGET = "buildTarget";
        protected const string BATCH_OPT_BUILD_NUMBER = "buildNumber";
        protected const string BATCH_OPT_BUNDLE_VERSION = "bundleVersion";
        protected const string BATCH_OPT_BUILD_PATH = "buildPath";
        protected const string BATCH_OPT_EXECUTABLE_NAME = "executableName";
        protected const string BATCH_OPT_BUILD_ACTION = "buildAction";
        protected const string BATCH_OPT_BUILD_TYPE = "buildType";
        protected const string BATCH_OPT_BUILD_CHANNEL = "buildChannel";

        protected static string mBatchAppendSymbols;
        protected static string mBatchBuildTargetString;
        protected static int mBatchBuildNumber;
        protected static string mBatchBundleVersion;
        protected static string mBatchBuildPath;
        protected static string mBatchExecutableName;
        protected static string mBatchBuildTypeString;
        protected static string mUnityChannelString;

        /// <summary>
        /// 命令行手动调用刷新  防止Unity有概率资源未刷新
        /// </summary>
        public static void Refresh()
        {
            AssetDatabase.Refresh();
        }

        public static void BuildPackage()
        {
            //1. 解析命令行参数
            ParseCommandLineArgs();

            //2. 修改打包参数
            bool cancel = false;
            BuildTargetGroup targetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(mBatchBuildTargetString);
            BuildTarget target = BuildPipelineCommonTools.BuildUtils.GetBuildTarget(targetGroup);
            if (!BuildPipelineValidator.CheckSupportedTarget(targetGroup, target))
            {
                return;
            }
            if (cancel)
            {
                Debug.LogWarning("Build canceled (not enough information)");
                return;
            }

            ProjectBuildPresetSettings buildPresetSettings = new ProjectBuildPresetSettings()
            {
                buildTarget = target,
                bundleVersionCode = mBatchBuildNumber,
                version = mBatchBundleVersion,
                overrideBuildPath = true,
                buildPath = mBatchBuildPath,
                overrideExecutableName = true,
                executableName = mBatchExecutableName,
            };

            //3. 执行打包逻辑
            BuildPipelineExecutor.Build(buildPresetSettings);
        }

        public static void BuildBundles()
        {
            //1. 解析命令行参数
            ParseCommandLineArgs();

            BuildTargetGroup targetGroup = BuildPipelineCommonTools.BuildUtils.GetBuildTargetGroup(mBatchBuildTargetString);
            BuildTarget target = BuildPipelineCommonTools.BuildUtils.GetBuildTarget(targetGroup);
            if (!BuildPipelineValidator.CheckSupportedTarget(targetGroup, target))
            {
                return;
            }

            //这里调用项目自己的ab打包方法
        }

        /// <summary>
        /// 解析命令行参数
        /// </summary>
        protected static void ParseCommandLineArgs()
        {
            CommandLineArgs.Reset();
            CommandLineArgs.GetValueAsString(BATCH_OPT_BUILD_TARGET, out mBatchBuildTargetString);
            CommandLineArgs.GetValueAsString(BATCH_OPT_BUILD_PATH, out mBatchBuildPath);
            CommandLineArgs.GetValueAsString(BATCH_OPT_EXECUTABLE_NAME, out mBatchExecutableName);
            CommandLineArgs.GetValueAsInt(BATCH_OPT_BUILD_NUMBER, out mBatchBuildNumber);
            CommandLineArgs.GetValueAsString(BATCH_OPT_BUNDLE_VERSION, out mBatchBundleVersion);
            CommandLineArgs.GetValueAsString(BATCH_OPT_APPEND_SYMBOLS, out mBatchAppendSymbols);
            CommandLineArgs.GetValueAsString(BATCH_OPT_BUILD_CHANNEL, out mUnityChannelString);
            CommandLineArgs.GetValueAsString(BATCH_OPT_BUILD_TYPE, out mBatchBuildTypeString);
        }
    }
}

#endif
