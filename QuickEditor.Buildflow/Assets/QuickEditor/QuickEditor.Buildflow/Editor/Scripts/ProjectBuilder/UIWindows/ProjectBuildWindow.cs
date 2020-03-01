#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

#if ODIN_INSPECTOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;

#endif
#if ODIN_INSPECTOR

    internal sealed class ProjectBuildWindow : OdinMenuEditorWindow
    {
        private static ProjectBuildWindow mWindow;

        public static void ShowWindow()
        {
            if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂停中, 请不要操作!", "确定");
                return;
            }

            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("错误", "游戏脚本正在编译, 请不要操作!", "确定");
                return;
            }
            mWindow = GetWindow<ProjectBuildWindow>();
            mWindow.titleContent = new GUIContent("Quick Buildflow");
            mWindow.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 700);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "首页", null, EditorIcons.House },
                { "打包", null, EditorIcons.Tag },
                { "打包/打包工具", ProjectBuildConfigure.Current, EditorIcons.SingleUser  },
                { "管理", null, EditorIcons.Tag },
                { "管理/游戏管理", ProjectCommonConfigure.Current, EditorIcons.SettingsCog },
                { "管理/SDK管理", ProjectSDKConfigure.Current, EditorIcons.SettingsCog },
                { "管理/Xcode管理", ProjectXcodeConfigure.Current, EditorIcons.SettingsCog },

                { "其他", null, EditorIcons.Tag },
                { "其他/全局工具配置", GlobalToolConfigure.Current },
                { "其他/Player Settings", Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault() },
            };
            return tree;
        }
    }

#endif
}

#endif
