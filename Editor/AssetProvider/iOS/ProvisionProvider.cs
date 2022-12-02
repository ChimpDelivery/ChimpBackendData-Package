using System.IO;
using System.Threading;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Requests;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public class ProvisionProvider : IProvider
    {
        private readonly BackendApiConfigs _ApiConfigs = BackendApiConfigs.GetInstance();

        public bool IsCompleted { get; set; }

        public static string Token => Application.isBatchMode
            ? CommandLineParser.GetArgument("-apiKey")
            : BackendSettingsHolder.instance.ApiToken;

        public void Provide()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.tvOS)
            {
                return;
            }

            UnityWebRequest www = new ProvisionProfileRequest().Get();
            www.downloadHandler = new DownloadHandlerFile(_ApiConfigs.TempFile);
            www.SendWebRequest();

            while (!www.isDone)
            {
                Debug.Log("[TalusBackendData-Package] iOSProvision Step | Waiting for response");
                Thread.Sleep(1000);
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[TalusBackendData-Package] iOSProvision Step | Provision File exits: " + File.Exists(_ApiConfigs.TempFile));

                string newPath = _ApiConfigs.ArtifactFolder
                    + "/"
                    + www.GetResponseHeader(_ApiConfigs.FileNameKey);

                File.Move(_ApiConfigs.TempFile, newPath);

                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision File path: {newPath}");

                string profileUuid = www.GetResponseHeader(_ApiConfigs.ProvisionUuidKey);
                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision profile uuid: {profileUuid}");

                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
                PlayerSettings.iOS.iOSManualProvisioningProfileID = profileUuid;

                GenerateExportOptions(profileUuid);
            }

            www.Dispose();

            return;
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

            string exportOptionsPath = $"{BackendApiConfigs.GetInstance().ArtifactFolder}/exportOptions.plist";
            File.WriteAllLines(exportOptionsPath, fileContents.ToArray());
            Debug.Log($"[TalusCI-Package] exportOptions.plist created at {exportOptionsPath}");

            IsCompleted = true;
        }
    }
}