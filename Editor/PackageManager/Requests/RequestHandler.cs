using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace TalusBackendData.Editor.PackageManager.Requests
{
    public class RequestHandler<T> where T : Request
    {
        public T Request { get; }
        
        private System.Action<StatusCode> OnComplete { get; }

        public RequestHandler(T request, System.Action<StatusCode> onComplete = null)
        {
            Request = request;
            OnComplete = onComplete;

            EditorApplication.update += Handler;
        }

        private void Handler()
        {
            if (!Request.IsCompleted)
            {
                return;
            }

            OnComplete?.Invoke(Request.Status);
            EditorApplication.update -= Handler;
        }
    }
}
