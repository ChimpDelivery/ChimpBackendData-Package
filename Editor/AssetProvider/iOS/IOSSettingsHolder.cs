using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.AssetProvider.iOS
{
    /// <summary>
    ///     IOS Data Providers Holder
    /// </summary>
    [FilePath("ProjectSettings/TalusIOSProvider.asset", FilePathAttribute.Location.ProjectFolder)]
    public class IOSSettingsHolder : BaseSettingsHolder<IOSSettingsHolder>
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
