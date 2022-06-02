namespace TalusBackendData.Editor.PackageManager
{
    [System.Serializable]
    internal class PackageStatus
    {
        public bool Exist;
        public string Hash;
        public bool UpdateExist;

        public PackageStatus(bool exist, string hash, bool updateExist)
        {
            Exist = exist;
            Hash = hash;
            UpdateExist = updateExist;
        }
    }
}
