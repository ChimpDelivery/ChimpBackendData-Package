using UnityEditor;

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