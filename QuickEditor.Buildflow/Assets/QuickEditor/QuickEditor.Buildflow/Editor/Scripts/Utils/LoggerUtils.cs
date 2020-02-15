namespace QuickEditor.Buildflow
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public sealed partial class LoggerUtils
    {
        public const string DefaultPrefix = "> ";
        public const string Assembly = "QuickEditor.Buildflow";
        public const string FORMAT = DefaultPrefix + "<b>[{0}]</b> --> {1}";

        public static bool EnableLog = true; // 是否启用日志，仅可控制普通级别的日志的启用与关闭，LogError和LogWarn都是始终启用的。
        public static bool EnableTime = false;

        public static void Log(string message)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Info, message);
            Debug.Log(finalMessage);
        }

        public static void Log(string message, params object[] args)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Info, string.Format(message, args));
            Debug.Log(finalMessage);
        }

        public static void LogError(string message)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Error, message);
            Debug.LogError(finalMessage);
        }

        public static void LogError(string message, params object[] args)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Error, string.Format(message, args));
            Debug.LogError(finalMessage);
        }

        public static void LogWarning(string message)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Warning, message);
            Debug.LogWarning(finalMessage);
        }

        public static void LogWarning(string message, params object[] args)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Warning, string.Format(message, args));
            Debug.LogWarning(finalMessage);
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, Priority.Info, string.Format(format, args));
            Debug.Log(finalMessage);
        }

        public static void LogFormat(Priority priority, string format, params object[] args)
        {
            if (!EnableLog) { return; }
            string finalMessage = GetContent(Assembly, priority, string.Format(format, args));
            switch (priority)
            {
                case Priority.FatalError:

                case Priority.Error:
                    Debug.LogError(finalMessage);
                    break;

                case Priority.Warning:
                    Debug.LogWarning(finalMessage);
                    break;

                case Priority.Info:
                    Debug.Log(finalMessage);
                    break;
            }
        }

        private static string GetContent(string assembly, Priority priority, string message, bool shouldColour = false)
        {
            string tagColour = "white";
            string priortiyColour = priorityToColour[priority];

            if (EnableTime)
            {
                if (shouldColour)
                {
                    return string.Format("{0}<b><color={1}>[{2}]</color></b> --> {3} <color={4}>{5}</color>", DefaultPrefix, tagColour, assembly, GetLogTime(), priortiyColour, message);
                }

                return string.Format("{0}<b>[{1}]</b> --> {2} {3}", DefaultPrefix, assembly, GetLogTime(), message);
            }
            else
            {
                if (shouldColour)
                {
                    return string.Format("{0}<b><color={1}>[{2}]</color></b> --> <color={3}>{4}</color>", DefaultPrefix, tagColour, assembly, priortiyColour, message);
                }

                return string.Format("{0}<b>[{1}]</b> --> {2}", DefaultPrefix, assembly, message);
            }
        }

        private static string GetLogTag(object obj)
        {
            FieldInfo field = obj.GetType().GetField("LOG_TAG");
            if (field != null)
            {
                return (string)field.GetValue(obj);
            }
            return obj.GetType().Name;
        }

        internal static string GetLogTime()
        {
            return string.Format("[{0}]", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
        }

        internal static string GetDate()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        private static readonly Dictionary<Priority, string> priorityToColour = new Dictionary<Priority, string>
        {
		#if UNITY_PRO_LICENSE
            { Priority.Info, "white" },
		#else
		    { Priority.Info, "black" },
		#endif
            { Priority.Warning, "orange" },
            { Priority.Error, "red" },
            { Priority.FatalError, "red" },
        };

        public enum Priority
        {
            Info = 1,
            Warning,
            Error,
            FatalError,
        }

#if UNITY_EDITOR

        internal sealed class LoggerRedirect
        {
            private static readonly System.Text.RegularExpressions.Regex LogRegex = new Regex(@" \(at (.+)\:(\d+)\)\r?\n");

            [UnityEditor.Callbacks.OnOpenAsset(0)]
            private static bool OnOpenAsset(int instanceId, int line)
            {
                string selectedStackTrace = GetSelectedStackTrace();
                if (string.IsNullOrEmpty(selectedStackTrace))
                {
                    return false;
                }

                if (!selectedStackTrace.Contains("LoggerUtils"))
                {
                    return false;
                }

                Match match = LogRegex.Match(selectedStackTrace);
                if (!match.Success)
                {
                    return false;
                }

                // 跳过第一次匹配的堆栈
                match = match.NextMatch();
                if (!match.Success)
                {
                    return false;
                }
                if (!selectedStackTrace.Contains("ETHotfix.Log"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Application.dataPath.Replace("Assets", "") + match.Groups[1].Value, int.Parse(match.Groups[2].Value));

                    return true;
                }
                else
                {
                    // 跳过第2次匹配的堆栈
                    match = match.NextMatch();
                    if (!match.Success)
                    {
                        return false;
                    }
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Application.dataPath.Replace("Assets", "") + match.Groups[1].Value, int.Parse(match.Groups[2].Value));

                    return true;
                }
            }

            private static string GetSelectedStackTrace()
            {
                Assembly editorWindowAssembly = typeof(UnityEditor.EditorWindow).Assembly;
                if (editorWindowAssembly == null)
                {
                    return null;
                }

                System.Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
                if (consoleWindowType == null)
                {
                    return null;
                }

                FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
                if (consoleWindowFieldInfo == null)
                {
                    return null;
                }

                UnityEditor.EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as UnityEditor.EditorWindow;
                if (consoleWindow == null)
                {
                    return null;
                }

                if (consoleWindow != UnityEditor.EditorWindow.focusedWindow)
                {
                    return null;
                }

                FieldInfo activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                if (activeTextFieldInfo == null)
                {
                    return null;
                }

                return (string)activeTextFieldInfo.GetValue(consoleWindow);
            }
        }

#endif
    }
}