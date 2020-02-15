#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;

    public class MainThreadEventsExecutor
    {
        private readonly Queue<object> mainThreadActions = new Queue<object>();
        private bool mainThreadUpdateEnabled;
        private readonly object actionsLock = new object();

        private static MainThreadEventsExecutor mInstance;

        private static MainThreadEventsExecutor Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new MainThreadEventsExecutor();
                return mInstance;
            }
            set
            {
                mInstance = value;
            }
        }

        public static void Push(Action callback)
        {
            Instance.PushInternal(callback);
        }

        public static void Push(IEnumerator callback)
        {
            Instance.PushInternal(callback);
        }

        private void PushInternal(object callback)
        {
            lock (actionsLock)
            {
                EnableMainThreadUpdate();
                mainThreadActions.Enqueue(callback);
            }
        }

        private void MainThreadUpdate()
        {
            lock (actionsLock)
            {
                if (mainThreadActions.Count == 0)
                    DisableMainThreadUpdate();
                else
                {
                    var count = mainThreadActions.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        var action = mainThreadActions.Dequeue();
                        if (action is Action)
                        {
                            ((Action)action)();
                        }
                        else
                        if (action is IEnumerator)
                        {
                            var enumerator = (IEnumerator)action;
                            if (enumerator.MoveNext())
                            {
                                var res = enumerator.Current;
                                mainThreadActions.Enqueue(action);
                            }
                        }
                    }
                }
            }
        }

        private void EnableMainThreadUpdate()
        {
            if (!mainThreadUpdateEnabled)
            {
                EditorApplication.update += MainThreadUpdate;
                mainThreadUpdateEnabled = true;
            }
        }

        private void DisableMainThreadUpdate()
        {
            if (mainThreadUpdateEnabled)
            {
                EditorApplication.update -= MainThreadUpdate;
                mainThreadUpdateEnabled = false;
            }
        }
    }
}

#endif
