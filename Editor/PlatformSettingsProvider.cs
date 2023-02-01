using System.Collections.Generic;

using UnityEditor;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor
{
    internal class PlatformSettingsProvider : BaseSettingsProvider<PlatformSettingsHolder>
    {
        public override PlatformSettingsHolder Holder => PlatformSettingsHolder.instance;
        public override string Title => "Talus Studio/3. Platform Providers";
        public override string Description => "Platform specific data providers";

        public PlatformSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        { }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new PlatformSettingsProvider("Talus Studio/3. Platform Providers", SettingsScope.Project);
        }
    }
}