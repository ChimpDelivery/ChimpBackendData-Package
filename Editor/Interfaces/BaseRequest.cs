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
        public abstract string ContentType { get; }
        
        public UnityWebRequest Get()
        {
            UnityWebRequest request = UnityWebRequest.Get($"{ServerUrl}/api/{ApiUrl}");
            request.SetRequestHeader("Authorization", $"Bearer {Token}");
            request.SetRequestHeader("Accept", ContentType);
            request.SetRequestHeader("Content-Type", ContentType);

            return request;
        }
    }
}
