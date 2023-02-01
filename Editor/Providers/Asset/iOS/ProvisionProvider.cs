using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor.Providers.Asset.iOS
{
    [CreateAssetMenu(menuName = "_OTHERS/Data Providers/iOS Provision")]
    public class ProvisionProvider : BaseProvider
    {
        public override void Provide()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.tvOS)
            {
                IsCompleted = true;

                Debug.Log($"[TalusBackendData-Package] Current build target is not iOS! iOSProvision Step skipped...");
                return;
            }

            var request = new ProvisionProfileRequest();

            BackendApi.RequestRoutine(
                request,
                new DownloadHandlerFile(BackendSettingsHolder.instance.TempProvisionProfile),
                onSuccess: () =>
                {
                    string profileUuid = request.Request.GetResponseHeader("Dashboard-Provision-Profile-UUID");
                    Debug.Log($"[TalusBackendData-Package] iOSProvision Step | Provision File exits: {File.Exists(BackendSettingsHolder.instance.TempProvisionProfile)}");
                    Debug.Log($"[TalusBackendData-Package] iOSProvision Step | Provision profile uuid: {profileUuid}");

                    PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
                    PlayerSettings.iOS.iOSManualProvisioningProfileID = profileUuid;

                    GenerateExportOptions(profileUuid);
                }
            );

            IsCompleted = true;
        }

        private void GenerateExportOptions(string profileUuid)
        {
            var fileContents = new List<string>
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">",
                "<plist version=\"1.0\">",
                "<dict>",
                "    <key>compileBitcode</key>",
                "    <false/>",
                "    <key>provisioningProfiles</key>",
                "    <dict>",
               $"        <key>{PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS)}</key>",
               $"        <string>{profileUuid}</string>",
                "    </dict>",
                "    <key>method</key>",
                "    <string>app-store</string>",
                "    <key>signingStyle</key>",
                "    <string>automatic</string>",
                "    <key>stripSwiftSymbols</key>",
                "    <true/>",
                "    <key>uploadSymbols</key>",
                "    <false/>",
                "</dict>",
                "</plist>"
            };

            string exportOptionsPath = Path.Combine(BackendSettingsHolder.instance.ArtifactFolder, "exportOptions.plist");
            File.WriteAllLines(exportOptionsPath, fileContents.ToArray());
            Debug.Log($"[TalusBackendData-Package] exportOptions.plist created at {exportOptionsPath}");
        }
    }
}