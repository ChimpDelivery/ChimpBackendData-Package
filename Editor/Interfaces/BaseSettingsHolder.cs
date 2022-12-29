using UnityEditor;

using UnityEngine;

namespace TalusBackendData.Editor.Interfaces
{
    public abstract class BaseSettingsHolder<T> : ScriptableSingleton<T> where T : ScriptableObject
    {
        public string Path => GetFilePath();

        private void Awake()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }

        public void SaveSettings()
        {
            Save(true);
        }
    }
}
