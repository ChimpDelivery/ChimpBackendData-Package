using UnityEngine;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.Models.Requests
{
    [System.Serializable]
    public class GetAppRequest : BaseRequest
    {
        public string AppId => Application.isBatchMode
                ? CommandLineParser.GetArgument("-appId")
                : BackendSettingsHolder.instance.AppId;

        public override string ApiUrl => "apps/get-app?id=" + AppId;

        public override string ContentType => "application/json";
    }
}
