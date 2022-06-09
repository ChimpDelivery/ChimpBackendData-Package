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
        }

        public override void OnGUI(string searchContext)
        {
            _SerializedObject.Update();

            EditorGUILayout.BeginVertical();
            {
                Color defaultColor = GUI.color;
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.HelpBox(
                    string.Join(
                        "\n\n",
                        $"{BackendSettingsHolder.ProviderPath}",
                        "You can get the 'Api Token' from 'Talus Dashboard/Profile'"),
                    MessageType.Info,
                    true
                );
                GUI.backgroundColor = defaultColor;

                GUILayout.Space(8);
                EditorGUI.BeginChangeCheck();
                GUI.enabled = !UnlockPanel;
                {
                    SerializedProperty serializedProperty = _SerializedObject.GetIterator();
                    while (serializedProperty.NextVisible(true))
                    {
                        if (serializedProperty.name == "m_Script") { continue; }
                        if (serializedProperty.name == "_AppId") { continue; }

                        serializedProperty.stringValue = EditorGUILayout.PasswordField(
                            GetLabel(serializedProperty.displayName),
                            serializedProperty.stringValue
                        );
                    }
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