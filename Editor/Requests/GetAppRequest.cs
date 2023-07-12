using UnityEngine;

using ChimpBackendData.Editor.Utility;
using ChimpBackendData.Editor.Interfaces;

namespace ChimpBackendData.Editor.Requests
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
