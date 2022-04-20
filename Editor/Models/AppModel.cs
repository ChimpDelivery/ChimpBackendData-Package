namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class AppModel
    {
        public string app_bundle;
        public string app_name;
        public string fb_app_id;
        public string elephant_id;
        public string elephant_secret;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}
