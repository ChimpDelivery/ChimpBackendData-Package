using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Interfaces;
using TalusBackendData.Editor.Providers;

namespace TalusBackendData.Editor
{
    /// <summary>
    ///     Platform Data Providers Holder
    /// </summary>
    [FilePath("ProjectSettings/TalusPlatformProvider.asset", FilePathAttribute.Location.ProjectFolder)]
    public class PlatformSettingsHolder : BaseSettingsHolder<PlatformSettingsHolder>
    {
        [SerializeField]
        private List<BaseProvider> _Providers;
        public List<BaseProvider> Providers
        {
            get => _Providers;
            set
            {
                _Providers = value;
                SaveSettings();
            }
        }
    }
}
