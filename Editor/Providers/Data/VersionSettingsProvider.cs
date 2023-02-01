using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.Providers.Data
{
    [CreateAssetMenu(menuName = "_OTHERS/Data Providers/Version Settings")]
    public class VersionSettingsProvider : BaseProvider
    {
        public override void Provide()
        {
            if (!Application.isBatchMode)
            {
                Debug.Log("[TalusBackendData-Package] Version Provider could not run! Only CI/CD pipeline supported!");
                IsCompleted = true;
                return;
            }

            string appVersion = CommandLineParser.GetArgument("-buildVersion");
            string bundleVersion = CommandLineParser.GetArgument("-bundleVersion");
            Debug.Log($"[TalusBackendData-Package] App Version: {appVersion}, Bundle Version: {bundleVersion}");

            PlayerSettings.bundleVersion = appVersion;
            PlayerSettings.Android.bundleVersionCode = int.Parse(bundleVersion);
            PlayerSettings.iOS.buildNumber = bundleVersion;

            Debug.Log("[TalusBackendData-Package] Version settings initialized.");

            IsCompleted = true;
        }
    }
}