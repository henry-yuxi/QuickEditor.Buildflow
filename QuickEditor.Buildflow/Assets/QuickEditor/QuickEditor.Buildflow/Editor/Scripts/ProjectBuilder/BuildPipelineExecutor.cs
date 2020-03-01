#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Monitor;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Debug = LoggerUtils;

    [InitializeOnLoad]
    internal sealed class BuildPipelineExecutor
    {
        static BuildPipelineExecutor()
        {
            PlayerSettingsSnapshot.ApplySnapshot();
            QuickUnityEditorEventsWatcher watcher = QuickUnityEditorEventsWatcher.Observe();
            watcher.BuildPipeline.OnProcessScene.AddListener(onProcessScene);
            watcher.BuildPipeline.OnPreprocessBuild.AddListener(onPreprocessBuild);
            watcher.BuildPipeline.OnPostprocessBuild.AddListener(onPostprocessBuild);
        }

        private static BuildEnvsOptions mBuildEnvsOptions;

        public static BuildEnvsOptions DefaultBuildEnvsOptions
        {
            get
            {
                if (mBuildEnvsOptions == null) { mBuildEnvsOptions = new BuildEnvsOptions(); }
                return mBuildEnvsOptions;
            }
        }

        public static void Build(ProjectBuildPresetSettings setting)
        {
            BuildPipelineExecutor.DefaultBuildEnvsOptions.SetBuildSettings(setting);
            UnityEditorResolver.EditorBuildSettings.SwitchActiveBuildTarget(BuildPipelineExecutor.DefaultBuildEnvsOptions.targetGroup, BuildPipelineExecutor.DefaultBuildEnvsOptions.target, () =>
            {
                PreBuild(BuildPipelineExecutor.DefaultBuildEnvsOptions);
            });
        }

        private static void PreBuild(BuildEnvsOptions buildOptions)
        {
            if (UnityEditor.BuildPipeline.isBuildingPlayer) { return; }
            PlayerSettingsSnapshot.ApplySnapshotIfFailed(() =>
            {
                PlayerSettingsSnapshot.TakeSnapshot();
                BuildPipelineExecutor.DefaultBuildEnvsOptions.ApplySettings();
                bool valid = BuildPipelineValidator.IsValid(buildOptions.targetGroup);
                if (!valid)
                {
                    //Debug.LogError("Build failure with errors");
                    throw new BuildFailedException("Build failure with errors");
                }
                int code = BuildPlayer(buildOptions, false);
                if (CommandLineArgs.InBatchMode())
                {
                    EditorApplication.Exit(code);
                }
            });
        }

        private static int BuildPlayer(BuildEnvsOptions buildOptions, bool revealInFinder = false)
        {
            BuildPipelineCommonTools.FileUtils.CheckDir(Path.GetDirectoryName(buildOptions.locationPathName));
#if UNITY_2018_1_OR_NEWER
            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(buildOptions.GetBuildPlayerOptions());
            UnityEditor.Build.Reporting.BuildSummary summary = report.summary;
            string errorMessage = string.Join("\n", report.steps.SelectMany(s => s.messages).Where(m => m.type == LogType.Error).Select(m => m.content).ToArray());
            bool buildSucceeded = (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded);
#else
            string errorMessage = BuildPipeline.BuildPlayer(buildOptions.scenes, buildOptions.locationPathName, buildOptions.target, buildOptions.options);
            bool buildSucceeded = !string.IsNullOrEmpty(errorMessage);
#endif

            UnityEngine.Debug.ClearDeveloperConsole();
            BuildPipelineExecutor.DefaultBuildEnvsOptions.DeletePackagingCacheFolders();
            if (buildSucceeded)
            {
                if (revealInFinder) { EditorUtility.RevealInFinder(buildOptions.locationPathName); }
#if UNITY_2018_1_OR_NEWER
                Debug.Log("Build completed successfully for {0}: {1} mb and took {2} seconds with {3} error(s). Location: {4}",
                    summary.platform.ToString(),
                    (summary.totalSize / 1024 / 1024).ToString("N2"),
                    summary.totalTime.Seconds,
                    summary.totalErrors,
                    summary.outputPath
                    );
#else
                    Debug.Log(string.Format("Build completed successfully for {0} to {1}", buildPlayerOptions.target, buildPlayerOptions.locationPathName));
#endif
                return 0;
            }
            else
            {
                Debug.LogError("Build failed with errors \n" + errorMessage);
                //throw new BuildFailedException("Build failed with errors \n" + errorMessage);
                return 1;
            }
        }

        private static void IncreaseVersion()
        {
        }

        private static void WriteVersion(string dstPath)
        {
        }

        #region Build Process

        private static void onProcessScene(Scene scene)
        {
        }

        private static void onPreprocessBuild(BuildTarget target, string pathToBuildProject)
        {
            Debug.Log("Starting to perform preprocess build tasks for {0} platform.", target);
#if UNITY_EDITOR_LINUX
            if (target == BuildTarget.Android)
                KillUnityShaderCompiler();
#endif

            BuildPipelineExecutor.DefaultBuildEnvsOptions.CopyPackagingResources();
            BuildPipelineExecutor.DefaultBuildEnvsOptions.ApplyPackagingResources();
        }

        private static void onPostprocessBuild(BuildTarget target, string pathToBuildProject)
        {
            Debug.Log("Starting to perform Postprocess build tasks for {0} platform.", target);
            PlayerSettingsSnapshot.ApplySnapshot();
        }

        private static void KillUnityShaderCompiler()
        {
            foreach (var proc in Process.GetProcessesByName("UnityShaderCompiler"))
            {
                proc.Kill();
            }
        }

        #endregion Build Process
    }
}

#endif
