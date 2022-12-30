using System.Collections.Generic;

using UnityEditor;

using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor
{
    internal class BackendSettingsProvider : BaseSettingsProvider<BackendSettingsHolder>
    {
        public override BackendSettingsHolder Holder => BackendSettingsHolder.instance;

        public override string Title => "Talus Studio/1. Authentication";
        public override string Description => "You can get the 'Api Token' from 'Talus Dashboard/Profile'";

        public BackendSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        { }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new BackendSettingsProvider("Talus Studio/1. Authentication", SettingsScope.Project);
        }
    }
}