#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public static class CommandLineArgs
    {
        #region Field

        private static StringBuilder mStringBuilder;

        private static Dictionary<string, object> CommandLineArgsSet;

        #endregion Field

        #region Constructor

        static CommandLineArgs()
        {
            CommandLineArgsSet = new Dictionary<string, object>();
            ConstructCommandLineArgs();
        }

        #endregion Constructor

        #region Method

        public static bool InBatchMode()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            foreach (string command in commandLineArgs)
            {
                if (command.ToLower().Contains("batchmode"))
                {
                    return true;
                }
            }
            return false;
        }

        public static void Reset()
        {
            if (CommandLineArgsSet == null) { CommandLineArgsSet = new Dictionary<string, object>(); }
            CommandLineArgsSet.Clear();
            if (mStringBuilder == null) { mStringBuilder = new StringBuilder(); }
            mStringBuilder.Length = 0;
            ConstructCommandLineArgs();
        }

        private static void ConstructCommandLineArgs()
        {
            int index = 0;
            string[] commandLineArgsRaw = Environment.GetCommandLineArgs();
            while (index < commandLineArgsRaw.Length)
            {
                string arg = commandLineArgsRaw[index];
                if (arg.StartsWith("-"))
                {
                    List<string> values = new List<string>();
                    bool valuesKeep = false;
                    index += 1;
                    while (index < commandLineArgsRaw.Length)
                    {
                        string value = commandLineArgsRaw[index];

                        if (value.StartsWith("-"))
                        {
                            break;
                        }
                        else
                        {
                            if (value.EndsWith(","))
                            {
                                value = value.TrimEnd(',');
                                valuesKeep = true;
                            }
                            values.Add(value);
                            index += 1;
                        }
                    }

                    if (!CommandLineArgsSet.ContainsKey(arg))
                    {
                        if (values.Count == 0)
                        {
                            CommandLineArgsSet.Add(arg, null);
                        }
                        else if (values.Count == 1 && !valuesKeep)

                        {
                            CommandLineArgsSet.Add(arg, values[0]);
                        }
                        else
                        {
                            CommandLineArgsSet.Add(arg, values.ToArray());
                        }
                    }
                }
                else
                {
                    index += 1;
                }
            }
        }

        public new static string ToString()
        {
            if (mStringBuilder == null)
            {
                mStringBuilder = new StringBuilder();
                foreach (KeyValuePair<string, object> arg in CommandLineArgsSet)
                {
                    mStringBuilder.Append(arg.Key + " : ");
                    if (arg.Value != null)
                    {
                        if (arg.Value.GetType().IsArray)
                        {
                            object[] values = (object[])arg.Value;
                            foreach (object value in values)
                            {
                                mStringBuilder.Append((value == null ? " " : value.ToString()) + ", ");
                            }
                        }
                        else
                        {
                            mStringBuilder.Append(arg.Value);
                        }
                    }
                    mStringBuilder.Append("\n");
                }
            }
            return mStringBuilder.ToString();
        }

        public static bool GetValueAs<T>(string argumentName, out T value)
        {
            value = default(T);
            try
            {
                value = (T)CommandLineArgsSet[argumentName];
                return true;
            }
            catch
            {
                Debug.LogError("CommandLineArgs.cs - GetValueAs<T>() - Can't retrieve any custom argument named [" + argumentName + "] in the command line [" + ToString() + "].");
                return false;
            }
        }

        public static bool GetValueAsInt(string argumentName, out int value)
        {
            value = default(int);
            try
            {
                value = int.Parse((string)CommandLineArgsSet[argumentName]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValueAsFloat(string argumentName, out float value)
        {
            value = default(float);
            try
            {
                value = float.Parse((string)CommandLineArgsSet[argumentName]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValueAsString(string argumentName, out string value)
        {
            value = null;
            try
            {
                value = ((string)CommandLineArgsSet[argumentName]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValueAsBool(string argumentName, out bool value)
        {
            value = default(bool);
            try
            {
                // NOTE:
                // If the value is larger than 0, it becomes true.
                value = int.Parse((string)CommandLineArgsSet[argumentName]) > 0;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValuesAsInt(string argumentName, out int[] values)
        {
            values = null;
            try
            {
                string[] valuesTemp = (string[])CommandLineArgsSet[argumentName];
                values = new int[valuesTemp.Length];
                for (int i = 0; i < valuesTemp.Length; i++)
                {
                    values[i] = int.Parse(valuesTemp[i]);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValuesAsFloat(string argumentName, out float[] values)
        {
            values = null;
            try
            {
                string[] valuesTemp = (string[])CommandLineArgsSet[argumentName];
                values = new float[valuesTemp.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = float.Parse(valuesTemp[i]);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValuesAsString(string argumentName, out string[] values)

        {
            values = null;
            try
            {
                values = (string[])CommandLineArgsSet[argumentName];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetValuesAsBool(string argumentName, out bool[] values)
        {
            values = null;
            try
            {
                string[] valuesTemp = (string[])CommandLineArgsSet[argumentName];
                values = new bool[valuesTemp.Length];
                for (int i = 0; i < valuesTemp.Length; i++)
                {
                    values[i] = int.Parse(valuesTemp[i]) > 0;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasArgument(string argumentName)
        {
            return CommandLineArgsSet.ContainsKey(argumentName);
        }

        #endregion Method
    }
}

#endif
