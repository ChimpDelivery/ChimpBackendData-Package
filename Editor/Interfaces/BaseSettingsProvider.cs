using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace ChimpBackendData.Editor.Interfaces
{
    public abstract class BaseSettingsProvider<T> : SettingsProvider where T : BaseSettingsHolder<T>
    {
        public T Holder => BaseSettingsHolder<T>.instance;
        public SerializedObject SerializedObject { get; set; }

        // Panel properties
        public abstract string Description { get; }
        public virtual bool UnlockPanel { get; set; } = true;
        public virtual System.Action OnSettingsReset => delegate { Debug.LogError("Not implemented!"); };

        public BaseSettingsProvider(string path) : base(path, SettingsScope.Project)
        {
            // ScriptableSingleton changes SO hideflags internally
            // We manually set hideflags to make editable in inspector
            if ((Holder.hideFlags & HideFlags.NotEditable) != 0)
            {
                Holder.hideFlags &= ~HideFlags.NotEditable;
                Holder.SaveSettings();
            }

            SerializedObject = new SerializedObject(Holder);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            UnlockPanel = true;
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            ShowInfo();
            ShowHolderProperties();
            GUILayout.FlexibleSpace(); // stick buttons to the bottom
            ShowResetButton();
            ShowLockButton();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                SerializedObject.ApplyModifiedProperties();
                Holder.SaveSettings();
            }
        }

        private void ShowInfo()
        {
            Color defaultColor = GUI.color;
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.HelpBox(Description, MessageType.Info, true);
            GUI.backgroundColor = defaultColor;
        }

        private void ShowHolderProperties()
        {
            GUI.enabled = !UnlockPanel;

            SerializedProperty prop = SerializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    EditorGUILayout.PropertyField(SerializedObject.FindProperty(prop.name), true);
                }
                while (prop.NextVisible(false));
            }
        }

        private void ShowResetButton()
        {
            GUI.enabled = !UnlockPanel;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Reset to defaults", GUILayout.MinHeight(50)))
            {
                OnSettingsReset.Invoke();
            }
        }

        private void ShowLockButton()
        {
            GUI.enabled = true;
            GUI.backgroundColor = Color.yellow;

            string lockButtonName = (UnlockPanel) ? "Unlock Settings" : "Lock Settings";

            if (GUILayout.Button(lockButtonName, GUILayout.MinHeight(50)))
            {
                UnlockPanel = !UnlockPanel;
            }
        }
    }
}
