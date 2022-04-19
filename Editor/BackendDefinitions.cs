using System.Collections.Generic;

namespace TalusBackendData.Editor
{
    public static class BackendDefinitions
    {
        public static readonly string BackendSymbol = "ENABLE_BACKEND";

        public static readonly string BackendApiUrlPref = "Backend_Api_Url";

        public static readonly string BackendApiTokenPref = "Backend_Api_Token";

        public static readonly Dictionary<string, string> BackendPackages = new Dictionary<string, string>
        {
            { "com.talus.talusplayservicesresolver", "https://github.com/TalusStudio/TalusPlayServicesResolver-Package.git" },
            { "com.talus.talusfacebook", "https://github.com/TalusStudio/TalusFacebook-Package.git" },
            { "com.talus.taluselephant", "https://github.com/TalusStudio/TalusElephant-Package.git" }
        };
    }
}
