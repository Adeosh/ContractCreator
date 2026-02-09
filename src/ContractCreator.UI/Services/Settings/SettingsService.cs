using Avalonia.Styling;
using System.Text.Json;
using AvaloniaApp = Avalonia.Application;

namespace ContractCreator.UI.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private const string FileName = "user_settings.json";
        private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
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
            get => _settings.IsDarkTheme;
            set
            {
                if (_settings.IsDarkTheme != value)
                {
                    _settings.IsDarkTheme = value;

                    // Сначала меняем визуал
                    ApplyTheme(value);

                    // Потом сохраняем
                    Save();
                }
            }
        }

        public SettingsService()
        {
            Load();
            ApplyTheme(_settings.IsDarkTheme);
        }

        private void Load()
        {
            _settings = new AppSettings();

            if (File.Exists(FileName))
            {
                try
                {
                    var json = File.ReadAllText(FileName);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);

                    if (loaded != null)
                        _settings = loaded;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка при загрузке настроек из файла {FileName}. " +
                        "Будут использованы настройки по умолчанию.", FileName);
                }
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, _jsonOptions);
                File.WriteAllText(FileName, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось сохранить настройки в {FileName}", FileName);
            }
        }

        private void ApplyTheme(bool isDark)
        {
            if (AvaloniaApp.Current != null)
            {
                var newTheme = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

                if (AvaloniaApp.Current.RequestedThemeVariant != newTheme)
                {
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        AvaloniaApp.Current.RequestedThemeVariant = newTheme;
                    });
                }
            }
        }

        private class AppSettings
        {
            public int? CurrentFirmId { get; set; }
            public bool IsDarkTheme { get; set; } = false;
        }
    }
}
