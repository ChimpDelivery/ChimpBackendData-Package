using UnityEngine;

using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Requests.Interfaces;

namespace TalusBackendData.Editor.Requests
{
    [System.Serializable]
    public class GetAppRequest : BaseRequest
    {
        public string AppId => Application.isBatchMode
                ? CommandLineParser.GetArgument("-appId")
                : BackendSettingsHolder.instance.AppId;

        public override string ApiUrl => $"apps/get-app?id={AppId}";
    }
}
