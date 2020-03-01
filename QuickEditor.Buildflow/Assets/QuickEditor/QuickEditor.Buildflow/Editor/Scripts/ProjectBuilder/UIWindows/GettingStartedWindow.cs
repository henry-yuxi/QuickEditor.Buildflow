#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using UnityEngine;
    using UnityEditor;

#if ODIN_INSPECTOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;

#endif
#if ODIN_INSPECTOR

    internal sealed class GettingStartedWindow : OdinEditorWindow
    {
        private static GettingStartedWindow mWindow;

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
            mWindow = GetWindow<GettingStartedWindow>();
            mWindow.titleContent = new GUIContent("Getting Started");
            mWindow.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 700);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

        }
    }

#endif
}

#endif
