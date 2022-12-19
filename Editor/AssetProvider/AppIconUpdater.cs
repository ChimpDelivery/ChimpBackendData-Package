using System.IO;
using System.Threading;

using UnityEngine;
using UnityEngine.Networking;

using UnityEditor;

using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.AssetProvider
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

            UnityWebRequest request = UnityWebRequest.Get(model.app_icon);
            request.downloadHandler = new DownloadHandlerFile(Path.Combine(Settings.ProjectFolder, iconPath));
            request.SendWebRequest();

            while (!request.isDone)
            {
                Thread.Sleep(500);
            }
            
            BatchMode.SaveAssets();

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
                Debug.Log("[TalusBackendData-Package] Update Project Icon completed!");
            }
            
            BatchMode.SaveAssets();
            
            request.Dispose();
        }
    }
}
