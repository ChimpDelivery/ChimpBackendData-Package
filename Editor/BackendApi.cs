using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

using Unity.EditorCoroutines.Editor;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Utility;
using System.Threading;

namespace TalusBackendData.Editor
{
    public static class BackendApi
    {
        public static void GetApi<TRequest, TModel>(TRequest request, Action<TModel> onFetchComplete)
            where TRequest : BaseRequest
            where TModel : BaseModel
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(RequestRoutine(
                request,
                new DownloadHandlerBuffer(),
                onSuccess: () =>
                {
                    var model = JsonUtility.FromJson<TModel>(request.Request.downloadHandler.text);
                    if (Application.isBatchMode)
                    {
                        Debug.Log($"[TalusBackendData-Package] Fetched Model: {model}");
                    }
                    onFetchComplete(model);
                }
            ));
        }

        private static IEnumerator RequestRoutine(BaseRequest request, DownloadHandler downloadHandler, Action onSuccess)
        {
            UnityWebRequest www = request.Get();
            www.downloadHandler = downloadHandler;
            Debug.Log($"[TalusBackendData-Package] Request Url: {www.url}");
            www.SendWebRequest();

            while (!www.isDone)
            {
                Debug.Log($"[TalusBackendData-Package] Request Url: {www.url} | Waiting for response");
                Thread.Sleep(1000);
            }

            Debug.Log($"[TalusBackendData-Package] Request result: {www.result}");
            Debug.Log($"[TalusBackendData-Package] Request responce code: {www.responseCode}");
            Debug.Log($"[TalusBackendData-Package] Request downloaded bytes: {www.downloadedBytes}");

            if (request.HasError)
            {
                Debug.LogError($"[TalusBackendData-Package] Request Error: {www.GetMsg()}");
                yield break;
            }

            onSuccess.Invoke();
        }
    }
}
