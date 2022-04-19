using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Models;

namespace TalusBackendData.Editor
{
    public class FetchAppInfoWindow : EditorWindow
    {
        private string _AppId;

        [MenuItem("TalusKit/Backend/Fetch App Info", false, 1001)]
        private static void Init()
        {
            FetchAppInfoWindow window = GetWindow<FetchAppInfoWindow>();
            window.titleContent = new GUIContent("Fetch App Info");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(4);
            GUILayout.Label("App Settings", EditorStyles.boldLabel);
            _AppId = EditorGUILayout.TextField("App ID:", _AppId);

            if (GUILayout.Button("Fetch"))
            {
                new FetchAppInfo(BackendSettings.ApiUrl, BackendSettings.ApiToken, _AppId).GetInfo(UpdateBackendData);
            }

            GUILayout.EndVertical();
        }

        private void UpdateBackendData(AppModel app)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
            PlayerSettings.productName = app.app_name;

            Debug.Log("Updating product name and bundle id...");
        }
    }
}
