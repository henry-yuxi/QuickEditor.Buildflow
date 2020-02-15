#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
#if UNITY_IOS

    using System.IO;

#if UNITY_2018_OR_NEWER

    using UnityEditor.iOS.Xcode;

#else

    using UnityEditor.iOS.Xcode.Custom;

#endif

    public class QuickXcodeDirectory
    {
        protected const string META = ".meta";
        protected const string ARCHIVE = ".a";
        protected const string FRAMEWORK = ".framework";
        protected const string BUNDLE = ".bundle";

        protected const string IMAGE_XCASSETS_DIRECTORY_NAME = "Unity-iPhone";

        protected const string PRODUCT_NAME = "$(PRODUCT_NAME)";
        protected const string PROJECT_ROOT = "$(PROJECT_DIR)/";

        protected const string HEADER_SEARCH_PATHS_KEY = "HEADER_SEARCH_PATHS";
        protected const string LIBRARY_SEARCH_PATHS_KEY = "LIBRARY_SEARCH_PATHS";
        protected const string FRAMEWORK_SEARCH_PATHS_KEY = "FRAMEWORK_SEARCH_PATHS";

        public static void CopyAndAddBuildToXcode(PBXProject pbxProject, string targetGuid, string copyDirPath, string pathToBuildProject, string currentDirectoryPath, bool needToAddBuild = true)
        {
            string unityDirPath = copyDirPath;
            string mBuildProjectPath = pathToBuildProject;

            if (!string.IsNullOrEmpty(currentDirectoryPath))
            {
                unityDirPath = Path.Combine(unityDirPath, currentDirectoryPath);
                mBuildProjectPath = Path.Combine(mBuildProjectPath, currentDirectoryPath);
                Delete(mBuildProjectPath);
                Directory.CreateDirectory(mBuildProjectPath);
            }

            foreach (string filePath in Directory.GetFiles(unityDirPath))
            {
                string extension = Path.GetExtension(filePath);
                if (extension == META)
                {
                    continue;
                }
                else if (extension == ARCHIVE)
                {
                    pbxProject.AddBuildProperty(targetGuid, LIBRARY_SEARCH_PATHS_KEY, PROJECT_ROOT + currentDirectoryPath);
                }
                string fileName = Path.GetFileName(filePath);
                string copyPath = Path.Combine(mBuildProjectPath, fileName);
                if (fileName[0] == '.')
                {
                    continue;
                }

                File.Delete(copyPath);
                File.Copy(filePath, copyPath);
                if (needToAddBuild)
                {
                    string relativePath = Path.Combine(currentDirectoryPath, fileName);
                    pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile(relativePath, relativePath, PBXSourceTree.Source));
                }
            }

            foreach (string directoryPath in Directory.GetDirectories(unityDirPath))
            {
                string directoryName = Path.GetFileName(directoryPath);
                bool nextNeedToAddBuild = needToAddBuild;

                if (directoryName.Contains(FRAMEWORK) || directoryName.Contains(BUNDLE) || directoryName == IMAGE_XCASSETS_DIRECTORY_NAME)
                {
                    nextNeedToAddBuild = false;
                }

                CopyAndAddBuildToXcode(pbxProject, targetGuid, copyDirPath, pathToBuildProject, Path.Combine(currentDirectoryPath, directoryName), nextNeedToAddBuild);

                if (directoryName.Contains(FRAMEWORK) || directoryName.Contains(BUNDLE))
                {
                    string relativePath = Path.Combine(currentDirectoryPath, directoryName);

                    pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile(relativePath, relativePath, PBXSourceTree.Source));

                    pbxProject.AddBuildProperty(targetGuid, FRAMEWORK_SEARCH_PATHS_KEY, PROJECT_ROOT + currentDirectoryPath);
                }
            }
        }

        public static void CopyAndReplace(string sourcePath, string copyPath)
        {
            Delete(copyPath);
            Directory.CreateDirectory(copyPath);

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                File.Copy(file, Path.Combine(copyPath, Path.GetFileName(file)));
            }

            foreach (var dir in Directory.GetDirectories(sourcePath))
            {
                CopyAndReplace(dir, Path.Combine(copyPath, Path.GetFileName(dir)));
            }
        }

        public static void Delete(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            string[] filePaths = Directory.GetFiles(targetDirectoryPath);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);

            foreach (string directoryPath in directoryPaths)
            {
                Delete(directoryPath);
            }

            Directory.Delete(targetDirectoryPath, false);
        }
    }

#endif
}

#endif