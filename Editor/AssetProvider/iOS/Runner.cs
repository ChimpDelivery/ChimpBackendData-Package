using System.Threading;
using System.Collections.Generic;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public static class Runner
    {
        private static readonly List<BaseProvider> providers = new()
        {
            new VersionSettingsProvider(),
            new ProductSettingsProvider(),
            new ProvisionProvider(),
        };

        public static bool IsSatisfy
        {
            get
            {
                int satisfied = 0;

                for (int i = 0; i < providers.Count; ++i)
                {
                    if (providers[i].IsCompleted)
                    {
                        ++satisfied;
                    }
                }

                return satisfied == providers.Count;
            }
        }

        // Jenkins execute this function as a stage
        public static void CollectAssets()
        {
            providers.ForEach(provider => provider.Provide());

            while (!IsSatisfy)
            {
                Thread.Sleep(100);
            }

            BatchMode.Close(0);
        }
    }
}