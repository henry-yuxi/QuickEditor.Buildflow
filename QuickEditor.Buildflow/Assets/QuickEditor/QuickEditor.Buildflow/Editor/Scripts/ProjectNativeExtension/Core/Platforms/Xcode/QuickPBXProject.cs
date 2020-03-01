#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
#if UNITY_IOS

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using Debug = LoggerUtils;

#if UNITY_2018_OR_NEWER

    using UnityEditor.iOS.Xcode;

#else

    using UnityEditor.iOS.Xcode.Custom;

#endif

    public partial class QuickPBXProject : System.IDisposable
    {
        protected const string PROJECT_ROOT = "$(PROJECT_DIR)/"; //工程根目录
        protected const string IMAGE_XCASSETS_DIRECTORY_NAME = "Unity-iPhone"; // Images.xcassets 默认的target

        protected const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";
        protected const string DEVELOPMENT_TEAM = "DEVELOPMENT_TEAM";

        protected const string OTHER_CFLAGS_KEY = "OTHER_CFLAGS";
        protected const string OTHER_LDFLAGS_KEY = "OTHER_LDFLAGS"; //LinkerFlags 所有链接标签

        protected const string FRAMEWORK_SEARCH_PATHS_KEY = "FRAMEWORK_SEARCH_PATHS";
        protected const string HEADER_SEARCH_PATHS_KEY = "HEADER_SEARCH_PATHS";
        protected const string LIBRARY_SEARCH_PATHS_KEY = "LIBRARY_SEARCH_PATHS";

        protected const string GCC_ENABLE_CPP_EXCEPTIONS_KEY = "GCC_ENABLE_CPP_EXCEPTIONS";
        protected const string GCC_ENABLE_CPP_RTTI_KEY = "GCC_ENABLE_CPP_RTTI";
        protected const string GCC_ENABLE_OBJC_EXCEPTIONS_KEY = "GCC_ENABLE_OBJC_EXCEPTIONS";

        protected readonly string mBuildProjectPath;
        protected readonly string mProjPath;
        protected string mTargetGuid;
        protected string mTargetName;
        protected PBXProject mProj;
        protected bool mCanLoad = true;
        protected string TAG = "QPBXProject";

        public QuickPBXProject(string pathToBuildProject)
        {
            mBuildProjectPath = pathToBuildProject;
            mProjPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
        }

        public PBXProject Proj { get { return mProj; } }
        public string TargetName { get { return mTargetName; } }
        public string TargetGuid { get { return mTargetGuid; } }

        #region 文件读写相关方法

        public bool CanLoad
        {
            get
            {
                mCanLoad = true;
                if (!System.IO.File.Exists(mProjPath))
                {
                    Debug.LogError(string.Format("PBXProjectPath : {0} not found", mProjPath));
                    mCanLoad = false;
                }
                return mCanLoad;
            }
        }

        public void Load()
        {
            mProj = new PBXProject();
            mProj.ReadFromFile(mProjPath);
            mTargetName = PBXProject.GetUnityTargetName();
            mTargetGuid = mProj.TargetGuidByName(mTargetName);

            //针对不同版本设置参数的方式
            //string configGuid = proj.BuildConfigByName("targetGuid", "Debug");
            //proj.SetBuildPropertyForConfig(configGuid, "ENABLE_BITCODE", "NO");
        }

        public void Write()
        {
            mProj.WriteToFile(mProjPath);
            //File.WriteAllText(mProjPath, mProj.WriteToString());
        }

        public void Dispose()
        {
        }

        #endregion 文件读写相关方法

        public void AddCapability(PBXCapabilityType capability, string entitlementsFilePath = null, bool addOptionalFramework = false)
        {
            Debug.Log(string.Format("{0}: Begin Add Capability : {1}, EntitlementsFilePath : {2}, AddOptionalFramework : {3}", TAG, capability.id, entitlementsFilePath, addOptionalFramework));
            if (string.IsNullOrEmpty(entitlementsFilePath))
            {
                mProj.AddCapability(mTargetGuid, capability);
            }
            else
            {
                string mTargetEntitlementPath = entitlementsFilePath;
                string xcodeEntitlementPath = entitlementsFilePath;
                string packToolsPath = Application.dataPath.Replace("/YL/Assets", "/ProjectBuilder/PackTools/ExportOptions");
                string entitlement = Path.Combine(packToolsPath, entitlementsFilePath);
                if (File.Exists(entitlement))
                {
                    mTargetEntitlementPath = mTargetName + "/" + entitlementsFilePath;
                    xcodeEntitlementPath = Path.Combine(mBuildProjectPath, mTargetName + "/" + entitlementsFilePath);
                    File.Copy(entitlement, xcodeEntitlementPath, true);
                }
                //mProj.AddFile(mTargetEntitlementPath, Path.GetFileName(entitlementsFilePath));
                //mProj.AddBuildProperty(mTargetGuid, "CODE_SIGN_ENTITLEMENTS", mTargetEntitlementPath);
                mProj.AddCapability(mTargetGuid, capability, mTargetEntitlementPath, addOptionalFramework);
            }
            Debug.Log(string.Format("{0}: Adding Capability : {1}, EntitlementsFilePath : {2}, AddOptionalFramework : {3}", TAG, capability.id, entitlementsFilePath, addOptionalFramework));
        }

        /// <summary>
        /// 添加内部框架 Framework
        /// </summary>
        /// <param name="framework"></param>
        /// <param name="weak"></param>
        public void AddFramework(string framework, bool weak)
        {
            if (mProj.ContainsFramework(mTargetGuid, framework))
            {
                mProj.RemoveFrameworkFromProject(mTargetGuid, framework);
            }
            mProj.AddFrameworkToProject(mTargetGuid, framework, weak);
            Debug.Log(string.Format("Adding framework : {0}, IsWeek : {1}", framework, weak));
        }

        /// <summary>
        /// 添加tbd
        /// </summary>
        /// <param name="lib"></param>
        public void AddLibrary(string lib)
        {
            string fileGuid = mProj.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
            mProj.AddFileToBuild(mTargetGuid, fileGuid);
            Debug.Log(string.Format("Adding library : {0}", lib));
        }

        public void SetCompilerFlags(string flags, List<string> targetPaths)
        {
            foreach (string targetPath in targetPaths)
            {
                if (!mProj.ContainsFileByProjectPath(targetPath))
                {
                    Debug.LogWarning("Set Compiler Flag Error, {0} not found", targetPath);
                    continue;
                }
                string fileGuid = mProj.FindFileGuidByProjectPath(targetPath);
                List<string> flagsList = mProj.GetCompileFlagsForFile(mTargetGuid, fileGuid);
                flagsList.Add(flags);
                mProj.SetCompileFlagsForFile(mTargetGuid, fileGuid, flagsList);
            }
        }

        public void SetBuildProperty(string name, string value)
        {
            mProj.SetBuildProperty(mTargetGuid, name, value);
        }

        public void SetTeamID(string teamId)
        {
            mProj.SetTeamId(mTargetGuid, teamId);
        }

        public void SetBitCode(bool enable)
        {
            mProj.SetBuildProperty(mTargetGuid, ENABLE_BITCODE_KEY, enable ? "YES" : "NO");
        }

        public void SetCppExceptions(bool enable)
        {
            mProj.SetBuildProperty(mTargetGuid, GCC_ENABLE_CPP_EXCEPTIONS_KEY, enable ? "YES" : "NO");
        }

        public void SetCppRtti(bool enable)
        {
            mProj.SetBuildProperty(mTargetGuid, GCC_ENABLE_CPP_RTTI_KEY, enable ? "YES" : "NO");
        }

        public void SetObjcExceptions(bool enable)
        {
            mProj.SetBuildProperty(mTargetGuid, GCC_ENABLE_OBJC_EXCEPTIONS_KEY, enable ? "YES" : "NO");
        }

        public void SetCompileFlagsForFile(string file, string headersPath, string compileFlags)
        {
            if (string.IsNullOrEmpty(file) || string.IsNullOrEmpty(compileFlags)) { return; }
            string fileGuid = mProj.FindFileGuidByProjectPath(file);
            if (fileGuid == null)
            {
                Debug.LogError(string.Format("{0}: Cannot find {1}", TAG, file));
                return;
            }
            List<string> flags = compileFlags.Split(',')
                    .Select(x =>
                    {
                        string returnString = x;
                        while (true)
                        {
                            if (returnString.IndexOf(" ") != 0)
                                break;
                            returnString = returnString.Substring(1);
                            if (string.IsNullOrEmpty(returnString))
                                return x;
                        }
                        while (true)
                        {
                            int spaceIndex = returnString.LastIndexOf(" ");
                            if ((returnString.Length - 1) != spaceIndex)
                                break;
                            returnString = returnString.Substring(0, (returnString.Length - 1));
                            if (string.IsNullOrEmpty(returnString))
                                return x;
                        }
                        return returnString;
                    }).ToList();
            mProj.SetCompileFlagsForFile(mTargetGuid, fileGuid, flags);
            if (string.IsNullOrEmpty(headersPath)) { return; }
            SetHeaderSearchPaths(new List<string>() { headersPath });
        }

        public void SetLinkerFlag(List<string> linkerFlagArray)
        {
            if (linkerFlagArray != null && linkerFlagArray.Count > 0)
            {
                foreach (var flag in linkerFlagArray)
                {
                    mProj.AddBuildProperty(mTargetGuid, OTHER_LDFLAGS_KEY, flag);
                }
            }
        }

        public void SetFrameworkSearchPaths(List<string> paths)
        {
            foreach (var path in paths)
            {
                mProj.AddBuildProperty(mTargetGuid, FRAMEWORK_SEARCH_PATHS_KEY, path);
            }
        }

        public void SetLibrarySearchPaths(List<string> paths)
        {
            foreach (var path in paths)
            {
                mProj.AddBuildProperty(mTargetGuid, LIBRARY_SEARCH_PATHS_KEY, path);
            }
        }

        public void SetHeaderSearchPaths(List<string> paths)
        {
            foreach (var path in paths)
            {
                mProj.AddBuildProperty(mTargetGuid, HEADER_SEARCH_PATHS_KEY, path);
            }
        }
    }

#endif
}

#endif
