using System.Linq;
using System.Threading;
using System.Collections.Generic;

using UnityEditor;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.Providers.Asset.iOS
{
    public static class Runner
    {
        private static List<BaseProvider> Providers = PlatformSettingsHolder.instance.Providers;

        public static bool IsSatisfy => Providers.Count(t => t.IsCompleted) == Providers.Count;

        // Jenkins execute this function as a stage
        [MenuItem("TalusBackend/Project Settings/iOS")]
        public static void CollectAssets()
        {
            BatchMode.Log("[TalusBackendData-Package] CollectAssets() is running for iOS...");
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                BatchMode.Close(-1);
            }

            Providers.ForEach(provider => provider.Provide());

            while (!IsSatisfy)
            {
                Thread.Sleep(100);
            }

            BatchMode.Close(0);
        }
    }
}