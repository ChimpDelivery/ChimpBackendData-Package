using UnityEditor;

namespace TalusBackendData.Editor
{
    public static class BackendSettings
    {
        private static string _ApiUrl = "";
        private static string _ApiToken = "";
        private static string _AppId = "";

        public static string ApiUrl
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            set
            {
                _ApiUrl = value;
                EditorPrefs.SetString(BackendDefinitions.BackendApiUrlPref, value);
            }
        }

        public static string ApiToken
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            set
            {
                _ApiToken = value;
                EditorPrefs.SetString(BackendDefinitions.BackendApiTokenPref, value);
            }
        }

        public static string AppId
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendAppIdPref);
            set
            {
                _AppId = value;
                EditorPrefs.SetString(BackendDefinitions.BackendAppIdPref, value);
            }
        }
    }
}