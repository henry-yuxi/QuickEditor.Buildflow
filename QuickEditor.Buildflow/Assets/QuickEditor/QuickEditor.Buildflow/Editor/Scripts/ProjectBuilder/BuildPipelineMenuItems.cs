#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using UnityEditor;
    using Debug = LoggerUtils;

    internal sealed partial class BuildPipelineMenuItems
    {
        internal const string BuildflowRootMenuEntry = "Tools/QuickEditor.Buildflow";

        internal const string GettingStartedMenuEntry = BuildflowRootMenuEntry + "/";

        internal const string HelpMenuEntry = BuildflowRootMenuEntry + "/Help";

        internal const string ReleaseNotesMenuEntry = BuildflowRootMenuEntry + "/Release Notes";

        internal const string AboutMenuEntry = BuildflowRootMenuEntry + "/About";

        internal const int GettingStartedMenuEntryPriority = 1000;

        internal const int ReleaseNotesMenuEntryPriority = 2000;

        internal const int HelpMenuEntryPriority = 9000;

        internal const int AboutMenuEntryPriority = 9001;

        #region Getting Started MenuItem Funcs

        [MenuItem(GettingStartedMenuEntry + "Getting Started", false, GettingStartedMenuEntryPriority)]
        public static void GettingStarted()
        {
            BuildPipelineCommonTools.FileUtils.CheckDir(BuildPipelineAsset.ExternalBuildflowPath);
            BuildPipelineCommonTools.FileUtils.CheckDir(BuildPipelineAsset.InternalBuildflowPath);
            BuildPipelineCommonTools.FileUtils.CheckDir(BuildPipelineAsset.InternalBuildflowDeployPath);
            BuildPipelineCommonTools.FileUtils.CheckDir(BuildPipelineAsset.InternalBuildflowDeployConfigurePath);
            BuildPipelineCommonTools.FileUtils.CheckDir(BuildPipelineAsset.InternalBuildflowDeployKeystorePath);
            Debug.Log("Buildflow Dirs initialize successful");
            if (!QuickAssetDatabase.ContainsAsset<GlobalToolConfigure>())
            {
                QuickAssetDatabase.CreateAsset<GlobalToolConfigure>(BuildPipelineAsset.GetRelativeAssetsPath(BuildPipelineAsset.InternalBuildflowDeployConfigurePath));
            }
            if (!QuickAssetDatabase.ContainsAsset<ProjectBuildConfigure>())
            {
                QuickAssetDatabase.CreateAsset<ProjectBuildConfigure>(BuildPipelineAsset.GetRelativeAssetsPath(BuildPipelineAsset.InternalBuildflowDeployConfigurePath));
            }
            if (!QuickAssetDatabase.ContainsAsset<ProjectCommonConfigure>())
            {
                QuickAssetDatabase.CreateAsset<ProjectCommonConfigure>(BuildPipelineAsset.GetRelativeAssetsPath(BuildPipelineAsset.InternalBuildflowDeployConfigurePath));
            }
            if (!QuickAssetDatabase.ContainsAsset<ProjectSDKConfigure>())
            {
                QuickAssetDatabase.CreateAsset<ProjectSDKConfigure>(BuildPipelineAsset.GetRelativeAssetsPath(BuildPipelineAsset.InternalBuildflowDeployConfigurePath));
            }
            if (!QuickAssetDatabase.ContainsAsset<ProjectXcodeConfigure>())
            {
                QuickAssetDatabase.CreateAsset<ProjectXcodeConfigure>(BuildPipelineAsset.GetRelativeAssetsPath(BuildPipelineAsset.InternalBuildflowDeployConfigurePath));
            }
            Debug.Log("Buildflow configuration files initialize successful");
            Debug.Log("Buildflow initialize successful");
            //#if ODIN_INSPECTOR
            //            GettingStartedWindow.ShowWindow();
            //#endif
        }

        #endregion Getting Started MenuItem Funcs

        #region Build MenuItem Funcs

        [MenuItem(GettingStartedMenuEntry + "Build Tools", false, GettingStartedMenuEntryPriority + 100)]
        public static void OpenProjectBuildWindow()
        {
#if ODIN_INSPECTOR
            ProjectBuildWindow.ShowWindow();
#endif
        }

        #endregion Build MenuItem Funcs

        #region ReleaseNotes MenuItem Funcs

        [MenuItem(ReleaseNotesMenuEntry, false, ReleaseNotesMenuEntryPriority)]
        public static void OpenReleaseNotesWindow()
        {
        }

        #endregion ReleaseNotes MenuItem Funcs

        #region Help MenuItem Funcs

        [MenuItem(HelpMenuEntry, false, HelpMenuEntryPriority)]
        public static void OpenHelpWindow()
        {
        }

        #endregion Help MenuItem Funcs

        #region About MenuItem Funcs

        [MenuItem(AboutMenuEntry, false, AboutMenuEntryPriority)]
        public static void OpenAboutWindow()
        {
            AboutWindow.ShowWindow();
        }

        #endregion About MenuItem Funcs
    }
}

#endif
