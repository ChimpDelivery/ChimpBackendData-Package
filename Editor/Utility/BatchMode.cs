using UnityEditor;

using UnityEngine;

namespace ChimpBackendData.Editor.Utility
{
    public static class BatchMode
    {
        public static void Close(int exitCode)
        {
            if (!Application.isBatchMode) { return; }

            EditorApplication.Exit(exitCode);
        }

        public static void Log(string msg)
        {
            if (!Application.isBatchMode) { return; }

            Debug.Log(msg);
        }

        public static void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
