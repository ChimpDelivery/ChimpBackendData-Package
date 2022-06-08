using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace TalusBackendData.Editor
{
    internal class BackendSettingsProvider : SettingsProvider
    {
        private class Styles
        {
            public static readonly GUIContent ApiUrlLabel = EditorGUIUtility.TrTextContent(
                "Api URL:",
                "Talus dashboard url."
            );

            public static readonly GUIContent ApiTokenLabel = EditorGUIUtility.TrTextContent(
                "Api Token:",
                "Talus dashboard auth token."
            );
        }

        private SerializedObject _SerializedObject;

        private SerializedProperty _ApiUrl;
        private SerializedProperty _ApiToken;

        public BackendSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            BackendSettingsHolder.instance.SaveSettings();

            _SerializedObject = new SerializedObject(BackendSettingsHolder.instance);
            _ApiUrl = _SerializedObject.FindProperty(nameof(_ApiUrl));
            _ApiToken = _SerializedObject.FindProperty(nameof(_ApiToken));
        }

        public override void OnGUI(string searchContext)
        {
            _SerializedObject.Update();

            EditorGUILayout.BeginVertical();

            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.HelpBox(
                string.Join(
                    "\n\n",
                    "Talus Backend - Authentication",
                    "You can get the 'Api Token' from 'Talus Dashboard/Profile'"),
                MessageType.Info,
                true
            );

            EditorGUI.BeginChangeCheck();
            GUI.backgroundColor = (BackendSettingsHolder.instance.ApiUrl == string.Empty) ? Color.red : Color.green;
            _ApiUrl.stringValue = EditorGUILayout.TextField(Styles.ApiUrlLabel, _ApiUrl.stringValue);
            GUI.backgroundColor = (BackendSettingsHolder.instance.ApiToken == string.Empty) ? Color.red : Color.green;
            _ApiToken.stringValue = EditorGUILayout.PasswordField(Styles.ApiTokenLabel, _ApiToken.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                _SerializedObject.ApplyModifiedProperties();
                BackendSettingsHolder.instance.SaveSettings();
            }

            EditorGUILayout.EndVertical();
        }

        [SettingsProvider]
        public static SettingsProvider CreateBackendSettingsProvider()
        {
            return new BackendSettingsProvider(
                BackendSettingsHolder.ProviderPath,
                SettingsScope.Project,
                GetSearchKeywordsFromGUIContentProperties<Styles>()
            );
        }
    }
}