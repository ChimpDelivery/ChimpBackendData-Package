using System;
using System.Collections;

using TalusBackendData.Editor.Models;

using UnityEngine;
using UnityEngine.Networking;

using Unity.EditorCoroutines.Editor;

namespace TalusBackendData.Editor
{
    public class FetchAppInfo
    {
        private readonly string _ApiUrl;
        private readonly string _ApiToken;
        private readonly string _AppId;

        public FetchAppInfo(string apiUrl, string apiToken, string appId)
        {
            _ApiUrl = apiUrl;
            _ApiToken = apiToken;
            _AppId = appId;
        }

        public void GetInfo(Action<AppModel> onFetchComplete)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetAppInfo(onFetchComplete));
        }

        public IEnumerator GetAppInfo(Action<AppModel> onFetchComplete)
        {
            string apiUrl = $"{_ApiUrl}/api/appstoreconnect/get-app-list/{_AppId}";
            Debug.Log("[Unity-BackendData-Package] apiUrl: " + apiUrl);

            using UnityWebRequest www = UnityWebRequest.Get(apiUrl);
            www.SetRequestHeader("api-key", _ApiToken);

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
