using System.Collections.Generic;

using UnityEditor;

using ChimpBackendData.Editor.Interfaces;

namespace ChimpBackendData.Editor
{
    internal class BackendSettingsProvider : BaseSettingsProvider<BackendSettingsHolder>
    {
        public override BackendSettingsHolder Holder => BackendSettingsHolder.instance;
        public override string Description => "You can get the 'Api Token' from 'Dashboard/Profile'";

        public BackendSettingsProvider(string path) : base(path)
        { }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new BackendSettingsProvider("Chimp Delivery/1. Authentication");
        }
    }
}