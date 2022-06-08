using UnityEditor;

namespace TalusBackendData.Editor.Utility
{
    public static class InfoBox
    {
        public static void Show(string title, string message, string ok, string close = null, System.Action okAction = null)
        {
            if (EditorUtility.DisplayDialog(title, message, ok, close))
            {
                okAction?.Invoke();
            }
        }

        public static void ShowBackendParameterError(string parameterName)
        {
            Show(
                "Error :(",
                $"{parameterName} can not be empty!\n\n(Edit/Project Settings/{BackendSettingsHolder.ProviderPath})",
                "Open Settings",
                "Close",
                () => SettingsService.OpenProjectSettings(BackendSettingsHolder.ProviderPath)
            );
        }

        public static void ShowConfirmation(string message, System.Action okAction = null)
        {
            if (EditorUtility.DisplayDialog("Are you sure ?", message, "Yes, I know", "Cancel"))
            {
                okAction?.Invoke();
            }
        }

    }
}
