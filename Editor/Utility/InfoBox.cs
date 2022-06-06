using UnityEditor;

namespace TalusBackendData.Editor.Utility
{
    public static class InfoBox
    {
        public static void Create(string title, string message, string ok, string close = null, System.Action okAction = null)
        {
            if (EditorUtility.DisplayDialog(title, message, ok, close))
            {
                okAction?.Invoke();
            }
        }
    }
}
