using UnityEditor;

namespace TalusBackendData.Editor.User
{
    internal class BackendSettingsProvider : SettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new BackendSettingsProvider();
        }

        public BackendSettingsProvider() : base(BackendSettings.Path, SettingsScope.Project)
        { }

        public override bool HasSearchInterest(string searchContext)
        {
            return base.HasSearchInterest(searchContext);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Api URL:", EditorStyles.boldLabel);
            BackendSettings.ApiUrl = EditorGUILayout.TextField(EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref));

            EditorGUILayout.LabelField("Api Token:", EditorStyles.boldLabel);
            BackendSettings.ApiToken = EditorGUILayout.TextField(EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref));

            EditorGUILayout.EndVertical();
        }
    }
}