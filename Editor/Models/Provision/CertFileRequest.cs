using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models.Provision
{
    [System.Serializable]
    public class CertFileRequest : IFileRequest
    {
        public string ApiUrl => "appstoreconnect/get-cert";
    }
}
