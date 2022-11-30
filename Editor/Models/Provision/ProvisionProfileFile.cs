using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models.Provision
{
    [System.Serializable]
    public class ProvisionProfileFile : BaseFileType
    {
        public override string ApiUrl => "appstoreconnect/get-provision-profile";
    }
}
