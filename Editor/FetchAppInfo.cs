using System;
using System.Collections;

using TalusBackendData.Editor.Models;

using UnityEngine;
using UnityEngine.Networking;

namespace TalusBackendData.Editor
{
    public class FetchAppInfo
    {
        public IEnumerator GetAppInfo(Action<AppModel> onFetchComplete)
        {
            string apiKey = CommandLineParser.GetArgument("-apiKey");
            string url = CommandLineParser.GetArgument("-apiUrl");
            string appId = CommandLineParser.GetArgument("-appId");

            string apiUrl = $"{url}/api/appstoreconnect/get-app-list/{appId}";
            Debug.Log("[Unity-CI-Package] apiUrl: " + apiUrl);

            using UnityWebRequest www = UnityWebRequest.Get(apiUrl);
            www.SetRequestHeader("api-key", apiKey);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                var appModel = JsonUtility.FromJson<AppModel>(www.downloadHandler.text);

                yield return null;

                Debug.Log("[Unity-CI-Package] App bundle: " + appModel.app_bundle);
                onFetchComplete(appModel);
            }
        }
    }
}
