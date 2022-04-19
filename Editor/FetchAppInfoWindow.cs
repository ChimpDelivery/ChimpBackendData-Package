using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Models;

namespace TalusBackendData.Editor
{
    public class FetchAppInfoWindow : EditorWindow
    {
        [MenuItem("TalusKit/Backend/Fetch App Info", false, 10001)]
        private static void Init()
        {
            FetchAppInfoWindow window = GetWindow<FetchAppInfoWindow>();
            window.titleContent = new GUIContent("Fetch App Info");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(8);

            if (GUILayout.Button("Fetch"))
            {
                if (string.IsNullOrEmpty(BackendSettings.ApiUrl))
                {
                    Debug.LogError("ApiUrl can not be empty! (Edit/Preferences/Talus/Backend Settings) ");
                }
                else if (string.IsNullOrEmpty(BackendSettings.ApiToken))
                {
                    Debug.LogError("ApiToken can not be empty! (Edit/Preferences/Talus/Backend Settings)");
                }
                else if (string.IsNullOrEmpty(BackendSettings.AppId))
                {
                    Debug.LogError("AppId can not be empty! (Edit/Preferences/Talus/Backend Settings)");
                }
                else
                {
                    Debug.Log("App info fetching...");
                    new FetchAppInfo(BackendSettings.ApiUrl, BackendSettings.ApiToken, BackendSettings.AppId).GetInfo(UpdateBackendData);
                }
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
