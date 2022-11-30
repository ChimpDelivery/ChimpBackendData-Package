using System.IO;

namespace TalusBackendData.Editor
{
    [System.Serializable]
    public class BackendApiConfigs
    {
        public string FileNameKey      = "Dashboard-File-Name";
        public string ProvisionUuidKey = "Dashboard-Provision-Profile-UUID";
        public string TeamIdKey        = "Dashboard-Team-ID";

        public string ProvisionFolder => $"{Directory.GetCurrentDirectory()}/Provision";
        public string TempFile => $"{ProvisionFolder}/dashboard-temp-file";
    }
}
