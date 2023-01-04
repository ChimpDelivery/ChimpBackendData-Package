using System.IO;

using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Interfaces;

using SysPath = System.IO.Path;

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

        public string ProjectFolder => Directory.GetCurrentDirectory();

        // build artifacts
        public string ArtifactFolderName => "Builds";
        public string ArtifactFolder => SysPath.Join(ProjectFolder, ArtifactFolderName);

        // app-icon
        public string AppIconName => "app-icon.png";
        public string AppIconFullPath => SysPath.Combine(
            SysPath.Join(ProjectFolder, "Assets"),
            AppIconName
        );

        // provision
        public string TempProvisionProfile => ArtifactFolder + "/temp-provision";
    }
}
