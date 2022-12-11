using System.IO;
using System.Threading;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public class ProvisionProvider : BaseProvider
    {
        public override void Provide()
        {
            UnityWebRequest www = new ProvisionProfileRequest().Get();
            www.downloadHandler = new DownloadHandlerFile(BackendSettingsHolder.instance.TempProvisionProfile);
            www.SendWebRequest();

            while (!www.isDone)
            {
                Debug.Log("[TalusBackendData-Package] iOSProvision Step | Waiting for response");
                Thread.Sleep(1000);
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string profileUuid = www.GetResponseHeader("Dashboard-Provision-Profile-UUID");
                Debug.Log($"[TalusBackendData-Package] iOSProvision Step | Provision File exits: {File.Exists(BackendSettingsHolder.instance.TempProvisionProfile)}");
                Debug.Log($"[TalusBackendData-Package] iOSProvision Step | Provision profile uuid: {profileUuid}");

                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
                PlayerSettings.iOS.iOSManualProvisioningProfileID = profileUuid;

                GenerateExportOptions(profileUuid);
            }

            www.Dispose();
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

            string exportOptionsPath = $"{BackendSettingsHolder.instance.ArtifactFolder}/exportOptions.plist";
            File.WriteAllLines(exportOptionsPath, fileContents.ToArray());
            Debug.Log($"[TalusBackendData-Package] exportOptions.plist created at {exportOptionsPath}");

            IsCompleted = true;
        }
    }
}