using Avalonia.Styling;
using System.Reactive.Subjects;
using System.Text.Json;
using AvaloniaApp = Avalonia.Application;

namespace ContractCreator.UI.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private const string FileName = "user_settings.json";
        private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
        private AppSettings _settings;
        private readonly BehaviorSubject<int?> _currentFirmIdSubject;
        private readonly BehaviorSubject<string> _currentFirmNameSubject;
        private readonly BehaviorSubject<string> _storagePathSubject;

        public int? CurrentFirmId
        {
            get => _settings.CurrentFirmId;
            set
            {
                _settings.CurrentFirmId = value;
                Save();
                _currentFirmIdSubject.OnNext(value);
            }
        }

        public string CurrentFirmName
        {
            get => _settings.CurrentFirmName ?? "Фирма не выбрана";
            set
            {
                if (_settings.CurrentFirmName != value)
                {
                    _settings.CurrentFirmName = value;
                    Save();
                    _currentFirmNameSubject.OnNext(value ?? "Фирма не выбрана");
                }
            }
        }

        public string StoragePath
        {
            get => _settings.StoragePath ?? string.Empty;
            set
            {
                if (_settings.StoragePath != value)
                {
                    _settings.StoragePath = value;
                    Save();
                    _storagePathSubject.OnNext(value ?? string.Empty);
                }
            }
        }

        public IObservable<int?> CurrentFirmIdChanged => _currentFirmIdSubject;
        public IObservable<string> CurrentFirmNameChanged => _currentFirmNameSubject;
        public IObservable<string> StoragePathChanged => _storagePathSubject;

        public bool IsDarkTheme
        {
            get => _settings.IsDarkTheme;
            set
            {
                if (_settings.IsDarkTheme != value)
                {
                    _settings.IsDarkTheme = value;

                    ApplyTheme(value);
                    Save();
                }
            }
        }

        public SettingsService()
        {
            Load();
            ApplyTheme(_settings!.IsDarkTheme);

            _currentFirmIdSubject = new BehaviorSubject<int?>(_settings.CurrentFirmId);
            _currentFirmNameSubject = new BehaviorSubject<string>(_settings.CurrentFirmName ?? "Фирма не выбрана");
            _storagePathSubject = new BehaviorSubject<string>(_settings.StoragePath ?? string.Empty);
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
            public string? CurrentFirmName { get; set; }
            public bool IsDarkTheme { get; set; } = false;
            public string? StoragePath { get; set; }
        }
    }
}
