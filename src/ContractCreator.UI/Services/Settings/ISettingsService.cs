namespace ContractCreator.UI.Services.Settings
{
    public interface ISettingsService
    {
        int? CurrentFirmId { get; set; }
        bool IsDarkTheme { get; set; }
        void Save();
    }
}
