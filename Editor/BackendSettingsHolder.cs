using UnityEditor;
using UnityEngine;

namespace TalusBackendData.Editor
{
    /// <summary>
    ///     BackendSettingsHolder provides auth information to work with Backend API.
    /// </summary>
    [FilePath("ProjectSettings/TalusAuth.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BackendSettingsHolder : ScriptableSingleton<BackendSettingsHolder>
    {
        // SO(TalusAuth.asset) instance path (must be ignored by GIT)
        public string Path => GetFilePath();

        // Unity3D - BackendSettings Panel Path
        private const string _ProviderPath = "Talus Studio/1. Authentication";
        public static string ProviderPath => _ProviderPath;

        // {Web Dashboard} - Api Root URL
        [SerializeField]
        private string _ApiUrl = "http://34.252.141.173";
        public string ApiUrl
        {
            get => _ApiUrl;
            set
            {
                _ApiUrl = value;
                SaveSettings();
            }
        }

        // {Web Dashboard} - Api Token
        [SerializeField]
        private string _ApiToken = default;
        public string ApiToken
        {
            get => _ApiToken;
            set
            {
                _ApiToken = value;
                SaveSettings();
            }
        }

        [SerializeField]
        private string _AppId;
        public string AppId
        {
            get => _AppId;
            set
            {
                _AppId = value;
                SaveSettings();
            }
        }
        
        public void SaveSettings()
        {
            Save(true);
        }

        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }
    }
}