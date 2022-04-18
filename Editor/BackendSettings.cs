using UnityEditor;

using UnityEngine;

namespace TalusBackendData.Editor
{
    static class BackendSettings
    {
        private static string _ApiUrl = "";
        internal static string _ApiToken = "";

        internal static string ApiUrl
        {
            get
            {
                if (!EditorPrefs.HasKey("Backend_Api_Url"))
                {
                    Debug.Log("Backend_Api_Url editor pref not found!");
                    return _ApiUrl;
                }

                return EditorPrefs.GetString("Backend_Api_Url");
            }
            set
            {
                _ApiUrl = value;
                EditorPrefs.SetString("Backend_Api_Url", value);
            }
        }

        internal static string ApiToken
        {
            get
            {
                if (!EditorPrefs.HasKey("Backend_Api_Token"))
                {
                    Debug.Log("Backend_Api_Token editor pref not found!");
                    return _ApiToken;
                }

                return EditorPrefs.GetString("Backend_Api_Token");
            }
            set
            {
                _ApiToken = value;
                EditorPrefs.SetString("Backend_Api_Token", value);
            }
        }
    }
}