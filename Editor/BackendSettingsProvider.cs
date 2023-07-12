using System.Collections.Generic;

using UnityEditor;

using ChimpBackendData.Editor.Interfaces;

namespace ChimpBackendData.Editor
{
    internal class BackendSettingsProvider : BaseSettingsProvider<BackendSettingsHolder>
    {
        public override BackendSettingsHolder Holder => BackendSettingsHolder.instance;

        public override string Title => "Chimp Delivery/1. Authentication";
        public override string Description => "You can get the 'Api Token' from 'Dashboard/Profile'";

        public BackendSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        { }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new BackendSettingsProvider("Chimp Delivery/1. Authentication", SettingsScope.Project);
        }
    }
}