namespace TalusBackendData.Editor.AssetProvider
{
    public abstract class BaseProvider
    {
        public bool IsCompleted { get; set; }

        public abstract void Provide();
    }
}