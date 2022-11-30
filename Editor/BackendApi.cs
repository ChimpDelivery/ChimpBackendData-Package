using System;
using System.IO;
using System.Collections;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using Unity.EditorCoroutines.Editor;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Models;

namespace TalusBackendData.Editor
{
    /// <summary>
    /// - Fetch iOS App information
    /// - Download required Certificate & Profile
    /// - Fetch Unity3D Package information
    /// </summary>
    public class BackendApi
    {
        private const string FILE_RESPONSE_KEY = "Dashboard-File-Name";
        private const string TEMP_FILE = "Assets/_dashboardTemp/temp-file";
        
        private readonly string _ApiUrl;
        private readonly string _ApiToken;

        public BackendApi(string apiUrl, string apiToken)
        {
            _ApiUrl = apiUrl;
            _ApiToken = apiToken;
        }

        public void GetAppInfo(string appId, Action<AppModel> onFetchComplete)
        {
            string apiUrl = $"{_ApiUrl}/api/apps/get-app?id={appId}";

            EditorCoroutineUtility.StartCoroutineOwnerless(GetApiResponse(apiUrl, onFetchComplete));
        }

        public void GetPackageInfo(string packageId, Action<PackageModel> onFetchComplete)
        {
            string apiUrl = $"{_ApiUrl}/api/packages/get-package?package_id={packageId}";

            EditorCoroutineUtility.StartCoroutineOwnerless(GetApiResponse(apiUrl, onFetchComplete));
        }

        public void GetAllPackages(Action<PackagesModel> onFetchComplete)
        {
            string apiUrl = $"{_ApiUrl}/api/packages/get-packages";

            EditorCoroutineUtility.StartCoroutineOwnerless(GetApiResponse(apiUrl, onFetchComplete));
        }

        public void DownloadFile(IFileRequest connector, Action<string> onDownloadComplete)
        {
            string apiUrl = $"{_ApiUrl}/api/{connector.ApiUrl}";

            EditorCoroutineUtility.StartCoroutineOwnerless(GetDownloadResponse(apiUrl, onDownloadComplete));
        }
        
        private IEnumerator GetApiResponse<T>(string url, Action<T> onFetchComplete)
        {
            using UnityWebRequest www = GetRequest(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[TalusBackendData-Package] Error: {GetResponseMessage(www)}");
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

        private IEnumerator GetDownloadResponse(string url, Action<string> onDownloadComplete)
        {
            using UnityWebRequest www = GetRequest(url);
            www.downloadHandler = new DownloadHandlerFile(TEMP_FILE);
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[TalusBackendData-Package] Error: {GetResponseMessage(www)}");
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

        private UnityWebRequest GetRequest(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer {_ApiToken}");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

        private static void CleanUpTemp()
        {
            DirectoryInfo dirInfo = Directory.GetParent(TEMP_FILE);
            File.Delete($"Assets/{dirInfo.Name}.meta");
            Directory.Delete(dirInfo.FullName);
            
            SyncAssets();
            
            Debug.Log("[TalusCI-Package] Download cleanup finished!");
        }

        private static string GetResponseMessage(UnityWebRequest request)
        {
            return request.responseCode switch
            {
                503 => "Web Dashboard is under maintenance!",
                401 => "Unauthorized! Check auth token...",
                _ => request.error
            };
        }

        private static void SyncAssets()
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
