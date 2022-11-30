using System;
using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;

using Unity.EditorCoroutines.Editor;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor
{
    public static class BackendApi
    {
        public static readonly BackendApiConfigs Configs = new();
        
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
            EditorCoroutineUtility.StartCoroutineOwnerless(RequestRoutine(
                request, 
                new DownloadHandlerFile(Configs.TempFile), 
                onSuccess: () => 
                {
                    // response includes custom header that contains original filename
                    string fileName = request.GetHeader(Configs.FileNameKey);
                    string filePath = Path.Combine(Configs.ArtifactFolder, fileName);
                    
                    // if downloaded file is exist just delete
                    if (File.Exists(filePath)) 
                    {
                        File.Delete(filePath);
                    }
                    
                    File.Move(Configs.TempFile, filePath);
                    
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
                Debug.LogError($"[TalusBackendData-Package] Request Error: { www.GetMsg() }");
                yield break;
            }
            
            onSuccess.Invoke();
        }
    }
}
