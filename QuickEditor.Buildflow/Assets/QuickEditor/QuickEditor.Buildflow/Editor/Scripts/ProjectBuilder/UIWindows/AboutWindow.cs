#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    internal sealed class AboutWindow : QuickEditorWindow
    {
        private static AboutWindow mWindow;
        private static new Vector2 WindowRect = new Vector2(250, 250);

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
            WindowTitle = "Quick Buildflow";
            WindowRect = new Vector2(250, 330);
            mWindow = GetEditorWindow<AboutWindow>(true);
            mWindow.minSize = WindowRect;
            mWindow.maxSize = WindowRect;
            mWindow.maximized = true;
            Undo.undoRedoPerformed += () =>
            {
                mWindow.Repaint();
            };
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            GUILayout.Space(30);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(PackageConfigure.Current.Package.Name);
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Version " + PackageConfigure.Current.Package.Version);
            GUILayout.Space(20);
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Designed and implemented by");
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(PackageConfigure.Current.Package.Author);
            GUILayout.Space(20);
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(PackageConfigure.Current.Package.Copyright);
        }
    }
}

#endif
