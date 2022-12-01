using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

using Unity.EditorCoroutines.Editor;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Utility;

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

        public static void DownloadFile(BaseRequest request, Action<string> onDownloadComplete)
        {
            var apiConfigs = BackendApiConfigs.GetInstance();

            EditorCoroutineUtility.StartCoroutineOwnerless(RequestRoutine(
                request,
                new DownloadHandlerFile(apiConfigs.TempFile),
                onSuccess: () =>
                {
                    // response includes custom header that contains original filename
                    string fileName = request.GetHeader(apiConfigs.FileNameKey);
                    string filePath = $"{apiConfigs.ArtifactFolder}/{fileName}";

                    Debug.Log($"[TalusBackendData-Package] Temporary file path: {apiConfigs.TempFile}");
                    Debug.Log($"[TalusBackendData-Package] Real file path: {filePath}");
                    // File.Move(apiConfigs.TempFile, filePath);

                    onDownloadComplete(filePath);
                }
            ));
        }

        private static IEnumerator RequestRoutine(BaseRequest request, DownloadHandler downloadHandler, Action onSuccess)
        {
            using UnityWebRequest www = request.Get();
            www.downloadHandler = downloadHandler;
            yield return www.SendWebRequest();

            if (request.HasError)
            {
                Debug.LogError($"[TalusBackendData-Package] Request Error: {www.GetMsg()}");
                yield break;
            }

            onSuccess.Invoke();
        }
    }
}
