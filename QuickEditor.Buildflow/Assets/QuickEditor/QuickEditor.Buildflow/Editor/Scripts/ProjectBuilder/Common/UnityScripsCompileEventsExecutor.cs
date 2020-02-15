#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEditor;

    public class UnityScripsCompileEventsExecutor
    {
        private static string TempMethodNamesKey = "CompileExecutor_TempMethodNames";

        public static void Make<T>(Action<T> callback, T arg)
        {
            if (!typeof(T).IsSerializable && !typeof(ISerializable).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException("A serializable Type of arg is required");
            if (!callback.Method.IsStatic)
                throw new InvalidOperationException("A method of callback must be static");
            if (callback.Method.DeclaringType == null)
                throw new InvalidOperationException("A method must have declaring type");

            Set(callback.Method.DeclaringType.FullName, callback.Method.Name, Serialize(arg));
        }

        protected static void Set(string className, string methodName, string arg = null)
        {
            string methoedName = className + "." + methodName + "(" + arg + ")";
            AddTempMethodName(methoedName);
        }

        private static string[] GetTempMethodNameArr()
        {
            return GetTempMethodNames().Split(';').Where(value => value != null && value.Trim() != "").ToArray();
        }

        private static void AddTempMethodName(string value)
        {
            SetTempMethodNames(GetTempMethodNames() + ";" + value);
        }

        private static string GetTempMethodNames()
        {
            if (EditorPrefs.HasKey(TempMethodNamesKey))
            {
                return EditorPrefs.GetString(TempMethodNamesKey);
            }
            return "";
        }

        private static void SetTempMethodNames(string value)
        {
            EditorPrefs.SetString(TempMethodNamesKey, value);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var methods = GetTempMethodNameArr();
            foreach (var method in methods)
            {
                if (method == null) { return; }
                var pointIndex = method.LastIndexOf(".");
                var left = method.LastIndexOf("(");
                var right = method.LastIndexOf(")");
                string className = method.Substring(0, pointIndex);
                string methodName = method.Substring(pointIndex + 1, left - pointIndex - 1);
                string argName = method.Substring(left + 1, right - left - 1);
                var arg = Deserialize(argName);
                Type.GetType(className).GetMethod(methodName).Invoke(null, new[] { arg });

                //Type type = Type.GetType(className);
                //if (type == null)
                //{
                //    continue;
                //}
                //var method1 = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { }, null);
                //var method2 = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
                //if (method1 == null && method2 == null)
                //{
                //    continue;
                //}
                //if (method1 != null && argName.Trim() == "")
                //    method1.Invoke(null, null);
                //if (method2 != null && argName.Trim() != "")
                //    method2.Invoke(null, new object[] { argName });
            }
            SetTempMethodNames("");

            // Invoke();
        }

        private static string Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static object Deserialize(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}

#endif
