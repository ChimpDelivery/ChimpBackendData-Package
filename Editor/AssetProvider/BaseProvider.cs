namespace TalusBackendData.Editor.AssetProvider
{
    public interface IProvider
    {
        public bool IsCompleted { get; set; }
        public void Provide();
    }
}