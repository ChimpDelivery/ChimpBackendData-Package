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
                GetApiResponse(request, onFetchComplete)
            );
        }
        
        public static void DownloadFile(BaseRequest connector, Action<string> onDownloadComplete)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(
                GetDownloadResponse(connector, onDownloadComplete)
            );
        }
        
        private static IEnumerator GetApiResponse<T>(BaseRequest request, Action<T> onFetchComplete)
        {
            using UnityWebRequest www = request.Get();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || 
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[TalusBackendData-Package] Error: {www.GetMsg()}");
            }
            else
            {
                var model = JsonUtility.FromJson<T>(www.downloadHandler.text);

                yield return null;

                if (Application.isBatchMode)
                {
                    Debug.Log($"[TalusBackendData-Package] Fetched AppModel: {model}");
                }

                onFetchComplete(model);
            }
        }

        private static IEnumerator GetDownloadResponse(BaseRequest request, Action<string> onDownloadComplete)
        {
            using UnityWebRequest www = request.Get();
            www.downloadHandler = new DownloadHandlerFile(TEMP_FILE);
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.ConnectionError || 
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[TalusBackendData-Package] Error: {www.GetMsg()}");
            }
            else
            {
                // request includes custom header that contains downloaded file name
                string fileName = www.GetResponseHeader(FILE_RESPONSE_KEY);
                string filePath = $"Assets/Settings/{fileName}";
                
                // file downloaded and moved successfully
                SyncAssets();
                if (AssetDatabase.MoveAsset(TEMP_FILE, filePath) == string.Empty)
                {
                    CleanUpTemp();
                    onDownloadComplete($"Check: {TEMP_FILE}");
                }
                else
                {
                    Debug.LogError($"Error: Couldn't moved downloaded file! Check: {TEMP_FILE} (maybe {fileName} exist on {filePath}...)");
                }
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
