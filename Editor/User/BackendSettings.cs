using UnityEditor;

namespace TalusBackendData.Editor.User
{
    public static class BackendSettings
    {
        public static string Path => "Talus Studio/Backend Settings";

        public static string ApiUrl
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            set => EditorPrefs.SetString(BackendDefinitions.BackendApiUrlPref, value);
        }

        public static string ApiToken
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            set => EditorPrefs.SetString(BackendDefinitions.BackendApiTokenPref, value);
        }
    }
}