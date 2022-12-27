using UnityEngine;

namespace TalusBackendData.Editor.AssetProvider
{
    public abstract class BaseProvider : ScriptableObject
    {
        public bool IsCompleted { get; protected set; }

#if ODIN_INSPECTOR_3
        [Sirenix.OdinInspector.Button]
#endif
        public abstract void Provide();

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }
    }
}