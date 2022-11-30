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

        public static void GetApi<TRequest, TModel>(
            TRequest request, 
            Action<TModel> onFetchComplete
        ) where TRequest : BaseRequest where TModel : BaseModel
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(
                RequestRoutine(
                    request, 
                    new DownloadHandlerBuffer(), 
                    () => {
                        
                        var model = JsonUtility.FromJson<TModel>(request.Request.downloadHandler.text);
                        
                        if (Application.isBatchMode)
                        {
                            Debug.Log($"[TalusBackendData-Package] Fetched AppModel: {model}");
                        }

                        onFetchComplete(model);
                    }
                )
            );
        }
        
        public static void DownloadFile(BaseRequest request, Action<string> onDownloadComplete)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(
                RequestRoutine(
                    request, 
                    new DownloadHandlerFile(TEMP_FILE), 
                    () => {
                        
                        SyncAssets();
                        
                        // request includes custom header that contains downloaded file name
                        string fileName = request.Request.GetResponseHeader(FILE_RESPONSE_KEY);
                        string filePath = $"Assets/Settings/{fileName}";

                        bool isMoved = AssetDatabase.MoveAsset(TEMP_FILE, filePath) == string.Empty;
                        if (isMoved)
                        {
                            CleanUpTemp();
                            onDownloadComplete($"Check: {TEMP_FILE}");

                            return;
                        }
   
                        Debug.LogError($"Error: Couldn't moved downloaded file! Check: {TEMP_FILE} (maybe {fileName} exist on {filePath}...)");
                    }
                )
            );
        }
        
        private static IEnumerator RequestRoutine(
            BaseRequest request,
            DownloadHandler downloadHandler,
            Action onSuccess 
        )
        {
            using UnityWebRequest www = request.Get();
            www.downloadHandler = downloadHandler;
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[TalusBackendData-Package] Error: {www.GetMsg()}");
            }
            else
            {
                onSuccess.Invoke();
            }
        }
        
        private static void CleanUpTemp()
        {
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
