using ChimpBackendData.Editor.Interfaces;

namespace ChimpBackendData.Editor.Requests
{
    [System.Serializable]
    public class CertRequest : BaseRequest
    {
        public override string ApiUrl => "appstoreconnect/get-cert";
        public override string ContentType => "application/x-pkcs12";
    }
}
