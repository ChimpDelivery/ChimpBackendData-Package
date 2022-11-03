using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace TalusBackendData.Editor.Interfaces
{
    /// <summary>
    ///     Base Settings Provider
    /// </summary>
    public abstract class BaseSettingsProvider<T> : SettingsProvider
    {
        // 
        public virtual string Title => "Default Panel Title";
        public virtual string Description => "Default Panel Description";
        
        //
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

            // panel info box
            {
                Color defaultColor = GUI.color;
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.HelpBox(Description, MessageType.Info, true);
                GUI.backgroundColor = defaultColor;
            }

            // to change properties we need to unlock panel
            GUI.enabled = !UnlockPanel;

            EditorGUI.BeginChangeCheck();

            // settings holder properties
            {
                GUILayout.Space(8);

                SerializedProperty serializedProperty = SerializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.name == "m_Script") { continue; }

                    EditorGUILayout.PropertyField(serializedProperty);
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
    }
}