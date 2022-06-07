using System.Collections.Generic;

namespace TalusBackendData.Editor
{
    public static class BackendDefinitions
    {
        public static readonly string BackendSymbol = "ENABLE_BACKEND";

        public static readonly string BackendApiUrlPref = "BACKEND_API_URL";
        public static readonly string BackendApiTokenPref = "BACKEND_API_TOKEN";

        public static readonly string BackendAppIdPref = "BACKEND_APP_ID";

        public static readonly Dictionary<string, string> Packages = new Dictionary<string, string>
        {
            { "talus-framework", "com.talus.talusframework" },
            { "talus-kit", "com.talus.taluskit" },
            { "talus-backenddata", "com.talus.talusbackenddata" },
            { "talus-ci", "com.talus.talusci" },
            { "talus-settings", "com.talus.talussettings" },
            { "talus-playservicesresolver", "com.talus.talusplayservicesresolver" },
            { "talus-facebook", "com.talus.talusfacebook" },
            { "talus-elephant", "com.talus.taluselephant" }
        };
    }
}
