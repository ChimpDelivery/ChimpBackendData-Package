using System.IO;
using System.Collections.Generic;
using System.Threading;
using System;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
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

                string newPath = _ApiConfigs.ArtifactFolder
                    + "/"
                    + www.GetResponseHeader(_ApiConfigs.FileNameKey);

                File.Move(
                    _ApiConfigs.TempFile,
                    newPath
                );

                Console.WriteLine("file path:" + newPath);

                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision profile name: {www.GetResponseHeader(_ApiConfigs.FileNameKey)}");
                Debug.Log($"[TalusCI-Package] iOSProvision Step | Provision profile uuid: {www.GetResponseHeader(_ApiConfigs.ProvisionUuidKey)}");

                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
                PlayerSettings.iOS.iOSManualProvisioningProfileID = www.GetResponseHeader(_ApiConfigs.ProvisionUuidKey);

                GenerateExportOptions(
                    www.GetResponseHeader(_ApiConfigs.FileNameKey).Split(".mobileprovision")[0],
                    www.GetResponseHeader(_ApiConfigs.ProvisionUuidKey)
                );

                www.Dispose();
            }

            return;
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