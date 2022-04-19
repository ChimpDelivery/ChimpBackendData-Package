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

        public BackendSettingsProvider() : base("Preferences/Talus/Backend Settings", SettingsScope.User)
        { }

        public override bool HasSearchInterest(string searchContext)
        {
            return base.HasSearchInterest(searchContext);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Api Url:", EditorStyles.boldLabel);
            BackendSettings.ApiUrl = EditorGUILayout.TextField(EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref));

            EditorGUILayout.LabelField("Api Token:", EditorStyles.boldLabel);
            BackendSettings.ApiToken = EditorGUILayout.TextField(EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref));

            EditorGUILayout.EndVertical();
        }
    }
}