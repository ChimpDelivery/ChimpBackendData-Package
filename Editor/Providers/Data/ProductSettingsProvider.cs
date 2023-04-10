using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.Providers.Data
{
    [CreateAssetMenu(menuName = "Talus/Build/Data Providers/Product Settings")]
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

    /// <summary>
    /// Download and update project icon
    /// </summary>
    public class AppIconUpdater
    {
        private static BackendSettingsHolder Settings => BackendSettingsHolder.instance;

        public AppIconUpdater(AppModel model)
        {
            Download(model);
        }

        public void Download(AppModel model)
        {
            string iconPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{Settings.AppIconName}");

            Debug.Log(@$"[TalusBackendData-Package] Project icon downloading :
                Source {model.app_icon},
                Destination {iconPath}"
            );

            BackendApi.RequestRoutine(
                new AppIconRequest(model.app_icon),
                new DownloadHandlerFile(Path.Combine(Settings.ProjectFolder, iconPath)),
                onSuccess: () =>
                {
                    BatchMode.SaveAssets();
                    PlayerSettings.SetIconsForTargetGroup(
                        BuildTargetGroup.Unknown,
                        new[] { LoadIcon(iconPath) }
                    );
                    BatchMode.SaveAssets();
                    Debug.Log("[TalusBackendData-Package] Update Project Icon completed!");
                });
        }

        private Texture2D LoadIcon(string iconPath)
        {
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon == null)
            {
                Debug.LogError("[TalusBackendData-Package] App icon is null!");
                return null;
            }

            return icon;
        }
    }
}