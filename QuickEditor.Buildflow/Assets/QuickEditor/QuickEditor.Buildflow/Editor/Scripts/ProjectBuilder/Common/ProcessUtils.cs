#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    public sealed class ProcessUtils
    {
        /*
* 调用外部程序
System.Diagnostics.Process.Start("notepad.exe");        -- 打开记事本
System.Diagnostics.Process.Start("calc.exe ");                -- 打开计算器
System.Diagnostics.Process.Start("regedit.exe ");           -- 打开注册表
System.Diagnostics.Process.Start("mspaint.exe ");        -- 打开画图板
System.Diagnostics.Process.Start("write.exe ");              -- 打开写字板
System.Diagnostics.Process.Start("mplayer2.exe ");        --打开播放器
System.Diagnostics.Process.Start("taskmgr.exe ");          --打开任务管理器
System.Diagnostics.Process.Start("eventvwr.exe ");          --打开事件查看器
System.Diagnostics.Process.Start("winmsd.exe ");           --打开系统信息
System.Diagnostics.Process.Start("winver.exe ");              --打开Windows版本信息
System.Diagnostics.Process.Start("mailto: "+ address);    -- 发邮件
打开word文档
NITY_STANDALONE_WIN
  string _path = "file://" + Application.dataPath + @"/StreamingAssets/";
  System.Diagnostics.Process.Start("explorer", "/n, " + _path + "word" + ".doc");
#endif
*/

        public static void ExecuteCommand(string workDir, string args)
        {
            string exeName = "";
#if UNITY_EDITOR_WIN
            exeName = "cmd.exe";
#elif UNITY_EDITOR_OSX
        exeName = "sh";
#endif
            CreateProcRunner(exeName, workDir, args);
        }

        public static ProcRunner CreateProcRunner(string executablePath, string arguments = "", string workingDirectory = "", bool useShellExecute = false, bool createNoWindow = true)
        {
            return new ProcRunner(executablePath, arguments, workingDirectory, useShellExecute, createNoWindow);
        }

        public static void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        if (process.ProcessName == processName)
                        {
                            process.Kill();
                            UnityEngine.Debug.Log("已杀死进程");
                        }
                    }
                }
                catch (System.InvalidOperationException)
                {
                }
            }
        }

        public static bool CheckProcess(string processName)
        {
            bool isRunning = false;
            Process[] processes = Process.GetProcesses();
            int i = 0;
            foreach (Process process in processes)
            {
                try
                {
                    i++;
                    if (!process.HasExited)
                    {
                        if (process.ProcessName.Contains(processName))
                        {
                            UnityEngine.Debug.Log(processName + "正在运行");
                            isRunning = true;
                            continue;
                        }
                        else if (!process.ProcessName.Contains(processName) && i > processes.Length)
                        {
                            UnityEngine.Debug.Log(processName + "没有运行");
                            isRunning = false;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return isRunning;
        }
    }

    public class ProcRunner
    {
        private bool mIsAborted;
        private readonly Process mProcess;
        private StringBuilder mErrorBuilder;
        private StringBuilder mOutputBuilder;
        private Action<bool> mOnExited;
        private Action<string> mOnOutput;
        private Func<string, bool> mOnError;
        private Action<int, string, string> mProcessAction;
        protected const int DefaultTimeoutMs = 1000 * 60; // 1 minute

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="args"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="useShellExecute">如果需要在 Unity Editor 中使用 Log 输出外部程序执行时的输出进行查看的话, 就必须将 UseShellExecute 设置为 false</param>
        /// <param name="createNoWindow"></param>
        public ProcRunner(string fileName, string args = "", string workingDirectory = "", bool useShellExecute = false, bool createNoWindow = true)
        {
            mProcess = new Process
            {
                StartInfo = {
                    FileName = fileName,
                    Arguments = args,
                    CreateNoWindow = createNoWindow,
                    UseShellExecute = useShellExecute,
                }
            };
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                mProcess.StartInfo.WorkingDirectory = workingDirectory;
            }

            if (mProcess.StartInfo.UseShellExecute)
            {
                mProcess.StartInfo.RedirectStandardOutput = false;
                mProcess.StartInfo.RedirectStandardError = false;
                mProcess.StartInfo.RedirectStandardInput = false;
            }
            else
            {
                // 将标准输出重定向，所有输出会通过事件回调的方式在回调参数中返回
                mProcess.StartInfo.RedirectStandardOutput = true;
                // 将错误输出重定向，所有输出会通过事件回调的方式在回调参数中返回
                mProcess.StartInfo.RedirectStandardError = true;
                mProcess.StartInfo.RedirectStandardInput = true;
                mProcess.StartInfo.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                mProcess.StartInfo.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
            }
        }

        public string Execute()
        {
            mProcess.Start();
            mProcess.WaitForExit();
            return HandleExited(mProcess.StandardOutput.ReadToEnd(), mProcess.StandardError.ReadToEnd());
        }

        public void Execute(Action<bool> onExited = null, Action<string> onOutput = null, Func<string, bool> onError = null)
        {
            mOnExited = onExited;
            mOnOutput = onOutput;
            mOnError = onError;
            mErrorBuilder = new StringBuilder();
            mOutputBuilder = new StringBuilder();
            bool isOutputClosed = false, isErrorsClosed = false;
            mProcess.OutputDataReceived += (sender, args) =>
            {
                isOutputClosed = DataReceived(args, OnOutputDataReceived);
            };
            mProcess.ErrorDataReceived += (sender, args) =>
            {
                isErrorsClosed = DataReceived(args, OnErrorDataReceived);
            };
            mProcess.EnableRaisingEvents = true;
            mProcess.Exited += (sender, args) =>
            {
                WaitUntilAndDo(() => isOutputClosed && isErrorsClosed, HandleExitedAsync);
            };
            mProcess.Start();
            mProcess.BeginOutputReadLine();
            mProcess.BeginErrorReadLine();
        }

        public void Abort()
        {
            mIsAborted = true;
            try
            {
                mProcess.Kill();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private string HandleExited(string output, string error)
        {
            var exitCode = mProcess.ExitCode;
            mProcess.Close();
            if (!mIsAborted && exitCode != 0 || !string.IsNullOrEmpty(error))
            {
                throw new ExternalException(string.Format("\nOutput:\n{0}\nError Output:\n{1}", output, error));
            }
            return output;
        }

        private static bool DataReceived(DataReceivedEventArgs args, Action<string> OnReceived)
        {
            if (args.Data != null)
            {
                OnReceived(args.Data);
                return false;
            }
            return true;
        }

        private void OnOutputDataReceived(string data)
        {
            mOutputBuilder.AppendLine(data);
            if (mOnOutput != null)
                mOnOutput(data);
        }

        private void OnErrorDataReceived(string data)
        {
            if (mOnError == null || mOnError(data))
            {
                mErrorBuilder.AppendLine(data);
                mProcess.Kill();
            }
        }

        private void HandleExitedAsync()
        {
            bool success = false;
            try
            {
                HandleExited(mOutputBuilder.ToString(), mErrorBuilder.ToString());
                success = !mIsAborted;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
            finally
            {
                if (mOnExited != null)
                    MainThreadEventsExecutor.Push(() => mOnExited(success));
            }
        }

        private static void WaitUntilAndDo(Func<bool> check, Action callbcak, int timeOut = 2000)
        {
            var stopwatch = Stopwatch.StartNew();
            new Thread(() =>
            {
                while (!check() && stopwatch.ElapsedMilliseconds < timeOut)
                {
                    Thread.Sleep(50);
                }
                callbcak();
            }).Start();
        }
    }
}

#endif
