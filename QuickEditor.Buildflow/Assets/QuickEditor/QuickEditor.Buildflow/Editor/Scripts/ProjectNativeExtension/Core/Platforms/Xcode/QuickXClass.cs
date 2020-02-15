#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System.IO;
    using Debug = LoggerUtils;

    public partial class QuickXClass : System.IDisposable
    {
        protected const string CLASS_PATH = "Classes/{0}";

        protected readonly string mFilePath;
        protected string mContent;
        protected bool mCanLoad = true;

        public QuickXClass(string pathToBuildProject, string fileName)
        {
            mFilePath = GetCClassFileName(pathToBuildProject, fileName);
        }

        protected string GetCClassFileName(string buildPath, string fileName)
        {
            return Path.Combine(buildPath, string.Format(CLASS_PATH, fileName));
        }

        #region 文件读写相关方法

        public bool CanLoad
        {
            get
            {
                mCanLoad = true;
                if (!System.IO.File.Exists(mFilePath))
                {
                    Debug.LogError(string.Format("FilePath : {0} not found", mFilePath));
                    mCanLoad = false;
                }
                return mCanLoad;
            }
        }

        public void Load()
        {
            StreamReader reader = new StreamReader(mFilePath);
            mContent = reader.ReadToEnd();
            reader.Close();
        }

        public void Write()
        {
            StreamWriter writer = new StreamWriter(mFilePath);
            writer.Write(mContent);
            writer.Close();
        }

        public void Dispose()
        {
        }

        #endregion 文件读写相关方法

        public void Append(string mark, string text)
        {
            if (string.IsNullOrEmpty(mark))
            {
                Debug.LogError(string.Format("File : {0} mark: {1} is null", mFilePath, mark));
                return;
            }
            if (mContent.Contains(text)) mContent.Replace(text, string.Empty);

            int beginIndex = mContent.IndexOf(mark);
            if (beginIndex == -1)
            {
                Debug.LogError(string.Format("File : {0} not found append mark: {1}", mFilePath, mark));
                return;
            }
            int endIndex = mContent.LastIndexOf("\n", beginIndex + mark.Length);
            mContent = mContent.Substring(0, endIndex) + "\n" + text + "\n" + mContent.Substring(endIndex);
        }

        public void Replace(string mark, string text)
        {
            if (string.IsNullOrEmpty(mark))
            {
                Debug.LogError(string.Format("File : {0} mark: {1} is null", mFilePath, mark));
                return;
            }
            int beginIndex = mContent.IndexOf(mark);
            if (beginIndex == -1)
            {
                Debug.LogError(string.Format("File : {0} not found replace mark: {1}", mFilePath, mark));
                return;
            }
            mContent = mContent.Replace(mark, text);
        }
    }
}

#endif