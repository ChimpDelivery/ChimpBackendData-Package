using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.AssetProvider
{
    public class VersionSettingsProvider : BaseProvider
    {
        public override void Provide()
        {
            if (!Application.isBatchMode)
            {
                Debug.Log("[TalusBackendData-Package] Version Provider could not run, only CI/CD supported!");
                IsCompleted = true;
                return;
            }


            string appVersion = CommandLineParser.GetArgument("-buildVersion");
            string bundleVersion = CommandLineParser.GetArgument("-bundleVersion");
            Debug.Log($"[TalusCI-Package] App Version: {appVersion}, Bundle Version: {bundleVersion}");

            PlayerSettings.bundleVersion = appVersion;
            PlayerSettings.Android.bundleVersionCode = int.Parse(bundleVersion);
            PlayerSettings.iOS.buildNumber = bundleVersion;

            Debug.Log("[TalusCI-Package] Version settings initialized.");

            IsCompleted = true;
        }
    }
}