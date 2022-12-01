using TalusBackendData.Editor.Interfaces;

namespace TalusBackendData.Editor.Models
{
    [System.Serializable]
    public class AppModel : BaseModel
    {
        public string app_name;
        public string app_bundle;
        public string fb_app_id;
        public string fb_client_token;
        public string ga_id;
        public string ga_secret;
    }
}
