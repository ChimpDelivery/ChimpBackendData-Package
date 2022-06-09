using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor
{
    internal class BackendSettingsProvider : BaseSettingsProvider<BackendSettingsProvider>
    {
        private SerializedObject _SerializedObject;
        private SerializedProperty _ApiUrl;
        private SerializedProperty _ApiToken;

        [SettingsProvider]
        public static SettingsProvider CreateBackendSettingsProvider()
        {
            return new BackendSettingsProvider(BackendSettingsHolder.ProviderPath, SettingsScope.Project);
        }

        public BackendSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            BackendSettingsHolder.instance.SaveSettings();

            _SerializedObject = new SerializedObject(BackendSettingsHolder.instance);
            _ApiUrl = _SerializedObject.FindProperty(nameof(_ApiUrl));
            _ApiToken = _SerializedObject.FindProperty(nameof(_ApiToken));
        }

        public override void OnGUI(string searchContext)
        {
            _SerializedObject.Update();

            EditorGUILayout.BeginVertical();
            {
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.HelpBox(
                    string.Join(
                        "\n\n",
                        $"{BackendSettingsHolder.ProviderPath}",
                        "You can get the 'Api Token' from 'Talus Dashboard/Profile'"),
                    MessageType.Info,
                    true
                );

                GUILayout.Space(8);

                EditorGUI.BeginChangeCheck();

                GUI.enabled = !UnlockPanel;
                {

                    GUI.backgroundColor = (BackendSettingsHolder.instance.ApiUrl == string.Empty) ? Color.red : Color.green;
                    _ApiUrl.stringValue = EditorGUILayout.TextField(GetLabel(_ApiUrl.displayName), _ApiUrl.stringValue);

                    GUI.backgroundColor = (BackendSettingsHolder.instance.ApiToken == string.Empty) ? Color.red : Color.green;
                    _ApiToken.stringValue = EditorGUILayout.PasswordField(GetLabel(_ApiToken.displayName), _ApiToken.stringValue);
                }

                // unlock button
                base.OnGUI(searchContext);

                if (EditorGUI.EndChangeCheck())
                {
                    _SerializedObject.ApplyModifiedProperties();
                    BackendSettingsHolder.instance.SaveSettings();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}