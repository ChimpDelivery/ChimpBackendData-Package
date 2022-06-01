using System.Collections.Generic;

namespace TalusBackendData.Editor
{
    public static class BackendDefinitions
    {
        public static readonly string BackendSymbol = "ENABLE_BACKEND";

        public static readonly string BackendApiUrlPref = "BACKEND_API_URL";
        public static readonly string BackendApiTokenPref = "BACKEND_API_TOKEN";
        public static readonly string BackendAppIdPref = "BACKEND_APP_ID";

        public static readonly Dictionary<string, string> BackendPackages = new Dictionary<string, string>
        {
            { "com.talus.talusbackenddata", "https://github.com/TalusStudio/TalusBackendData.git" },
            { "com.talus.talusci", "https://github.com/TalusStudio/TalusCI.git" },
            { "com.talus.talusplayservicesresolver", "https://github.com/TalusStudio/TalusPlayServicesResolver-Package.git" },
            { "com.talus.talusfacebook", "https://github.com/TalusStudio/TalusFacebook-Package.git" },
            { "com.talus.taluselephant", "https://github.com/TalusStudio/TalusElephant-Package.git" }
        };
    }
}
