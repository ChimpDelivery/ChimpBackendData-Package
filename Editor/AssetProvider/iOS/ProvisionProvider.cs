using System.IO;
using System.Collections.Generic;
using System.Threading;
using System;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Requests;
using TalusBackendData.Editor.Utility;

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

            UnityWebRequest www = UnityWebRequest.Get("http://34.252.141.173/api/appstoreconnect/get-provision-profile");
            www.downloadHandler = new DownloadHandlerFile(_ApiConfigs.TempFile);
            www.SetRequestHeader("Authorization", $"Bearer {Token}");
            www.SetRequestHeader("Accept", "application/octet-stream");
            www.SetRequestHeader("Content-Type", "application/octet-stream");
            www.SendWebRequest();

            while (!www.isDone)
            {
                Console.WriteLine("[TalusBackendData-Package] Waiting for response");
                Thread.Sleep(1000);
            }


            if (www.result == UnityWebRequest.Result.Success)
            {
                Console.WriteLine("[TalusBackendData-Package] Info has been successfully received!");
                Console.WriteLine("file exits: " + File.Exists(_ApiConfigs.TempFile));

                File.Move(
                    _ApiConfigs.TempFile,
                    Path.GetFileName(_ApiConfigs.TempFile).Split(".mobileprovision")[0]
                );
                // File.WriteAllBytes(_ApiConfigs.TempFile, www.downloadHandler.data);
                www.Dispose();
            }

            return;

            var request = new ProvisionProfileRequest();
            BackendApi.DownloadFile(request, onDownloadComplete: path =>
            {
                string fileUuid = request.GetHeader(_ApiConfigs.ProvisionUuidKey);
                string fileName = Path.GetFileName(path).Split(".mobileprovision")[0];
                string teamId = request.GetHeader(_ApiConfigs.TeamIdKey);

                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision profile path: {path}");
                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision profile name: {fileName}");
                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision profile uuid: {fileUuid}");

                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
                PlayerSettings.iOS.iOSManualProvisioningProfileID = fileUuid;

                GenerateExportOptions(fileName, teamId);
            });

            Debug.Log("[TalusCI-Package] iOSProvision Step | Completed!");
        }

        // UnityBuild splits into 2 stage on Jenkins.
        // 1. Stage => Prepares build configurations
        // 2. Stage => Run build
        // So Bundle id already resolved in the previous step on Jenkins
        private void GenerateExportOptions(string profileName, string teamId)
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
                $"        <string>{profileName}</string>",
                "    </dict>",
                "    <key>method</key>",
                "    <string>app-store</string>",
                "    <key>signingCertificate</key>",
                "    <string>iPhone Distribution</string>",
                "    <key>signingStyle</key>",
                "    <string>manual</string>",
                "    <key>stripSwiftSymbols</key>",
                "    <true/>",
                "    <key>teamID</key>",
                $"    <string>{teamId}</string>",
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