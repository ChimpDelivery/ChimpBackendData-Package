using UnityEditor;
using UnityEngine;

using TalusBackendData.Models;

namespace TalusBackendData
{
    public class FetchAppInfoWindow : EditorWindow
    {
        private const string API_URL = "http://dashboard.talusstudio.eu.ngrok.io";

        private string _AppId;
        private string _ApiToken;
        private string _FetchButton = "Fetch";

        [MenuItem("TalusKit/Backend/Fetch App Info")]
        private static void Init()
        {
            FetchAppInfoWindow window = (FetchAppInfoWindow) GetWindow(typeof(FetchAppInfoWindow));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            _AppId = EditorGUILayout.TextField("App ID", _AppId);
            _ApiToken = EditorGUILayout.TextField("Api Key", _ApiToken);

            if (GUILayout.Button(_FetchButton))
            {
                new FetchAppInfo(API_URL, _ApiToken, _AppId).GetInfo(UpdateBackendData);
            }
        }

        private void UpdateBackendData(AppModel app)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
            PlayerSettings.productName = app.app_name;

            Debug.Log("Updating product name and bundle id...");
        }
    }
}
