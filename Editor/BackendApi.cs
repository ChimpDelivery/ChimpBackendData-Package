using System;
using System.IO;
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
                new DownloadHandlerBuffer(),
                onSuccess: () =>
                {
                    if (!Directory.Exists(apiConfigs.ArtifactFolder))
                    {
                        Directory.CreateDirectory(apiConfigs.ArtifactFolder);
                    }

                    // response includes custom header that contains original filename
                    string fileName = request.GetHeader(apiConfigs.FileNameKey);
                    string filePath = $"{apiConfigs.ArtifactFolder}/{fileName}";

                    Debug.Log($"[TalusBackendData-Package] Temporary file path: {apiConfigs.TempFile}");
                    Debug.Log($"[TalusBackendData-Package] Real file path: {filePath}");
                    File.WriteAllBytes(filePath, request.Request.downloadHandler.data);

                    onDownloadComplete(filePath);
                }
            ));
        }

        private static IEnumerator RequestRoutine(BaseRequest request, DownloadHandler downloadHandler, Action onSuccess)
        {
            using UnityWebRequest www = request.Get();
            www.downloadHandler = downloadHandler;
            Debug.Log($"[TalusBackendData-Package] Request Url: {www.url}");
            yield return www.SendWebRequest();

            if (request.HasError)
            {
                Debug.LogError($"[TalusBackendData-Package] Request Error: {www.GetMsg()}");
                yield break;
            }

            while (!www.isDone)
            {
                yield return null;
            }

            onSuccess.Invoke();
        }
    }
}
