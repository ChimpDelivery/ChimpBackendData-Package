namespace TalusBackendData.Editor.Models.Interfaces
{
    public abstract class BaseModel
    {
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}
