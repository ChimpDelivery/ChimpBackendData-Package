using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.Interfaces
{
    public abstract class BaseRequest
    {
        public string ServerUrl => Application.isBatchMode
                ? CommandLineParser.GetArgument("-apiUrl")
                : BackendSettingsHolder.instance.ApiUrl;
        
        public string Token => Application.isBatchMode 
                ? CommandLineParser.GetArgument("-apiKey")
                : BackendSettingsHolder.instance.ApiToken;

        public abstract string ApiUrl { get; }
        public virtual string ContentType => "application/json";

        private UnityWebRequest _Request;
        public UnityWebRequest Request => _Request;

        public bool HasError => Request.result == UnityWebRequest.Result.ConnectionError || 
                                Request.result == UnityWebRequest.Result.ProtocolError;
        
        public UnityWebRequest Get()
        {
            _Request = UnityWebRequest.Get($"{ServerUrl}/api/{ApiUrl}");
            _Request.SetRequestHeader("Authorization", $"Bearer {Token}");
            _Request.SetRequestHeader("Accept", ContentType);
            _Request.SetRequestHeader("Content-Type", ContentType);

            return _Request;
        }
    }
}
