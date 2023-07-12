using UnityEditor;

using UnityEngine;

namespace ChimpBackendData.Editor.Utility
{
    public static class InfoBox
    {
        public static void ShowBackendParameterError(string parameterName)
        {
            Show(
                "Error :(",
                $"{parameterName} can not be empty!\n\n(Edit/Project Settings/Chimp Delivery/Auth)",
                "Open Settings",
                "Close",
                () => SettingsService.OpenProjectSettings("Chimp Delivery/1. Authentication")
            );
        }

        public static void ShowConfirmation(string message, System.Action okAction = null)
        {
            Show("Are you sure?", message, "Yes, I know", "Cancel", okAction);
        }

        public static void Show(string title, string message, string ok, string close = null, System.Action okAction = null)
        {
            if (Application.isBatchMode) { return; }
            if (EditorUtility.DisplayDialog(title, message, ok, close))
            {
                okAction?.Invoke();
            }
        }
    }
}
