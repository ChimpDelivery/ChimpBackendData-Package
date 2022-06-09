using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace TalusBackendData.Editor.Interfaces
{
    public abstract class BaseSettingsProvider<T> : SettingsProvider
    {
        public bool UnlockPanel { get; protected set; } = true;

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