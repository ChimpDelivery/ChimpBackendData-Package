using System.IO;

namespace TalusBackendData.Editor
{
    [System.Serializable]
    public class BackendApiConfigs
    {
        public string FileNameKey      = "Dashboard-File-Name";
        public string ProvisionUuidKey = "Dashboard-Provision-Profile-UUID";
        public string TeamIdKey        = "Dashboard-Team-ID";

        public string ArtifactFolder => Path.Combine(Directory.GetCurrentDirectory(), "Builds/");
        public string TempFile => $"{ArtifactFolder}/dashboard-temp-file";

        public static BackendApiConfigs GetInstance()
        {
            return new BackendApiConfigs();
        }
    }
}
