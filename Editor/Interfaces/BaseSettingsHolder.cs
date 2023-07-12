using UnityEditor;

using UnityEngine;

namespace ChimpBackendData.Editor.Interfaces
{
    public abstract class BaseSettingsHolder<T> : ScriptableSingleton<T> where T : ScriptableObject
    {
        public string Path => GetFilePath();

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }

        public void SaveSettings()
        {
            Save(true);
        }
    }
}
