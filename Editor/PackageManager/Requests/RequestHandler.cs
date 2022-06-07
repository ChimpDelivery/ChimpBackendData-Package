using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace TalusBackendData.Editor.PackageManager.Requests
{
    public class RequestHandler<T> where T : Request
    {
        public T Request { get; set; }
        public System.Action<StatusCode> OnComplete { get; set; }

        public RequestHandler(T request, System.Action<StatusCode> onComplete = null)
        {
            Request = request;
            OnComplete = onComplete;

            EditorApplication.update += Handler;
        }

        private void Handler()
        {
            if (Request != null && !Request.IsCompleted)
            {
                return;
            }

            OnComplete?.Invoke(Request.Status);
            EditorApplication.update -= Handler;
        }
    }
}
