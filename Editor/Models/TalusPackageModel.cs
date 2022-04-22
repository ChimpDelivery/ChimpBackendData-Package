namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class TalusPackageModel
    {
        public string package_url;
        public bool installed;

        public TalusPackageModel(string url, bool status)
        {
            package_url = url;
            installed = status;
        }
    }
}