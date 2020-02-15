#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using Debug = LoggerUtils;

    //public static readonly string[] kMethods = { "app-store", "enterprise", "ad-hoc", "development", };

    internal sealed partial class iOSPostProcess
    {
        public static void Process(string pathToBuildProject)
        {
            Debug.Log("iOSPostProcess: Starting to perform post build tasks for iOS platform.");
#if UNITY_IOS
            if (pathToBuildProject == null) { return; }
            var settings = ProjectXcodeConfigure.Current.Settings;
            if (settings == null || settings.Count == 0) { return; }
            var setting = settings.Find(s => s.group == "default");
            if (setting == null) { return; }
            SetInfoPlist(pathToBuildProject, setting);
            ModifyPBXProject(pathToBuildProject, setting, "");
            ModifyXcodeCodes(pathToBuildProject, setting);
#endif
        }

#if UNITY_IOS

        private static void SetGeneralSetting(string pathToBuildProject, ProjectXcodeConfigure configure)
        {
            QuickInfoPlist plist = new QuickInfoPlist(pathToBuildProject);
            if (plist.CanLoad)
            {
                plist.Load();
                plist.SetATS(configure.Global.EnableATS);
                plist.SetStatusBar(configure.Global.EnableStatusBar);
                plist.SetLaunchImages(configure.Global.NeedToDeleteLaunchiImages);
                plist.Write();
            }
            QuickPBXProject project = new QuickPBXProject(pathToBuildProject);
            if (project.CanLoad)
            {
                project.Load();
                project.SetBitCode(configure.Global.EnableBitCode);
                project.SetCppExceptions(configure.Global.EnableCppExceptions);
                project.SetCppRtti(configure.Global.EnableCppRtti);
                project.SetObjcExceptions(configure.Global.EnableObjcExceptions);
                project.Write();
            }
        }

        private static void SetInfoPlist(string pathToBuildProject, XcodeProjectPresetSettings setting)
        {
            if (pathToBuildProject == null || setting == null || setting.InfoPlist == null) { return; }
            XcodeProjectPresetSettings.XcodeInfoPlistSettings infoPlistSetting = setting.InfoPlist;
            QuickInfoPlist plist = new QuickInfoPlist(pathToBuildProject);
            if (plist.CanLoad)
            {
                plist.Load();
                plist.SetPrivacySensiticeData(infoPlistSetting.PrivacySensiticeData, "privacySensiticeData");
                plist.SetApplicationQueriesSchemes(infoPlistSetting.ApplicationQueriesSchemes);
                plist.SetBackgroundModes(infoPlistSetting.BackgroundModes);
                plist.SetDomains(infoPlistSetting.Domains);
                foreach (var set in infoPlistSetting.Permissions)
                {
                    if (set == null) { continue; }
                    plist.SetPermission(set.permissionName, set.describe);
                }
                foreach (var urlData in infoPlistSetting.BundleURLTypes)
                {
                    if (urlData == null) { continue; }
                    plist.SetURLSchemes(urlData.BundleTypeRole, urlData.BundleURLName, urlData.BundleURLSchmes);
                }

                plist.Write();
            }
        }

        private static void ModifyPBXProject(string pathToBuildProject, XcodeProjectPresetSettings setting, string exportMethod = null)
        {
            if (pathToBuildProject == null || setting == null || setting.PBXProject == null) { return; }
            XcodeProjectPresetSettings.XcodePBXProjectSettings pBXProjectSetting = setting.PBXProject;
            if (pBXProjectSetting == null) { return; }
            QuickPBXProject project = new QuickPBXProject(pathToBuildProject);
            if (project.CanLoad)
            {
                project.Load();
                project.SetLinkerFlag(pBXProjectSetting.LinkerFlagArray);
                project.SetFrameworkSearchPaths(pBXProjectSetting.FrameworkSearchPaths);
                project.SetLibrarySearchPaths(pBXProjectSetting.LibrarySearchPaths);
                project.SetHeaderSearchPaths(pBXProjectSetting.HeaderSearchPaths);

                foreach (var file in pBXProjectSetting.CompileFiles)
                {
                    if (file == null) { continue; }
                    project.SetCompileFlagsForFile(file.File, file.HeadersDirectory, file.CompileFlags);
                }

                foreach (var file in pBXProjectSetting.CompileFiles)
                {
                    if (file == null) { continue; }
                    project.SetCompileFlagsForFile(file.File, file.HeadersDirectory, file.CompileFlags);
                }

                foreach (var frame in pBXProjectSetting.Frameworks)
                {
                    if (frame == null) { continue; }
                    project.AddFramework(frame.Path, frame.IsWeak);
                }

                foreach (var lib in pBXProjectSetting.Librarys)
                {
                    if (lib == null) { continue; }
                    project.AddLibrary(lib.File);
                }

                if (exportMethod != null)
                {
                    if (exportMethod.Equals("AppStore") || exportMethod.Equals("AdHoc"))
                    {
                        foreach (var capability in pBXProjectSetting.Capabilities)
                        {
                            if (capability == null || !capability.Enable) { continue; }
                            project.AddCapability(capability.PBXCapability, capability.EntitlementsFilePath, capability.AddOptionalFramework);
                        }
                    }
                }

                project.Write();
            }
            project = new QuickPBXProject(pathToBuildProject);
            if (project.CanLoad)
            {
                if (pBXProjectSetting.Folders != null)
                {
                    foreach (var folder in pBXProjectSetting.Folders)
                    {
                        if (string.IsNullOrEmpty(folder)) { continue; }
                        QuickXcodeDirectory.CopyAndAddBuildToXcode(project.Proj, project.TargetGuid, folder, pathToBuildProject, string.Empty);
                    }
                }
                project.Write();
            }
        }

        private static void ModifyXcodeCodes(string pathToBuildProject, XcodeProjectPresetSettings setting)
        {
            if (pathToBuildProject == null || setting == null || setting.ModifyCode == null) { return; }
            foreach (var native in setting.ModifyCode.CodeDatas)
            {
                if (native == null || string.IsNullOrEmpty(native.targetFile)) { continue; }

                QuickXClass xClass = new QuickXClass(pathToBuildProject, native.targetFile);
                if (xClass.CanLoad)
                {
                    xClass.Load();
                    System.Text.StringBuilder code = new System.Text.StringBuilder();
                    code.Length = 0;
                    foreach (var appendCode in native.AppendCodes)
                    {
                        if (string.IsNullOrEmpty(appendCode)) { continue; }
                        code.AppendLine(appendCode);
                    }
                    xClass.Append(native.AppendMark, code.ToString());
                    code.Length = 0;
                    foreach (var replaceCode in native.ReplaceCodes)
                    {
                        if (string.IsNullOrEmpty(replaceCode)) { continue; }
                        code.AppendLine(replaceCode);
                    }
                    xClass.Replace(native.ReplaceMark, code.ToString());
                    xClass.Write();
                }
            }
            //QXcodeClass xcodeClass = new QXcodeClass(Path.Combine(pathToBuildProject, "Classes/UnityAppController.mm"));
            //xcodeClass.Load();
            //string code = "\n#import \"GYSDK.h\"" + "\n#import \"SDK2144.h\"";
            //xcodeClass.Append("#import \"UnityAppController.h\"", code);
            //code = "-(BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url{\n" +
            //    "\treturn [GYSDK handOpenUrl:url isDeepLink:NO];\n" +
            //    "}\n\n" +
            //    "-(BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString*, id> *)options{\n" +
            //    "\treturn [GYSDK handOpenUrl:url isDeepLink:YES];\n" +
            //    "}\n\n" +
            //    "- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void(^)(NSArray * __nullable restorableObjects))restorationHandler {\n" +
            //    "\t[GYSDK handOpenUrl:userActivity.webpageURL isDeepLink:YES];\n" +
            //    "\treturn YES;\n" +
            //    "}\n\n";
            //xcodeClass.Append("- (void)applicationDidEnterBackground", code);
            //xcodeClass.Write();
        }

#endif
    }
}

#endif
