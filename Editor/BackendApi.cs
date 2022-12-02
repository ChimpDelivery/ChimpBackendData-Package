using System;
using System.Collections;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using Unity.EditorCoroutines.Editor;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor
{
    public class BackendApi : EditorWindow
    {
        private static EditorWaitForSeconds _WaitForSecond = new(1.0f);

        public static void GetApi<TRequest, TModel>(TRequest request, Action<TModel> onFetchComplete)
            where TRequest : BaseRequest
            where TModel : BaseModel
        {
            RequestRoutine(
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
            );
        }

        private static IEnumerator RequestRoutine(BaseRequest request, DownloadHandler downloadHandler, Action onSuccess)
        {
            UnityWebRequest www = request.Get();
            www.downloadHandler = downloadHandler;
            www.SendWebRequest();

            Debug.Log($"[TalusBackendData-Package] Request URL: {www.url}");

            while (!www.isDone)
            {
                Debug.Log($"[TalusBackendData-Package] Request URL: {www.url} | Waiting for response");
                yield return _WaitForSecond;
            }

            Debug.Log($"[TalusBackendData-Package] Request Result: {www.result}");
            Debug.Log($"[TalusBackendData-Package] Request Response Code: {www.responseCode}");

            if (request.HasError)
            {
                Debug.LogError($"[TalusBackendData-Package] Request Error: {www.GetMsg()}");
                yield break;
            }

            onSuccess.Invoke();
        }
    }
}
