namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class PackagesModel
    {
        public PackageModel[] packages;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}