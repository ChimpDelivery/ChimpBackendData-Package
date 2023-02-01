using System.IO;

using UnityEngine;
using UnityEngine.Networking;

using UnityEditor;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor.Providers.Asset
{
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
