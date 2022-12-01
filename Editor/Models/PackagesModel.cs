using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class PackagesModel : BaseModel
    {
        public PackageModel[] packages;
    }
}