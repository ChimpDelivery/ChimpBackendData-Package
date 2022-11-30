using System;
using System.IO;
using System.Collections;

using UnityEditor;
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
                        Debug.Log($"[TalusBackendData-Package] Fetched AppModel: {model}");
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
                    SyncAssets();

                    // request includes custom header that contains downloaded file name
                    string fileName = request.GetHeader(Configs.FileNameKey);
                    string filePath = $"Assets/Settings/{fileName}";

                    bool isMoved = AssetDatabase.MoveAsset(Configs.TempFile, filePath) == string.Empty;
                    if (!isMoved)
                    {
                        Debug.LogError(@$"[TalusBackendData-Package] Error: Couldn't moved downloaded file!
                            Check: {Configs.TempFile} (maybe {fileName} exist on {filePath}...)"
                        );
                        return;
                    } 
                        
                    onDownloadComplete($"Check: {Configs.TempFile}");
                    CleanUpTemp();
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
        
        private static void CleanUpTemp()
        {
            // removes temp directory and meta file
            DirectoryInfo dirInfo = Directory.GetParent(Configs.TempFile);
            File.Delete($"Assets/{dirInfo.Name}.meta");
            Directory.Delete(dirInfo.FullName);
            
            SyncAssets();
            
            Debug.Log($"[TalusCI-Package] Request Temp: {Configs.TempFile} cleaned up!");
        }
        
        private static void SyncAssets()
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
