using System;
using System.Threading;

using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Models.Interfaces;
using TalusBackendData.Editor.Requests.Interfaces;

namespace TalusBackendData.Editor
{
    public static class BackendApi
    {
        public static void GetApi<TRequest, TModel>(TRequest request, Action<TModel> onFetchComplete)
            where TRequest : BaseRequest
            where TModel : BaseModel
        {
            RequestRoutine(request, new DownloadHandlerBuffer(), onSuccess: () =>
            {
                var model = JsonUtility.FromJson<TModel>(request.Request.downloadHandler.text);
                BatchMode.Log($"[TalusBackendData-Package] Fetched Model: {model}");
                onFetchComplete(model);
            });
        }

        private static void RequestRoutine(BaseRequest request, DownloadHandler downloadHandler, Action onSuccess)
        {
            UnityWebRequest www = request.Get();
            www.downloadHandler = downloadHandler;
            www.timeout = 30;
            www.SendWebRequest();

            BatchMode.Log($"[TalusBackendData-Package] Request URL: {www.url}");

            while (!www.isDone)
            {
                BatchMode.Log($"[TalusBackendData-Package] Request URL: {www.url} | Waiting for response");
                Thread.Sleep(1000);
            }

            BatchMode.Log($"[TalusBackendData-Package] Request Result: {www.result}, Response Code: {www.responseCode}");

            if (request.HasError)
            {
                Debug.LogError($"[TalusBackendData-Package] Request Error: {www.GetMsg()}");
                BatchMode.Close(-1);
            }

            onSuccess.Invoke();
            www.Dispose();
        }
    }
}
