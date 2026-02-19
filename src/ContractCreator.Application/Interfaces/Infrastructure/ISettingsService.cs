namespace ContractCreator.Application.Interfaces.Infrastructure
{
    public interface ISettingsService
    {
        int? CurrentFirmId { get; set; }
        string CurrentFirmName { get; set; }
        string StoragePath { get; set; }
        IObservable<int?> CurrentFirmIdChanged { get; }
        IObservable<string?> CurrentFirmNameChanged { get; }
        IObservable<string> StoragePathChanged { get; }
        bool IsDarkTheme { get; set; }
        void Save();
    }
}
