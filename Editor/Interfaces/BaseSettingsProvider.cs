using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace TalusBackendData.Editor.Interfaces
{
    /// <summary>
    ///     Base Settings Provider
    /// </summary>
    public abstract class BaseSettingsProvider : SettingsProvider
    {
        //
        public abstract string Title { get; }
        public abstract string Description { get; }

        // to change properties we need to unlock panel
        public virtual bool UnlockPanel { get; set; } = true;

        //
        public virtual System.Action OnSettingsReset => delegate { Debug.LogError("Not implemented!"); };

        public abstract SerializedObject SerializedObject { get; }

        public BaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            UnlockPanel = true;
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical();

            ShowInfo();

            GUI.enabled = !UnlockPanel;

            EditorGUI.BeginChangeCheck();

            // settings holder properties
            {
                GUILayout.Space(8);

                SerializedProperty prop = SerializedObject.GetIterator();
                if (prop.NextVisible(true))
                {
                    do
                    {
                        EditorGUILayout.PropertyField(SerializedObject.FindProperty(prop.name), true);
                    }
                    while (prop.NextVisible(false));
                }
            }

            // stick buttons to the bottom
            GUILayout.FlexibleSpace();

            ShowResetButton();
            ShowLockButton();

            EditorGUILayout.EndVertical();
        }

        private void ShowInfo()
        {
            Color defaultColor = GUI.color;
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.HelpBox(Description, MessageType.Info, true);
            GUI.backgroundColor = defaultColor;
        }

        private void ShowResetButton()
        {
            GUI.enabled = !UnlockPanel;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Reset to defaults", GUILayout.MinHeight(50)))
            {
                OnSettingsReset.Invoke();
            }
        }

        private void ShowLockButton()
        {
            GUI.enabled = true;
            GUI.backgroundColor = Color.yellow;

            string lockButtonName = (UnlockPanel) ? "Unlock Settings" : "Lock Settings";

            if (GUILayout.Button(lockButtonName, GUILayout.MinHeight(50)))
            {
                UnlockPanel = !UnlockPanel;
            }
        }
    }
}
