using Avalonia.Styling;
using System.Text.Json;
using AvaloniaApp = Avalonia.Application;

namespace ContractCreator.UI.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private const string FileName = "user_settings.json";
        private AppSettings _settings;

        public int? CurrentFirmId
        {
            get => _settings.CurrentFirmId;
            set
            {
                _settings.CurrentFirmId = value;
                Save();
            }
        }

        public bool IsDarkTheme
        {
            get => AvaloniaApp.Current!.RequestedThemeVariant == ThemeVariant.Dark;
            set
            {
                if (AvaloniaApp.Current != null)
                {
                    var newTheme = value ? ThemeVariant.Dark : ThemeVariant.Light;

                    if (AvaloniaApp.Current.RequestedThemeVariant != newTheme)
                    {
                        AvaloniaApp.Current.RequestedThemeVariant = newTheme;

                        _settings.IsDarkTheme = value;
                        Save();
                    }
                }
            }
        }

        public SettingsService()
        {
            Load();

            if (AvaloniaApp.Current != null)
            {
                AvaloniaApp.Current.RequestedThemeVariant = _settings!.IsDarkTheme 
                    ? ThemeVariant.Dark 
                    : ThemeVariant.Light;
            }
        }

        private void Load()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    var json = File.ReadAllText(FileName);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                    return;
                }
                catch { /* игнор ошибок чтения */ }
            }
            _settings = new AppSettings();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(_settings);
            File.WriteAllText(FileName, json);
        }

        private class AppSettings
        {
            public int? CurrentFirmId { get; set; }
            public bool IsDarkTheme { get; set; }
        }
    }
}
