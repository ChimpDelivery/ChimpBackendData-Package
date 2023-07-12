namespace ChimpBackendData.Editor.Interfaces
{
    public abstract class BaseModel
    {
        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}
