using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor.AssetProvider
{
    public class ProductSettingsProvider : BaseProvider
    {
        [MenuItem("TalusBackend/Project Settings/Sync", priority = -10000)]
        public static void Sync()
        {
            new ProductSettingsProvider().Provide();
        }

        public override void Provide()
        {
            Debug.Log("[TalusBackendData-Package] ProductSettingsProvider running...");

            BackendApi.GetApi<GetAppRequest, AppModel>(
                new GetAppRequest(),
                onFetchComplete: UpdateProductSettings
            );
        }

        private void UpdateProductSettings(AppModel app)
        {
            Debug.Log("[TalusBackendData-Package] Update product settings...");

            if (app != null)
            {
                Debug.Log($"[TalusBackendData-Package] App Model used by ProductSettingsProvider: {app}");

                PlayerSettings.productName = app.app_name;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, app.app_bundle);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, app.app_bundle);
                
                new AppIconUpdater(app);
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