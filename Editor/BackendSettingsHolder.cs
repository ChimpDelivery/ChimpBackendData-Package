using System.IO;

using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor
{
    /// <summary>
    ///     BackendSettingsHolder provides auth information to work with Backend API.
    ///     SO(TalusAuth.asset) instance path (must be ignored by GIT)
    /// </summary>
    [FilePath("ProjectSettings/TalusAuth.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BackendSettingsHolder : BaseSettingsHolder<BackendSettingsHolder>
    {
        // {Web Dashboard} - Api Root URL
        [Header("Backend Credentials")]
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
        private string _ApiToken;
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
        
        //
        public string ArtifactFolder => Directory.GetCurrentDirectory() + "/Builds";
        public string TempFile => ArtifactFolder + "/dashboard-temp-file";
    }
}
