using System.Collections;
using System.Collections.Generic;

using Unity.EditorCoroutines.Editor;

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

        public static void CollectAssets()
        {
            providers.ForEach(provider => provider.Provide());

            EditorCoroutineUtility.StartCoroutineOwnerless(
                WaitRoutine(IsSatisfy)
            );
        }

        private static IEnumerator WaitRoutine(bool condition)
        {
            while (true)
            {
                if (condition)
                {
                    yield return null;
                }

                BatchMode.Close(0);
            }
        }
    }
}