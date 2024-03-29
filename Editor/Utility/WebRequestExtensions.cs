﻿using UnityEngine.Networking;

namespace ChimpBackendData.Editor.Utility
{
    public static class WebRequestExtensions
    {
        public static string GetMsg(this UnityWebRequest request)
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
