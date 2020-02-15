#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;

    //#if ODIN_INSPECTOR
    //    [Sirenix.OdinInspector.TypeInfoBox("自动化打包工具全局配置")]
    //#endif
    [System.Serializable]
    internal class GlobalToolConfigure : QuickScriptableObject<GlobalToolConfigure>
    {
#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.TabGroup("SDK")]
        [Sirenix.OdinInspector.LabelText("SDK资源拷贝替换相关设置")]
#endif
        public ProjectSDKResSettings SDK = new ProjectSDKResSettings();

#if ODIN_INSPECTOR

        [Sirenix.OdinInspector.PropertySpace(10)]
        [Sirenix.OdinInspector.HorizontalGroup("Group", 0f, 0, 0)]
        [Sirenix.OdinInspector.Button("保存配置", Sirenix.OdinInspector.ButtonSizes.Large), Sirenix.OdinInspector.GUIColor(1f, 1f, 1f)]
#endif
        public void SaveConfigure()
        {
            Current.Save();
        }

        [System.Serializable]
        public class ProjectSDKResSettings
        {
#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Splash Settings")]
#endif
            public bool IsSplashLogos = true;

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Splash Settings")]
#endif
            public string SplashLogosFilter = "logo t:Sprite";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Splash Settings")]
#endif
            public string SplashLogosFilterSearchPath = "SplashLogos";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Splash Settings")]
#endif
            public string SplashScreenFilter = "splash t:Texture";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Splash Settings")]
#endif
            public string SplashScreenFilterSearchPath = "SplashScreen";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Icon Settings")]
#endif
            public string DefaultIconFilter = "icon t:Texture";

#if ODIN_INSPECTOR

            [Sirenix.OdinInspector.BoxGroup("Icon Settings")]
#endif
            public string DefaultIconFilterSearchPath = "Icon";
        }
    }
}

#endif
