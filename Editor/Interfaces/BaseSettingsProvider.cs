using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace TalusBackendData.Editor.Interfaces
{
    /// <summary>
    ///     Base Settings Provider
    /// </summary>
    public abstract class BaseSettingsProvider<T> : SettingsProvider, ISettingsProvider
    {
        public abstract string Title { get; }
        public abstract string Description { get; }
        public virtual bool UnlockPanel { get; set; } = true;

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

            // panel info box
            {
                Color defaultColor = GUI.color;
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.HelpBox(string.Join("\n\n", Title, Description), MessageType.Info, true);
                GUI.backgroundColor = defaultColor;
            }

            // to change properties we need to unlock panel
            GUI.enabled = !UnlockPanel;

            // settings holder properties
            {
                GUILayout.Space(8);
                EditorGUI.BeginChangeCheck();

                SerializedProperty serializedProperty = SerializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.name == "m_Script") { continue; }

                    serializedProperty.stringValue = EditorGUILayout.TextField(
                        GetLabel(serializedProperty.displayName),
                        serializedProperty.stringValue
                    );
                }
            }

            // stick buttons to the bottom
            GUILayout.FlexibleSpace();

            // reset button
            {
                GUI.enabled = !UnlockPanel;
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button("Reset to defaults", GUILayout.MinHeight(50)))
                {
                    OnSettingsReset.Invoke();
                }
            }

            // unlock button
            {
                GUI.enabled = true;
                GUI.backgroundColor = Color.yellow;

                string lockButtonName = (UnlockPanel) ? "Unlock Settings" : "Lock Settings";
                if (GUILayout.Button(lockButtonName, GUILayout.MinHeight(50)))
                {
                    UnlockPanel = !UnlockPanel;
                }
            }

            EditorGUILayout.EndVertical();
        }

        // helper
        protected GUIContent GetLabel(string name)
        {
            return EditorGUIUtility.TrTextContent(name);
        }
    }
}