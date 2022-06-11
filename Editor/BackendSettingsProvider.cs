using System.Collections.Generic;

using UnityEditor;
using UnityEngine.UIElements;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor
{
    internal class BackendSettingsProvider : BaseSettingsProvider<BackendSettingsProvider>
    {
        public override string Title => BackendSettingsHolder.ProviderPath;
        public override string Description => "You can get the 'Api Token' from 'Talus Dashboard/Profile'";

        public override SerializedObject SerializedObject => _SerializedObject;
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

            base.OnGUI(searchContext);

            if (EditorGUI.EndChangeCheck())
            {
                _SerializedObject.ApplyModifiedProperties();
                BackendSettingsHolder.instance.SaveSettings();
            }
        }
    }
}