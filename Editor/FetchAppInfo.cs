using System;
using System.Collections;

using TalusBackendData.Editor.Models;

using Unity.EditorCoroutines.Editor;

using UnityEngine;
using UnityEngine.Networking;

namespace TalusBackendData.Editor
{
    public class FetchAppInfo
    {
        private string _ApiKey;
        private string _ApiUrl;
        private string _AppId;

        public FetchAppInfo(string apiKey, string apiUrl, string appId)
        {
            _ApiKey = apiKey;
            _ApiUrl = apiUrl;
            _AppId = appId;
        }

        public IEnumerator GetAppInfo(Action<AppModel> onFetchComplete)
        {
            //string apiKey = CommandLineParser.GetArgument("-apiKey");
            //string url = CommandLineParser.GetArgument("-apiUrl");
            //string appId = CommandLineParser.GetArgument("-appId");

            string apiUrl = $"{_ApiUrl}/api/appstoreconnect/get-app-list/{_AppId}";
            Debug.Log("[Unity-BackendData-Package] apiUrl: " + apiUrl);

            using UnityWebRequest www = UnityWebRequest.Get(apiUrl);
            www.SetRequestHeader("api-key", _ApiKey);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                var appModel = JsonUtility.FromJson<AppModel>(www.downloadHandler.text);

                yield return null;

                Debug.Log("[Unity-BackendData-Package] App bundle: " + appModel.app_bundle);
                onFetchComplete(appModel);
            }
        }
    }
}
