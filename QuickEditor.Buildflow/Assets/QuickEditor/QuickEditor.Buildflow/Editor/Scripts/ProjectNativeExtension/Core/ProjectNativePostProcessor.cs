#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Monitor;
    using UnityEditor;

    [InitializeOnLoad]
    internal sealed partial class ProjectNativePostProcessor
    {
        static ProjectNativePostProcessor()
        {
            QuickUnityEditorEventsWatcher watcher = QuickUnityEditorEventsWatcher.Observe();
            watcher.BuildPipeline.OnPostprocessBuild.AddListener(onPostprocessBuild);
        }

        private static void onPostprocessBuild(BuildTarget target, string pathToBuildProject)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSX:
                    break;

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    StandalonePostProcess.Process(pathToBuildProject);
                    break;

                case BuildTarget.iOS:
                    iOSPostProcess.Process(pathToBuildProject);
                    break;

                case BuildTarget.Android:
                    AndroidPostProcess.Process(pathToBuildProject);
                    break;

                default:
                    break;
            }
        }
    }
}

#endif
