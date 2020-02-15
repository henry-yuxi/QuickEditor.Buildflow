#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using UnityEditor;
    using Debug = UnityEngine.Debug;

    internal static class GitRequest
    {
        public const string GIT_EXEC_PATH = "git";

        public static string CurrentBranch()
        {
            return CreateRequestGit(
                "rev-parse --abbrev-ref HEAD"
            ).Execute().Trim();
        }

        public static string Revision(bool getNumber)
        {
            return CreateRequestGit(getNumber ?
                "rev-list --count HEAD" :
                "rev-parse --short HEAD"
            ).Execute().Trim();
        }

        public static void Checkout(string branch)
        {
            Debug.Log("git fetch");
            CreateRequestGit("fetch").Execute();
            Debug.Log("Checkout branch " + branch);
            CreateRequestGit("checkout " + branch).Execute();
            Debug.Log("Checkout success");
            AssetDatabase.Refresh();
        }

        public static string FindRevision(string rev, int len, string branchForByNumber)
        {
            string findStr;
            if (branchForByNumber != null)
            {
                var count = CreateRequestGit("rev-list --count " + branchForByNumber).Execute();
                try
                {
                    var number = int.Parse(count) - int.Parse(rev);
                    if (number < 0) throw new FormatException();
                    findStr = string.Format("{0} --skip={1}", branchForByNumber, number);
                }
                catch (FormatException)
                {
                    throw new FormatException("Not valid number");
                }
            }
            else
            {
                findStr = rev;
            }
            return CreateRequestGit("--no-pager log -" + len + " " + findStr).Execute();
        }

        private static ProcRunner CreateRequestGit(string args = "")
        {
            return ProcessUtils.CreateProcRunner(GIT_EXEC_PATH, args);
        }
    }
}

#endif