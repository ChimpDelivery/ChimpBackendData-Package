using UnityEditor;

using UnityEngine;

namespace TalusBackendData.Editor
{
    public static class BackendSettings
    {
        private static string _ApiUrl = "";
        private static string _ApiToken = "";
        private static string _AppId = "";

        internal static string ApiUrl
        {
            get
            {
                if (!EditorPrefs.HasKey(BackendDefinitions.BackendApiUrlPref))
                {
                    Debug.Log("Backend_Api_Url editor pref not found!");
                    return _ApiUrl;
                }

                return EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            }
            set
            {
                _ApiUrl = value;
                EditorPrefs.SetString(BackendDefinitions.BackendApiUrlPref, value);
            }
        }

        internal static string ApiToken
        {
            get
            {
                if (!EditorPrefs.HasKey(BackendDefinitions.BackendApiTokenPref))
                {
                    Debug.Log("Backend_Api_Token editor pref not found!");
                    return _ApiToken;
                }

                return EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            }
            set
            {
                _ApiToken = value;
                EditorPrefs.SetString(BackendDefinitions.BackendApiTokenPref, value);
            }
        }

        internal static string AppId
        {
            get
            {
                if (!EditorPrefs.HasKey(BackendDefinitions.BackendAppIdPref))
                {
                    Debug.Log("Backend_App_Id editor pref not found!");
                    return _AppId;
                }

                return EditorPrefs.GetString(BackendDefinitions.BackendAppIdPref);
            }
            set
            {
                _AppId = value;
                EditorPrefs.SetString(BackendDefinitions.BackendAppIdPref, value);
            }
        }
    }
}