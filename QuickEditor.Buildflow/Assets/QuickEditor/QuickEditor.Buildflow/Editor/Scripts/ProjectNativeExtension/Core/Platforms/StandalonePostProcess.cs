#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.IO;
    using Debug = LoggerUtils;

    internal sealed partial class StandalonePostProcess
    {
        public static void Process(string pathToBuildProject)
        {
            Debug.Log("StandalonePostProcess: Starting to perform post build tasks for Windows platform.");
            if (pathToBuildProject == null) { return; }

            //GenerateWinExe(pathToBuildProject);
        }

        //[UnityEditor.MenuItem("Tools/导出自解压安装包exe")]
        //public static void Test()
        //{
        //    string path = EditorUtility.OpenFolderPanel("请选择需要压缩的目录", "../ProjectBuilder/Output/PC", "");
        //    if (string.IsNullOrEmpty(path)) return;

        //    if (!File.Exists(path + "/ylqt-Setup.exe"))
        //    {
        //        EditorUtility.DisplayDialog("", "不合法的目录", "确认");
        //        return;
        //    }

        //    GenerateWinExe(path + "/ylqt-Setup.exe");
        //}

        public static void GenerateWinExe(string pathToBuiltProject)
        {
            Debug.Log("BuildProjectPath: " + pathToBuiltProject);
            string zipRootDir = Path.GetDirectoryName(pathToBuiltProject) + "/";
            string path = Path.GetDirectoryName(pathToBuiltProject);
            int index = path.LastIndexOf("/");
            string setupName = path.Substring(index, path.Length - index);
            //string setupExeName = Path.GetFileName(pathToBuiltProject);
            string targetExePath = zipRootDir + setupName + ".exe";
            if (File.Exists(targetExePath)) File.Delete(targetExePath);

            //复制icon
            string icoFilePath = zipRootDir + "icon.ico";
            string bmpFilePath = zipRootDir + "icon.bmp";
            string sfxConfigPath = zipRootDir + "sfxConfig.txt";
            string uninstallPath = zipRootDir + "uninstall.exe";
            File.Copy("Assets/Res/Logo/PC/icon.ico", icoFilePath, true);
            File.Copy("Assets/Res/Logo/PC/icon.bmp", bmpFilePath, true);
            File.Copy("Assets/Res/Logo/PC/sfxConfig.txt", sfxConfigPath, true);
            File.Copy("Assets/Res/Logo/PC/uninstall.exe", uninstallPath, true);

            //获取winrar路径
            var regKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
            string winrarPath = null;
            try
            {//windows
                var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regKeyPath);
                winrarPath = regKey.GetValue("").ToString();
                regKey.Close();
            }
            catch (Exception)
            {//mac os
                winrarPath = "/usr/local/bin/rar";
                File.Copy("Assets/Res/Logo/PC/sfxConfigMac.txt", sfxConfigPath, true);
            }

            Debug.Log("WinRAR.exe Path=" + winrarPath);

            //        StringBuilder sfxConfig = new StringBuilder();
            //        sfxConfig.AppendLine("Title=妖恋奇谭安装程序"); //安装标题
            //        sfxConfig.AppendLine("Text=妖恋奇谭说明"); //安装说明
            //        sfxConfig.AppendLine("Path=C:\\Program Files\\ylqt\\"); //安装路径
            //        sfxConfig.AppendLine("Shortcut=D," + setupExeName + ",,,"+"妖恋奇谭,"+ "icon.ico"); //桌面快捷方式
            //        sfxConfig.AppendLine("Overwrite=1"); //覆盖所有文件
            //        sfxConfig.AppendLine("Update=U"); //更新新的或不存在的文件
            //        sfxConfig.AppendLine(
            //@"License=最终用户许可协议书
            //{
            //所有版权于 妖恋奇谭 均属于作者所专有。
            //此程序是共享软件，任何人在测试期限内均可以使用此软件。
            //在测试期限过后，您“必须”注册。
            //}"
            //        ); //许可
            //File.WriteAllText(sfxConfigPath, sfxConfig.ToString());

            string argument = "";

            argument += "a -r -sfx "; //压缩，递归，自解压
            argument += "-iimg" + "icon.bmp "; //解压图标
            argument += "-iicon" + "icon.ico "; //解压图标
            argument += "-zsfxConfig.txt "; //注释

            argument += targetExePath + " * "; //指定目录

            Debug.Log("argument=" + argument);
            try
            {//运行进程
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = winrarPath;
                p.StartInfo.WorkingDirectory = zipRootDir;
                p.StartInfo.Arguments = argument;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;//与CreateNoWindow联合使用可以隐藏进程运行的窗体
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.EnableRaisingEvents = true;                      // 启用Exited事件
                                                                   //p.Exited += p_Exited;
                p.Start();

                p.WaitForExit();

                if (p.ExitCode == 0)//正常退出
                {
                    //TODO记录日志
                    Debug.Log("执行完毕！");
                }
                else
                {
                    Debug.LogError("error exitcode " + p.ExitCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("系统错误：", ex);
            }
        }
    }
}

#endif