using ChimpBackendData.Editor.Interfaces;

namespace ChimpBackendData.Editor.Requests
{
    [System.Serializable]
    public class AppIconRequest : BaseRequest
    {
        public override bool IsPrivateApi => false;

        private readonly string _ApiUrl;
        public override string ApiUrl => _ApiUrl;

        public override string ContentType => "image/png";

        public AppIconRequest(string apiUrl)
        {
            _ApiUrl = apiUrl;
        }
    }
}
