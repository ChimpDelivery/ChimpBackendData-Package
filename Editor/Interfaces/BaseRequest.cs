﻿using UnityEngine;
using UnityEngine.Networking;

using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.Interfaces
{
    public abstract class BaseRequest
    {
        public static string ServerUrl => Application.isBatchMode
            ? CommandLineParser.GetArgument("-apiUrl")
            : BackendSettingsHolder.instance.ApiUrl;
        
        public static string Token => Application.isBatchMode 
            ? CommandLineParser.GetArgument("-apiKey")
            : BackendSettingsHolder.instance.ApiToken;

        public abstract string ApiUrl { get; }
        public virtual string ContentType => "application/json";

        public UnityWebRequest Request { get; private set; }

        public bool HasError => Request.result == UnityWebRequest.Result.ConnectionError || 
                                Request.result == UnityWebRequest.Result.ProtocolError;
        
        public UnityWebRequest Get()
        {
            Request = UnityWebRequest.Get($"{ServerUrl}/api/{ApiUrl}");
            Request.SetRequestHeader("Authorization", $"Bearer {Token}");
            Request.SetRequestHeader("Accept", ContentType);
            Request.SetRequestHeader("Content-Type", ContentType);

            return Request;
        }

        public string GetHeader(string key)
        {
            return Request.GetResponseHeader(key);
        }
    }
}
