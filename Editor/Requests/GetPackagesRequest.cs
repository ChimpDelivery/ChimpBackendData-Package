using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Requests
{
    [System.Serializable]
    public class GetPackagesRequest : BaseRequest
    {
        public override string ApiUrl => "packages/get-packages";
    }
}
