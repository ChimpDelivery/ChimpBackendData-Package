using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models.Provision
{
    [System.Serializable]
    public class CertFile : BaseFileType
    {
        public override string ApiUrl => "appstoreconnect/get-cert";
    }
}
