using System.Linq;
using System.Threading;
using System.Collections.Generic;

using TalusBackendData.Editor.Utility;

// ReSharper disable UnusedType.Global

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public static class Runner
    {
        private static readonly List<BaseProvider> _Providers = new()
        {
            new VersionSettingsProvider(),
            new ProductSettingsProvider(),
            new ProvisionProvider()
        };

        public static bool IsSatisfy => _Providers.Count(t => t.IsCompleted) == _Providers.Count;

        // Jenkins execute this function as a stage
        public static void CollectAssets()
        {
            _Providers.ForEach(provider => provider.Provide());

            while (!IsSatisfy)
            {
                Thread.Sleep(100);
            }

            BatchMode.Close(0);
        }
    }
}