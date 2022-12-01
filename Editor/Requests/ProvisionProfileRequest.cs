using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Requests
{
    [System.Serializable]
    public class ProvisionProfileRequest : BaseRequest
    {
        public override string ApiUrl => "appstoreconnect/get-provision-profile";
        public override string ContentType => "application/octet-stream";
    }
}
