using UnityEditor;

namespace TalusBackendData.Editor.User
{
    public static class BackendSettings
    {
        public static string ApiUrl
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            set
            {
                EditorPrefs.SetString(BackendDefinitions.BackendApiUrlPref, value);
            }
        }

        public static string ApiToken
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            set
            {
                EditorPrefs.SetString(BackendDefinitions.BackendApiTokenPref, value);
            }
        }

        public static string AppId
        {
            get => EditorPrefs.GetString(BackendDefinitions.BackendAppIdPref);
            set
            {
                EditorPrefs.SetString(BackendDefinitions.BackendAppIdPref, value);
            }
        }
    }
}