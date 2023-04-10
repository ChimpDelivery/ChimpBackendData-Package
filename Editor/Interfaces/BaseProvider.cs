using UnityEngine;

namespace TalusBackendData.Editor.Interfaces
{
    public abstract class BaseProvider : ScriptableObject
    {
        public bool IsCompleted { get; protected set; }

#if ODIN_INSPECTOR_3
        [Sirenix.OdinInspector.Button]
#endif
        public abstract void Provide();

        private void Awake()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }
    }
}