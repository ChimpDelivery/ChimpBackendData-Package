namespace TalusBackendData.Editor
{
    [System.Serializable]
    public class TalusPackage
    {
        public string PackageUrl;
        public bool Installed;

        public TalusPackage(string url, bool status)
        {
            PackageUrl = url;
            Installed = status;
        }
    }
}