namespace TalusBackendData.Editor.Interfaces
{
    public interface ISettingsProvider
    {
        public bool UnlockPanel { get; set; }
        public System.Action OnSettingsReset { get; }
    }
}
