using System.Collections.Generic;

using UnityEditor;
using UnityEngine.UIElements;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    internal class IOSSettingsProvider : BaseSettingsProvider
    {
        public override string Title => "Talus Studio/3. iOS Providers";
        public override string Description => "iOS specific data providers";

        public override SerializedObject SerializedObject => _SerializedObject;
        private SerializedObject _SerializedObject;

        [SettingsProvider]
        public static SettingsProvider CreateiOSSettingsProvider()
        {
            return new IOSSettingsProvider("Talus Studio/3. iOS Providers", SettingsScope.Project);
        }

        public IOSSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            IOSSettingsHolder.instance.SaveSettings();

            _SerializedObject = new SerializedObject(IOSSettingsHolder.instance);
        }

        public override void OnGUI(string searchContext)
        {
            _SerializedObject.Update();

            base.OnGUI(searchContext);

            if (EditorGUI.EndChangeCheck())
            {
                _SerializedObject.ApplyModifiedProperties();
                IOSSettingsHolder.instance.SaveSettings();
            }
        }
    }
}