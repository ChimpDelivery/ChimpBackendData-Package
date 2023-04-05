using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;
using TalusBackendData.Editor.Providers.Asset;

namespace TalusBackendData.Editor.Providers.Data
{
    [CreateAssetMenu(menuName = "Talus/Data Providers/Product Settings")]
    public class ProductSettingsProvider : BaseProvider
    {
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