using UnityEditor;

namespace TalusBackendData.Editor
{
    class BackendSettingsProvider : SettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new BackendSettingsProvider();
        }

        public BackendSettingsProvider() : base("Preferences/Backend Settings", SettingsScope.User)
        { }

        public override bool HasSearchInterest(string searchContext)
        {
            return base.HasSearchInterest(searchContext);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Api Url", EditorStyles.boldLabel);
            BackendSettings.ApiUrl = EditorGUILayout.TextField(EditorPrefs.GetString("Backend_Api_Url"));

            EditorGUILayout.LabelField("Api Token", EditorStyles.boldLabel);
            BackendSettings.ApiToken = EditorGUILayout.TextField(EditorPrefs.GetString("Backend_Api_Token"));

            EditorGUILayout.EndVertical();
        }
    }
}