using UnityEngine.Networking;

namespace TalusBackendData.Editor
{
    public static class WebRequestResolver
    {
        public static (UnityWebRequest, string) Get(
            string url, 
            string token, 
            string requestType = "application/json"
        )
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Accept", requestType);
            request.SetRequestHeader("Content-Type", requestType);

            return (request, request.GetMsg());
        }
        
        private static string GetMsg(this UnityWebRequest request)
        {
            return request.responseCode switch
            {
                503 => "Web Dashboard is under maintenance!",
                401 => "Unauthorized! Check auth token...",
                _ => request.error
            };
        }
    }
}
