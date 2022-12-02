namespace TalusBackendData.Editor.AssetProvider
{
    public abstract class BaseProvider
    {
        public bool IsCompleted { get; protected set; }

        public abstract void Provide();
    }
}