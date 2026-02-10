namespace ContractCreator.UI.Services.Settings
{
    public interface ISettingsService
    {
        int? CurrentFirmId { get; set; }
        string CurrentFirmName { get; set; }
        IObservable<int?> CurrentFirmIdChanged { get; }
        IObservable<string?> CurrentFirmNameChanged { get; }
        bool IsDarkTheme { get; set; }
        void Save();
    }
}
