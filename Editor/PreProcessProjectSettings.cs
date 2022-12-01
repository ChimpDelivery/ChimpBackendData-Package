using UnityEngine;

using UnityEditor;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor
{
    public class PreProcessProjectSettings
    {
        public static System.Action<AppModel> OnSyncComplete = delegate { };

        [MenuItem("TalusBackend/Sync Project Settings", priority = 12000)]
        public static void Sync()
        {
            new PreProcessProjectSettings().UpdateSettings();
        }

        [MenuItem("TalusBackend/App Signing/Download Cert")]
        public static void DownloadCert()
        {
            BackendApi.DownloadFile(new CertRequest(), Debug.Log);
        }

        [MenuItem("TalusBackend/App Signing/Download Provision Profile")]
        public static void DownloadProvisionProfile()
        {
            BackendApi.DownloadFile(new ProvisionProfileRequest(), Debug.Log);
        }

        public void UpdateSettings(System.Action onCustomComplete = null)
        {
            Debug.Log("[TalusBackendData-Package] PreProcessProjectSettings::Sync()");

            BackendApi.GetApi<GetAppRequest, AppModel>(
                new GetAppRequest(),
                app =>
                {
                    UpdateProductSettings(app);

                    OnSyncComplete(app);
                    onCustomComplete?.Invoke();
                }
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

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
