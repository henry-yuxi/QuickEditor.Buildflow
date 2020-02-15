#if UNITY_EDITOR

namespace QuickEditor.Buildflow
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public abstract class AbstractPresetSettings
    {
        //[HideInInspector]
        [SerializeField]
        public bool enabled = true;

        //[HideInInspector]
        [SerializeField]
        public string presetName = "New Preset";

        //[HideInInspector]
        //protected bool Folded = false;

        //[HideInInspector]
        //public bool TagForDeletion = false;

        //[HideInInspector]
        //public int PositionOffset = 0;

        //[HideInInspector]
        //[SerializeField]
        //public string PathNameFilter = string.Empty;

        //[HideInInspector]
        //[SerializeField]
        //public string FileNameFilter = string.Empty;

        public virtual void DrawInnerGUI()
        {
            DrawFilterGUI();
            DrawGeneralPresetGUI();
            DrawAndroidPresetGUI();
            DrawiOSPresetGUI();
            DrawOptionsGUI();
        }

        protected virtual void DrawGeneralPresetGUI()
        {
        }

        protected virtual void DrawAndroidPresetGUI()
        {
        }

        protected virtual void DrawiOSPresetGUI()
        {
        }

        protected virtual void DrawOptionsGUI()
        {
        }

        protected virtual void DrawFilterGUI()
        {
            EditorGUILayout.LabelField("Preset Setting", EditorStyles.boldLabel);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), QuickEditorColors.DarkGrayX11);
            GUILayout.Space(3);

            presetName = EditorGUILayout.TextField(new GUIContent("Preset Name", "Only used for organisation"), presetName);
        }
    }
}

#endif