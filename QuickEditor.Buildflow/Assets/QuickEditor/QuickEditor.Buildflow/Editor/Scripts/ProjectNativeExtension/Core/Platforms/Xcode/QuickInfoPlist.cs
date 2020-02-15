#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
#if UNITY_IOS

    using System.Collections.Generic;
    using System.IO;
    using Debug = LoggerUtils;

#if UNITY_2018_OR_NEWER

    using UnityEditor.iOS.Xcode;

#else

    using UnityEditor.iOS.Xcode.Custom;

#endif

    public partial class QuickInfoPlist : System.IDisposable
    {
        protected const string INFO_PLIST_NAME = "Info.plist";

        protected const string ATS_KEY = "NSAppTransportSecurity";
        protected const string EXCEPTION_DOMAINS_KEY = "NSExceptionDomains";
        protected const string INCLUDES_SUBDOMAINS_KEY = "NSIncludesSubdomains";
        protected const string EXCEPTION_REQUIRES_FORWARD_SECRECY_KEY = "NSExceptionRequiresForwardSecrecy";
        protected const string EXCEPTION_ALLOWS_INSECURE_HTTPLOADS_KEY = "NSExceptionAllowsInsecureHTTPLoads";

        protected const string BUNDLE_URL_TYPES_KEY = "CFBundleURLTypes";
        protected const string BUNDLE_URL_TYPE_ROLE_KEY = "CFBundleTypeRole";
        protected const string BUNDLE_URL_IDENTIFIER_KEY = "CFBundleURLName";
        protected const string BUNDLE_URL_SCHEMES_KEY = "CFBundleURLSchemes";

        protected const string ALLOWS_ARBITRARY_LOADS_KEY = "NSAllowsArbitraryLoads";
        protected const string APPLICATION_QUERIES_SCHEMES_KEY = "LSApplicationQueriesSchemes";

        protected const string UI_BACKGROUND_MODES_KEY = "UIBackgroundModes";

        protected const string UI_LAUNCHI_IMAGES_KEY = "UILaunchImages"; //开场动画
        protected const string UI_LAUNCHI_STORYBOARD_NAME_KEY = "UILaunchStoryboardName~iphone";

        protected const string UI_REQUIRES_FULL_SCREEN = "UIRequiresFullScreen";

        protected const string STATUS_HIDDEN_KEY = "UIStatusBarHidden";
        protected const string STATUS_BAR_APPEARANCE_KEY = "UIViewControllerBasedStatusBarAppearance";

        protected readonly string mProjectPath;
        protected readonly string mPlistPath;
        protected PlistDocument mPlist;
        protected PlistElementDict mPlistRoot;
        protected bool mCanLoad = true;

        public QuickInfoPlist(string pathToBuildProject)
        {
            mProjectPath = pathToBuildProject;
            mPlistPath = Path.Combine(pathToBuildProject, INFO_PLIST_NAME);
        }

        public PlistDocument Plist { get { return mPlist; } }
        public PlistElementDict PlistRoot { get { return mPlistRoot; } }

        #region 文件读写相关方法

        public bool CanLoad
        {
            get
            {
                mCanLoad = true;
                if (!System.IO.File.Exists(mPlistPath))
                {
                    Debug.LogError(string.Format("InfoPlistPath : {0} not found", mPlistPath));
                    mCanLoad = false;
                }
                return mCanLoad;
            }
        }

        public void Load()
        {
            mPlist = new PlistDocument();
            mPlist.ReadFromFile(mPlistPath);
            mPlistRoot = mPlist.root;
        }

        public void Write()
        {
            mPlist.WriteToFile(mPlistPath);
        }

        public void Dispose()
        {
        }

        #endregion 文件读写相关方法

        /// <summary>
        /// 设置白名单
        /// </summary>
        public void SetApplicationQueriesSchemes(List<string> applicationQueriesSchemes)
        {
            PlistElementArray queriesSchemes;
            if (mPlistRoot.values.ContainsKey(APPLICATION_QUERIES_SCHEMES_KEY))
            {
                queriesSchemes = mPlistRoot[APPLICATION_QUERIES_SCHEMES_KEY].AsArray();
            }
            else
            {
                queriesSchemes = mPlistRoot.CreateArray(APPLICATION_QUERIES_SCHEMES_KEY);
            }

            foreach (string queriesScheme in applicationQueriesSchemes)
            {
                if (!queriesSchemes.values.Contains(new PlistElementString(queriesScheme)))
                {
                    queriesSchemes.AddString(queriesScheme);
                }
            }
        }

        /// <summary>
        /// 设置后台模式
        /// </summary>
        /// <param name="modes"></param>
        public void SetBackgroundModes(List<string> modes)
        {
            PlistElementArray backModes;
            if (mPlistRoot.values.ContainsKey(UI_BACKGROUND_MODES_KEY))
            {
                backModes = mPlistRoot[UI_BACKGROUND_MODES_KEY].AsArray();
            }
            else
            {
                backModes = mPlistRoot.CreateArray(UI_BACKGROUND_MODES_KEY);
            }

            foreach (string mode in modes)
            {
                if (!backModes.values.Contains(new PlistElementString(mode)))
                {
                    backModes.AddString(mode);
                }
            }
        }

        /// <summary>
        /// 设置URL Schemes
        /// </summary>
        public void SetURLSchemes(string role, string bundleURLName, List<string> schemes)
        {
            PlistElementArray urlTypes;
            if (mPlistRoot.values.ContainsKey(BUNDLE_URL_TYPES_KEY))
            {
                urlTypes = mPlistRoot[BUNDLE_URL_TYPES_KEY].AsArray();
            }
            else
            {
                urlTypes = mPlistRoot.CreateArray(BUNDLE_URL_TYPES_KEY);
            }

            PlistElementDict itmeDict = urlTypes.AddDict();
            itmeDict.SetString(BUNDLE_URL_TYPE_ROLE_KEY, role);
            itmeDict.SetString(BUNDLE_URL_IDENTIFIER_KEY, bundleURLName);

            PlistElementArray schemesArray = itmeDict.CreateArray(BUNDLE_URL_SCHEMES_KEY);
            if (itmeDict.values.ContainsKey(BUNDLE_URL_SCHEMES_KEY))
            {
                schemesArray = itmeDict[BUNDLE_URL_SCHEMES_KEY].AsArray();
            }
            else
            {
                schemesArray = itmeDict.CreateArray(BUNDLE_URL_SCHEMES_KEY);
            }

            for (int i = 0; i < schemesArray.values.Count; i++)
            {
                schemes.Remove(schemesArray.values[i].AsString());
            }

            foreach (string scheme in schemes)
            {
                schemesArray.AddString(scheme);
            }
        }

        /// <summary>
        /// 设置ATS（是否允许http） /* iOS9所有的app对外http协议默认要求改成https */
        /// </summary>
        public void SetATS(bool enableATS)
        {
            PlistElementDict atsDict;
            if (mPlistRoot.values.ContainsKey(ATS_KEY))
            {
                atsDict = mPlistRoot[ATS_KEY].AsDict();
            }
            else
            {
                atsDict = mPlistRoot.CreateDict(ATS_KEY);
            }
            atsDict.SetBoolean(ALLOWS_ARBITRARY_LOADS_KEY, enableATS);
        }

        /// <summary>
        /// 设置域名白名单
        /// </summary>
        /// <param name="domains"></param>
        public void SetDomains(List<string> domains)
        {
            if (domains == null || domains.Count == 0) { return; }
            PlistElementDict atsDict;
            if (mPlistRoot.values.ContainsKey(ATS_KEY))
            {
                atsDict = mPlistRoot[ATS_KEY].AsDict();
            }
            else
            {
                atsDict = mPlistRoot.CreateDict(ATS_KEY);
            }
            PlistElementDict domainsDict;
            if (atsDict.values.ContainsKey(EXCEPTION_DOMAINS_KEY))
            {
                domainsDict = atsDict[EXCEPTION_DOMAINS_KEY].AsDict();
            }
            else
            {
                domainsDict = atsDict.CreateDict(EXCEPTION_DOMAINS_KEY);
            }
            PlistElementDict domainDict;
            foreach (string domain in domains)
            {
                if (domainsDict.values.ContainsKey(domain))
                {
                    domainDict = domainsDict[domain].AsDict();
                }
                else
                {
                    domainDict = domainsDict.CreateDict(domain);
                }
                domainDict.SetBoolean(INCLUDES_SUBDOMAINS_KEY, true);
                domainDict.SetBoolean(EXCEPTION_REQUIRES_FORWARD_SECRECY_KEY, false);
                domainDict.SetBoolean(EXCEPTION_ALLOWS_INSECURE_HTTPLOADS_KEY, true);
            }
        }

        /// <summary>
        /// 设置状态栏
        /// </summary>
        public void SetStatusBar(bool enable)
        {
            mPlistRoot.SetBoolean(STATUS_HIDDEN_KEY, !enable);
            mPlistRoot.SetBoolean(STATUS_BAR_APPEARANCE_KEY, enable);
        }

        /// <summary>
        /// 设置全屏
        /// </summary>
        /// <param name="enable"></param>
        public void SetFullScreen(bool enable)
        {
            mPlistRoot.SetBoolean(UI_REQUIRES_FULL_SCREEN, enable);
        }

        /// <summary>
        /// 设置开始画面
        /// </summary>
        public void SetLaunchImages(bool delete)
        {
            if (delete)
            {
                //设置开始画面
                if (mPlistRoot.values.ContainsKey(UI_LAUNCHI_IMAGES_KEY))
                {
                    mPlistRoot.values.Remove(UI_LAUNCHI_IMAGES_KEY);
                }
                if (mPlistRoot.values.ContainsKey(UI_LAUNCHI_STORYBOARD_NAME_KEY))
                {
                    mPlistRoot.values.Remove(UI_LAUNCHI_STORYBOARD_NAME_KEY);
                }
            }
        }

        /// <summary>
        /// 设置私有数据 iOS10新的特性
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="description"></param>
        public void SetPrivacySensiticeData(List<string> datas, string description = "")
        {
            foreach (var data in datas)
            {
                mPlistRoot.SetString(data, description);
            }
        }

        ///// <summary>

        ///// 设置私有数据

        ///// </summary>

        ///// <param name="plist"></param>

        ///// <param name="privacySensiticeDataList"></param>

        //private static void SetPrivacySensiticeData(PlistDocument plist, List<XcodeProjectSetting.PrivacySensiticeData> privacySensiticeDataList)

        //{
        //    PlistElementDict rootDict = plist.root;

        //    int count = privacySensiticeDataList.Count;

        //    for (int i = 0; i < count; i++)

        //    {
        //        XcodeProjectSetting.PrivacySensiticeData data = privacySensiticeDataList[i];

        //        switch (data.type)

        //        {
        //            case XcodeProjectSetting.NValueType.String:

        //                rootDict.SetString(data.key, data.value);

        //                break;

        //            case XcodeProjectSetting.NValueType.Int:

        //                rootDict.SetInteger(data.key, int.Parse(data.value));

        //                break;

        //            case XcodeProjectSetting.NValueType.Bool:

        //                rootDict.SetBoolean(data.key, bool.Parse(data.value));

        //                break;

        //            default:

        //                rootDict.SetString(data.key, data.value);

        //                break;

        //        }

        //    }

        //}

        public void AddStringKey(string key, string val)
        {
            mPlistRoot.SetString(key, val);
            Debug.Log(string.Format("Set String -> Key : {0}, Value : {1}", key, val));
            //rootDic.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            // for share sdk 截屏
            mPlistRoot.SetString("NSPhotoLibraryUsageDescription", "We need use photo library usage");
        }

        public void AddBooleanKey(string key, bool val)
        {
            mPlistRoot.SetBoolean(key, val);
            Debug.Log(string.Format("Set Boolean -> Key : {0}, Value : {1}", key, val));
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="value"></param>
        public void SetBundleDevelopmentRegion(string value)
        {
            // location native development region
            mPlistRoot.SetString("CFBundleDevelopmentRegion", "zh_CN");
        }

        public void SetPermission(string key, string val)
        {
            mPlistRoot.SetString(key, val);
            Debug.Log(string.Format("Set Permission : {0}, Value : {1}", key, val));
        }
    }

#endif
}

#endif