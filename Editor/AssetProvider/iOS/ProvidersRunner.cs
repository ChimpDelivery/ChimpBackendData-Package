using System.Collections;
using System.Collections.Generic;

using Unity.EditorCoroutines.Editor;

using UnityEditor;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    public static class ProvidersRunner
    {
        private static List<IProvider> providers = new()
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
                WaitRoutine(IsSatisfy, () => EditorApplication.Exit(0))
            );
        }

        private static IEnumerator WaitRoutine(bool condition, System.Action onAfter)
        {
            while (!condition)
            {
                yield return null;
            }

            onAfter.Invoke();
        }
    }
}