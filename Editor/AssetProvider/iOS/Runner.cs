using System.Linq;
using System.Threading;
using System.Collections.Generic;

using UnityEditor;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public static class Runner
    {
        private static List<BaseProvider> Providers = IOSSettingsHolder.instance.Providers;

        public static bool IsSatisfy => Providers.Count(t => t.IsCompleted) == Providers.Count;

        // Jenkins execute this function as a stage
        [MenuItem("TalusBackend/Project Settings/iOS")]
        public static void CollectAssets()
        {
            Providers.ForEach(provider => provider.Provide());

            while (!IsSatisfy)
            {
                Thread.Sleep(100);
            }

            BatchMode.Close(0);
        }
    }
}