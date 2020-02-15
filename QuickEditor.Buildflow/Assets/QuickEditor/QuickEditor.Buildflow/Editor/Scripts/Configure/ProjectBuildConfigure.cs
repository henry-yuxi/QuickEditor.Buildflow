#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    //#if ODIN_INSPECTOR
    //     [Sirenix.OdinInspector.TypeInfoBox("打包工具配置, 用于打包工具自动化操作")]
    //#endif
    [System.Serializable]
    public class ProjectBuildConfigure : QuickScriptableObject<ProjectBuildConfigure>
    {
        #region External Configuration

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.LabelText("打包配置")]
        [Sirenix.OdinInspector.ListDrawerSettings(NumberOfItemsPerPage = 1, ShowIndexLabels = true, ListElementLabelName = "presetName")]
#endif
        public List<ProjectBuildPresetSettings> Settings = new List<ProjectBuildPresetSettings>();

        #endregion External Configuration

        #region Internal Methods

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

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(5)]
        [Sirenix.OdinInspector.HorizontalGroup("Group2", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("@GetButtonName()", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
        public void BuildPackages()
        {
            Current.Save();
            if (!CheckBatchBuild())
            {
                Debug.LogError("当前无打包配置或者未选中需要打包的配置, 请检查后重新尝试");
                return;
            }
            foreach (var setting in Current.Settings)
            {
                if (setting.buildApplication)
                {
                    BuildPipelineExecutor.Build(setting);
                }
            }
        }

        protected bool CheckBatchBuild()
        {
            if (Current == null || Current.Settings == null) { return false; }
            return Current.Settings.Find(s => s.buildApplication) != null;
        }

        protected string GetButtonName()
        {
            return string.Format("批量打包(数量:{0})", GetActiveBuildApplicationCount);
        }

        protected int GetActiveBuildApplicationCount
        {
            get { return Current.Settings.FindAll(s => s.buildApplication).Count; }
        }

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(5)]
        [Sirenix.OdinInspector.HorizontalGroup("Group2", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("打包资源(未生效)", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
        public void BuildBundles()
        {
            Current.Save();
        }

        #endregion Internal Methods
    }

    [System.Serializable]
    public class ProjectBuildPresetSettings
    //: AbstractPresetSettings
    {
        [SerializeField]
        public string presetName = "New Preset";

        #region Application Build Settings

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Title("Application Build Settings")]
#endif
        [Tooltip("Buid Application.")]
        public bool buildApplication = true;

        public BuildTarget buildTarget = BuildTarget.Android;

        public BuildType buildType = BuildType.Release;

        //public BuildAction buildAction = BuildAction.BuildPackage;

        public BuildOptions buildOptions = BuildOptions.None;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.FoldoutGroup("Android Build Settings")]
        [Sirenix.OdinInspector.Indent(1)]
#endif
        public AndroidBuildSystem androidBuildSystem = AndroidBuildSystem.Gradle;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.FoldoutGroup("Android Build Settings")]
        [Sirenix.OdinInspector.Indent(1)]
#endif
        public AndroidExportSystem androidExportSystem = AndroidExportSystem.SingleAPK;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.FoldoutGroup("Android Build Settings")]
        [Sirenix.OdinInspector.Indent(1)]
#endif
        public bool useAPKExpansionFiles = false;//是否使用obb分离模式

        #endregion Application Build Settings

        #region Advanced Build Settings

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Title("Advanced Build Settings")]
        [Sirenix.OdinInspector.Indent(0)]
#endif

        public bool overrideBuildPath = false;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(1)]
        [Sirenix.OdinInspector.ShowIf("overrideBuildPath")]
#endif
        public string buildPath = string.Empty;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(0)]
#endif

        public bool overrideExecutableName = false;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(1)]
        [Sirenix.OdinInspector.ShowIf("overrideExecutableName")]
#endif
        public string executableName = string.Empty;

        #endregion Advanced Build Settings

        #region Version Settings

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Title("Version Settings")]
#endif
#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(0)]
        [Sirenix.OdinInspector.LabelText("Override Version (System Time)")]
#endif
        public bool useSystemTime = true;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(0)]
        [Sirenix.OdinInspector.HideIf("useSystemTime")]
#endif
        public string version = "1.0.0";

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(0)]
#endif
        public int buildNumber = 0;

        #endregion Version Settings

        #region Plugin Settings

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Title("Plugin Settings")]
        [Sirenix.OdinInspector.Indent(0)]
#endif
        public string channels = string.Empty;

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(0)]
        [Sirenix.OdinInspector.PropertySpace(0, 10)]
#endif
        public string plugins = string.Empty;

        #endregion Plugin Settings

        #region Build Settings Preview

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(1)]
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.FoldoutGroup("Build Settings Preview")]
#endif
        public BuildTarget activeBuildTarget { get { return buildApplication ? buildTarget : EditorUserBuildSettings.activeBuildTarget; } }

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(1)]
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.FoldoutGroup("Build Settings Preview")]
#endif
        public BuildTargetGroup activeTargetGroup { get { return BuildPipeline.GetBuildTargetGroup(activeBuildTarget); } }

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(1)]
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.FoldoutGroup("Build Settings Preview")]
#endif
        public string[] scenes { get { return EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes); } }

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.Indent(1)]
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.MultiLineProperty]
        [Sirenix.OdinInspector.FoldoutGroup("Build Settings Preview")]
#endif
        public string locationPathName
        {
            get
            {
                if (overrideExecutableName && !string.IsNullOrEmpty(executableName))
                {
                    return BuildPipelineCommonTools.BuildUtils.GetLocationPathName(activeBuildTarget, overrideBuildPath ? buildPath : null, executableName).MakeUnixFormatPath();
                }
                else
                {
                    return BuildPipelineCommonTools.BuildUtils.GetLocationPathName(activeBuildTarget, overrideBuildPath ? buildPath : null, buildType, androidExportSystem == AndroidExportSystem.SingleAPK || androidExportSystem == AndroidExportSystem.AppBundle).MakeUnixFormatPath();
                }
            }
        }

        #endregion Build Settings Preview

        #region Internal Methods

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(5)]
        [Sirenix.OdinInspector.HorizontalGroup("Group", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("打包应用", Sirenix.OdinInspector.ButtonSizes.Medium), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
        public void BuildPackage()
        {
            ProjectBuildConfigure.Current.Save();
            BuildPipelineExecutor.Build(this);
        }

        #endregion Internal Methods

        public enum BuildAction
        {
            None = -1,
            Refresh = 0,
            SetScriptingDefineSymbols = 1,
            BuildBuildle = 2,
            BuildPackage = 6,
            BuildWholePackage = 7
        }

        public enum BuildType
        {
            Beta = 0,
            Develop = 1,
            Release = 2
        }

        public enum AndroidExportSystem
        {
            AndroidProject = 0,
            AppBundle = 1,
            SingleAPK = 2,
            SplitArchitectureAPK = 3,
        }

        public enum ePlistMethod
        {
            AppStore,
            Enterprise,
            AdHoc,
            Development
        }
    }
}

#endif
