using UnityEditor;
using UnityEngine;

namespace TalusBackendData.Editor
{
    /// <summary>
    ///     FilePath must be ignored by GIT!
    ///     BackendSettingsHolder provides information for other Packages.
    /// </summary>
    [FilePath("ProjectSettings/TalusSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BackendSettingsHolder : ScriptableSingleton<BackendSettingsHolder>
    {
        // TalusSettings.asset path
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
    }
}