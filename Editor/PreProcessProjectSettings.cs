using UnityEngine;

using UnityEditor;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Models.Provision;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor
{
    public class PreProcessProjectSettings
    {
        public static System.Action<AppModel> OnSyncComplete = delegate { };

        private static string ApiUrl => (Application.isBatchMode) 
                ? CommandLineParser.GetArgument("-apiUrl") 
                : BackendSettingsHolder.instance.ApiUrl;
        
        private static string ApiToken => (Application.isBatchMode) 
                ? CommandLineParser.GetArgument("-apiKey") 
                : BackendSettingsHolder.instance.ApiToken;
        
        private static string AppId => (Application.isBatchMode) 
                ? CommandLineParser.GetArgument("-appId") 
                : BackendSettingsHolder.instance.AppId;

        [MenuItem("TalusBackend/Sync Project Settings", priority = 12000)]
        public static void Sync()
        {
            new PreProcessProjectSettings().UpdateSettings();
        }

        [MenuItem("TalusBackend/App Signing/Download Cert")]
        public static void DownloadCert()
        {
            var api = new BackendApi(ApiUrl, ApiToken);
            api.DownloadFile(new CertFile(), Debug.Log);
        }

        [MenuItem("TalusBackend/App Signing/Download Provision Profile")]
        public static void DownloadProvisionProfile()
        {
            var api = new BackendApi(ApiUrl, ApiToken);
            api.DownloadFile(new ProvisionProfileFile(), Debug.Log);  
        }

        public void UpdateSettings(System.Action onCustomComplete = null)
        {
            Debug.Log("[TalusBackendData-Package] PreProcessProjectSettings::Sync()");

            var api = new BackendApi(ApiUrl, ApiToken);
            api.GetAppInfo(AppId, (app) =>
            {
                UpdateProductSettings(app);
                
                OnSyncComplete(app);
                onCustomComplete?.Invoke();
            });
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
