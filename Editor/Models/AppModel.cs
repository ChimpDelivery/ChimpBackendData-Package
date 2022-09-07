namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class AppModel
    {
        public string app_bundle;
        public string app_name;
        public string fb_app_id;
        public string fb_client_token;
        public string ga_id;
        public string ga_secret;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}
