namespace TalusBackendData.Editor.PackageManager.Models
{
    [System.Serializable]
    internal class PackageStatus
    {
        public bool Exist;
        public string DisplayName;
        public string Hash;
        public bool UpdateExist;

        public PackageStatus(bool exist, string displayName, string hash, bool updateExist)
        {
            Exist = exist;
            DisplayName = displayName;
            Hash = hash;
            UpdateExist = updateExist;
        }
    }
}
