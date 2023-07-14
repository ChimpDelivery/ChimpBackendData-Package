using UnityEditor;

using UnityEngine;

namespace ChimpBackendData.Editor.Interfaces
{
    public abstract class BaseProvider : ScriptableObject
    {
        public bool IsCompleted { get; protected set; }

        public abstract void Provide();
    }

    [CustomEditor(typeof(BaseProvider), editorForChildClasses: true)]
    public class ProviderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BaseProvider provider = target as BaseProvider;
            if (GUILayout.Button("Provide"))
            {
                provider.Provide();
            }
        }
    }
}