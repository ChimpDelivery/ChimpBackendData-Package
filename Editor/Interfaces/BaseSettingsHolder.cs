namespace TalusBackendData.Editor.Interfaces
{
using UnityEditor;
using UnityEngine;

namespace TalusBackendData.Editor
{
    public abstract class BaseSettingsHolder<T> : ScriptableSingleton<T> where T : ScriptableObject
    {
        public string Path => GetFilePath();
        
        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }
        
        public virtual void SaveSettings()
        {
            Save(true);
        }
    }
}
}
