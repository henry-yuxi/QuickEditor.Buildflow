#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using Debug = LoggerUtils;

    internal sealed partial class AndroidPostProcess
    {
        public static void Process(string pathToBuildProject)
        {
            Debug.Log("AndroidPostProcess: Starting to perform post build tasks for Android platform.");
            if (pathToBuildProject == null) { return; }
        }
    }
}

#endif