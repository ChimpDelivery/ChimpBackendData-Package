using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class PackageModel : BaseModel
    {
        public string package_id;
        public string url;
        public string hash;
    }
}