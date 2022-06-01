using System.Collections.Generic;

namespace TalusBackendData.Editor
{
    public static class BackendDefinitions
    {
        public static readonly string BackendSymbol = "ENABLE_BACKEND";

        public static readonly string BackendApiUrlPref = "BACKEND_API_URL";
        public static readonly string BackendApiTokenPref = "BACKEND_API_TOKEN";
        public static readonly string BackendAppIdPref = "BACKEND_APP_ID";

        public static readonly List<string> BackendPackageList = new List<string>
{
            "com.talus.talusbackenddata",
            "com.talus.talusci",
            "com.talus.talusplayservicesresolver",
            "com.talus.talusfacebook",
            "com.talus.taluselephant"
        };
    }
}
