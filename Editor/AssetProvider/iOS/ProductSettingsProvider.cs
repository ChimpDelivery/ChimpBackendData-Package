using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public class ProductSettingsProvider : IProvider
    {
        public bool IsCompleted { get; set; }

        public void Provide()
        {
            Debug.Log("[TalusBackendData-Package] ProductSettingsProvider running...");

            BackendApi.GetApi<GetAppRequest, AppModel>(
                new GetAppRequest(),
                app => UpdateProductSettings(app)
            );
        }

        private void UpdateProductSettings(AppModel app)
        {
            Debug.Log("[TalusBackendData-Package] Update product settings...");

            if (app != null)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
                PlayerSettings.productName = app.app_name;

                Debug.Log($"[TalusBackendData-Package] App Model used by ProductSettingsProvider: {app}");
            }
            else
            {
                Debug.LogError("[TalusBackendData-Package] AppModel data is null! Product Settings couldn't updated...");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            IsCompleted = true;
        }
    }
}