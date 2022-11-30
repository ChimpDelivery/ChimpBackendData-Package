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
    /// <summary>
    /// - Fetch Dashboard Api
    /// - Download required Certificate & Profile
    /// </summary>
    public static class BackendApi
    {
        private const string FILE_RESPONSE_KEY = "Dashboard-File-Name";
        private const string TEMP_FILE = "Assets/_dashboardTemp/temp-file";

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
                new DownloadHandlerFile(TEMP_FILE), 
                onSuccess: () => 
                {
                    SyncAssets();
                        
                    // request includes custom header that contains downloaded file name
                    string fileName = request.Request.GetResponseHeader(FILE_RESPONSE_KEY);
                    string filePath = $"Assets/Settings/{fileName}";

                    bool isMoved = AssetDatabase.MoveAsset(TEMP_FILE, filePath) == string.Empty;
                    if (!isMoved)
                    {
                        Debug.LogError($"Error: Couldn't moved downloaded file! Check: {TEMP_FILE} (maybe {fileName} exist on {filePath}...)");
                        return;
                    } 
                        
                    onDownloadComplete($"Check: {TEMP_FILE}");
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
                Debug.LogError($"[TalusBackendData-Package] Error: {www.GetMsg()}");
                yield break;
            }
 
            onSuccess.Invoke();
        }
        
        private static void CleanUpTemp()
        {
            // removes temp directory and meta file
            DirectoryInfo dirInfo = Directory.GetParent(TEMP_FILE);
            File.Delete($"Assets/{dirInfo.Name}.meta");
            Directory.Delete(dirInfo.FullName);
            
            SyncAssets();
            
            Debug.Log("[TalusCI-Package] Download cleanup finished!");
        }
        
        private static void SyncAssets()
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
