using UnityEditor;

namespace TalusBackendData.Editor
{
    internal class BackendSettingsProvider : SettingsProvider
    {
        public string ApiUrl
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            set => EditorPrefs.SetString(BackendDefinitions.BackendApiUrlPref, value);
        }

        public string ApiToken
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            set => EditorPrefs.SetString(BackendDefinitions.BackendApiTokenPref, value);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new BackendSettingsProvider();
        }

        public BackendSettingsProvider() : base(BackendDefinitions.ProviderPath, SettingsScope.Project)
        { }

        public override bool HasSearchInterest(string searchContext)
        {
            return base.HasSearchInterest(searchContext);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Api URL:", EditorStyles.boldLabel);
            ApiUrl = EditorGUILayout.TextField(EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref));

            EditorGUILayout.LabelField("Api Token:", EditorStyles.boldLabel);
            ApiToken = EditorGUILayout.TextField(EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref));

            EditorGUILayout.EndVertical();
        }
    }
}