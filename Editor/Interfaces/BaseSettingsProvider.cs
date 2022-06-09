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
        public virtual bool UnlockPanel { get; set; } = true;
        public virtual System.Action OnSettingsReset => delegate { Debug.LogError("Not implemented!"); };

        public BaseSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            UnlockPanel = true;
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.FlexibleSpace();

            GUI.enabled = !UnlockPanel;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Reset to defaults", GUILayout.MinHeight(50)))
            {
                OnSettingsReset.Invoke();
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.yellow;

            string lockButtonName = (UnlockPanel) ? "Unlock Settings" : "Lock Settings";
            if (GUILayout.Button(lockButtonName, GUILayout.MinHeight(50)))
            {
                UnlockPanel = !UnlockPanel;
            }
        }

        // helper
        protected GUIContent GetLabel(string name)
        {
            return EditorGUIUtility.TrTextContent(name);
        }
    }
}