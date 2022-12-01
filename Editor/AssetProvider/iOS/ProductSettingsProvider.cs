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
            Debug.Log("[TalusBackendData-Package] PreProcessProjectSettings::Sync()");

            BackendApi.GetApi<GetAppRequest, AppModel>(
                new GetAppRequest(),
                app => UpdateProductSettings(app)
            );
        }

        private void UpdateProductSettings(AppModel app)
        {
            Debug.Log("[TalusBackendData-Package] update product settings...");

            if (app != null)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, app.app_bundle);

                PlayerSettings.productName = app.app_name;

                Debug.Log($"[TalusBackendData-Package] App Model used by Pre Process: {app}");
            }
            else
            {
                Debug.LogError("[TalusBackendData-Package] AppModel data is null! Product Settings couldn't updated...");
            }

            IsCompleted = true;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}