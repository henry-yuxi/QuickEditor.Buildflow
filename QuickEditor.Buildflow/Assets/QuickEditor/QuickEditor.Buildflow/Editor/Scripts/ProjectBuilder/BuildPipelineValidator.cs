#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using UnityEditor;
    using Debug = LoggerUtils;

    internal sealed class BuildPipelineValidator
    {
        public static bool CheckSupportedTarget(BuildTargetGroup targetGroup, BuildTarget target)
        {
            //SupportedTargets
            if (!BuildPipeline.IsBuildTargetSupported(targetGroup, target))
            {
                Debug.LogError(string.Format("BuildTarget -> {0} is not supported, Please install first. ", target.ToString()));
                return false;
            }
            return true;
        }

        public static bool IsValid(BuildTargetGroup targetGroup)
        {
            bool valid = true;
            if (targetGroup == BuildTargetGroup.Android)
            {
                if (!CheckAndroidEnvironmentVariables())
                {
                    valid = false;
                }
            }
            if (!CheckApplicationIdentifier(targetGroup))
            {
                valid = false;
            }

            return valid;
        }

        private const string DEFAULT_BUNDLE = "com.Company.ProductName";

        private static bool CheckApplicationIdentifier(BuildTargetGroup buildTarget)
        {
            bool valid = true;
            var identifier = PlayerSettings.GetApplicationIdentifier(buildTarget);
            if (string.IsNullOrEmpty(identifier) || identifier == DEFAULT_BUNDLE)
            {
                Debug.LogError("Bundle Identifier has not been set up correctly. Please set the Bundle Identifier in the Player Settings. The value must follow the convention 'com.YourCompanyName.YourProductName' and can contain alphanumeric characters and underscore.");
                valid = false;
            }
            return valid;
        }

        private static bool CheckAndroidEnvironmentVariables()
        {
            bool valid = true;
            string path = AndroidEnvironmentVariables.AndroidSdkRoot;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("EditorPrefs -> {AndroidSdkRoot} not fonud");
                valid = false;
            }
            path = AndroidEnvironmentVariables.JdkPath;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("EditorPrefs -> {JdkPath} not fonud");
                valid = false;
            }
            ScriptingImplementation scriptingImplementation = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            if (scriptingImplementation == ScriptingImplementation.IL2CPP)
            {
                path = AndroidEnvironmentVariables.AndroidNdkRoot;
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("EditorPrefs -> {AndroidNdkRoot} not fonud");
                    valid = false;
                }
            }

            if (string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) || string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
            {
                Debug.LogError("keystorepass is empty");
                valid = false;
            }
            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName) || string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
            {
                Debug.LogError("keyaliaspass is empty");
                //Debug.LogError("Unable to sign the application; please provide passwords!");
                valid = false;
            }

            return valid;
        }

        internal sealed class AndroidEnvironmentVariables
        {
            private const string AndroidSdkRootKey = "AndroidSdkRoot";
            private const string AndroidNdkRootKey = "AndroidNdkRoot";
            private const string JdkPathKey = "JdkPath";

            public static string AndroidSdkRoot
            {
                get { return EditorPrefs.GetString(AndroidSdkRootKey); }
                set { EditorPrefs.SetString(AndroidSdkRootKey, value); }
            }

            public static string AndroidNdkRoot
            {
                get { return EditorPrefs.GetString(AndroidNdkRootKey); }
                set { EditorPrefs.SetString(AndroidNdkRootKey, value); }
            }

            public static string JdkPath
            {
                get { return EditorPrefs.GetString(JdkPathKey); }
                set { EditorPrefs.SetString(JdkPathKey, value); }
            }
        }
    }
}

#endif
