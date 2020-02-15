#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public sealed class CommandMenu
    {
        internal const string BuildPipelineAssetMenuEntry = "QuickEditor.Buildflow/";
        internal const int BuildPipelineAssetMenuEntryPriority = 0;
    }

    [CreateAssetMenu(menuName = CommandMenu.BuildPipelineAssetMenuEntry + "Create BuildflowPackageConfigure", order = CommandMenu.BuildPipelineAssetMenuEntryPriority)]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.TypeInfoBox("自动化打包工具包配置")]
#endif
    [System.Serializable]
    public class PackageConfigure : QuickScriptableObject<PackageConfigure>
    {
#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("Package")]
        [Sirenix.OdinInspector.LabelText("工具包配置")]
#endif
        public PackageInfo Package = new PackageInfo();

        [System.Serializable]
        public class PackageInfo
        {
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.Title("Package Settings")]
#endif
            public string Name = "Quick Buildflow";

            public string PackageName = "QuickEditor.Buildflow";
            public string Version = "0.0.1";
            public string Author = "Henry";
            public string Copyright = "Copyright © 2019 Henry";

            public string[] kAssetPaths = new string[] {
                "Assets/QuickEditor",
            };

            public bool RevealInFinder = true;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.PropertySpace(10)]
            [Sirenix.OdinInspector.HorizontalGroup("Group", 0f, 0, 0)]
            [Sirenix.OdinInspector.Button("Export Package", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
            public void Export()
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
                if (kAssetPaths == null || kAssetPaths.Length == 0)
                {
                    EditorUtility.DisplayDialog("错误", "未选择任何需要导出的资源, 请检查", "确定");
                    return;
                }
                string kPackageName = string.Format("{0}/{1}_Version.{2}.unitypackage", BuildPipelineAsset.ExternalPackagePath, PackageName, Version);
                if (string.IsNullOrEmpty(kPackageName))
                {
                    return;
                }
                AssetDatabase.ExportPackage(kAssetPaths, kPackageName, ExportPackageOptions.Recurse | ExportPackageOptions.Default);
                Debug.Log("Export Package Successfully : " + kPackageName);
                if (RevealInFinder)
                {
                    EditorUtility.RevealInFinder(kPackageName);
                }
            }
        }
    }
}

#endif
