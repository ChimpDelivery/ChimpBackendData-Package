namespace TalusBackendData.Editor.PackageManager
{
    [System.Serializable]
    internal class PackageStatus
    {
        public bool Exists;
        public string Hash;
        public bool UpdateExists;

        public PackageStatus(bool exists, string hash, bool updateExists)
        {
            Exists = exists;
            Hash = hash;
            UpdateExists = updateExists;
        }
    }
}
