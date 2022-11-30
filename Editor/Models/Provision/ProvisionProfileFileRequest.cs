using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models.Provision
{
    [System.Serializable]
    public class ProvisionProfileFileRequest : IFileRequest
    {
        public string ApiUrl => "appstoreconnect/get-provision-profile";
    }
}
