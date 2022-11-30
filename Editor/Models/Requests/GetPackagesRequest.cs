using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models.Requests
{
    [System.Serializable]
    public class GetPackagesRequest : BaseRequest
    {
        public override string ApiUrl => "packages/get-packages";
        public override string ContentType => "application/json";
    }
}
