using UnityEditor;

using UnityEngine;

namespace ChimpBackendData.Editor.Interfaces
{
    public abstract class BaseSettingsHolder<T> : ScriptableSingleton<T> where T : ScriptableObject
    {
        public string Path => GetFilePath();

        public void SaveSettings()
        {
            Save(true);
        }
    }
}
