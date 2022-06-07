using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor
{
    public static class BackendDefinitions
    {
        // settings provider path.
        public static string ProviderPath => "Talus Studio/Backend Settings";

        // backend auth.
        public static readonly string BackendApiUrlPref = "BACKEND_API_URL";
        public static readonly string BackendApiTokenPref = "BACKEND_API_TOKEN";
        public static readonly string BackendAppIdPref = "BACKEND_APP_ID";

        public static string ApiUrl => EditorPrefs.GetString(BackendApiUrlPref);
        public static string ApiToken => EditorPrefs.GetString(BackendApiTokenPref);
        public static string AppId => EditorPrefs.GetString(BackendAppIdPref);

        // backend packages.
        public static readonly Dictionary<string, string> Packages = new Dictionary<string, string>
        {
            { "talus-framework",    "com.talus.talusframework" },
            { "talus-kit",          "com.talus.taluskit" },
            { "talus-backenddata",  "com.talus.talusbackenddata" },
            { "talus-ci",           "com.talus.talusci" },
            { "talus-settings",     "com.talus.talussettings" },
            { "talus-playservices", "com.talus.talusplayservicesresolver" },
            { "talus-facebook",     "com.talus.talusfacebook" },
            { "talus-elephant",     "com.talus.taluselephant" }
        };

        // unity backend define symbol.
        public static readonly string BackendSymbol = "ENABLE_BACKEND";

        public static void AddBackendSymbol()
        {
            if (DefineSymbols.Contains(BackendSymbol)) { return; }

            DefineSymbols.Add(BackendSymbol);
            Debug.Log($"[TalusBackendData-Package] {BackendSymbol} define symbol adding...");
        }

        public static void RemoveBackendSymbol()
        {
            if (!DefineSymbols.Contains(BackendSymbol)) { return; }

            DefineSymbols.Remove(BackendSymbol);
            Debug.Log($"[TalusBackendData-Package] {BackendSymbol} define symbol removing...");
        }
    }
}
