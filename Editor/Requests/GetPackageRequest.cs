using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Requests
{
    [System.Serializable]
    public class GetPackageRequest : BaseRequest
    {
        public string PackageId { get; set; }
        
        public override string ApiUrl => $"packages/get-package?package_id={PackageId}";
        public override string ContentType => "application/json";
    }
}
