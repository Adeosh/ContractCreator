using AvaloniaApp = Avalonia.Application;

namespace ContractCreator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Props
    private readonly INavigationService _navigationService;
    private readonly ISettingsService _settingsService;
    private readonly IFileService _fileService;
    private readonly IFirmService _firmService;
    private readonly IWorkerService _workerService;
    private readonly IUserDialogService _dialogService;

    [Reactive] public ViewModelBase? CurrentPage { get; set; }
    [Reactive] public bool IsDarkTheme { get; set; }
    [Reactive] public string CurrentFirmName { get; set; } = "Фирма не выбрана";
    [Reactive] public string StoragePath { get; set; } = string.Empty;
    [Reactive] public FirmDto? SelectedFirm { get; set; }
    [Reactive] public WorkerDto? SelectedWorker { get; set; }

    public ObservableCollection<FirmDto> AvailableFirms { get; } = new();
    public ObservableCollection<WorkerDto> AvailableWorkers { get; } = new();
    public ObservableCollection<MenuItemViewModel> MenuItems { get; }

    private bool _isSettingsLoaded = false;
    #endregion
    #region Actions
    public ReactiveCommand<Unit, Unit> ChangeStoragePathCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckFilesCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadSettingsDataCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }
    #endregion

    public MainWindowViewModel(
        INavigationService navigationService,
        ISettingsService settingsService,
        IFileService fileService,
        IFirmService firmService,
        IWorkerService workerService,
        IUserDialogService dialogService)
    {
        _settingsService = settingsService;
        _fileService = fileService;
        _firmService = firmService;
        _workerService = workerService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _navigationService.CurrentViewChanged += (viewModel) => CurrentPage = viewModel;

        ChangeStoragePathCommand = ReactiveCommand.CreateFromTask(ChangeStoragePathAsync);
        CheckFilesCommand = ReactiveCommand.CreateFromTask(CheckFilesAsync);
        LoadSettingsDataCommand = ReactiveCommand.CreateFromTask(LoadSettingsDataAsync);
        OpenSettingsCommand = ReactiveCommand.CreateFromTask(OpenSettingsAsync);

        SetupMain();

        IsDarkTheme = _settingsService.IsDarkTheme;
        CurrentFirmName = _settingsService.CurrentFirmName;
        StoragePath = _settingsService.StoragePath;

        MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("Документы")
                {
                    Items = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Договоры/контракты",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<ContractListViewModel>(ContractListMode.All))),
        
                        new MenuItemViewModel("Работа с документами",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<ContractListViewModel>(ContractListMode.Execution))),
        
                        new MenuItemViewModel("Архив",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<ContractListViewModel>(ContractListMode.Archive)))
                    }
                },

                new MenuItemViewModel("Контрагенты")
                {
                    Items = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Список контрагентов",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<CounterpartyListViewModel>())),
                        new MenuItemViewModel("Контакты",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<ContactListViewModel>()))
                    }
                },

                new MenuItemViewModel("Фирмы")
                {
                    Items = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Моя фирма",
                            ReactiveCommand.Create(() =>
                            {
                                var currentId = _settingsService.CurrentFirmId;
                                if (currentId != null)
                                {
                                     var param = new EditorParams { Mode = EditorMode.Edit, Id = currentId.Value };
                                     _navigationService.NavigateTo<FirmEditorViewModel>(param);
                                }
                                else
                                    _navigationService.NavigateTo<FirmListViewModel>();
                            })),

                        new MenuItemViewModel("Список фирм",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<FirmListViewModel>())),

                        new MenuItemViewModel("Сотрудники",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<WorkerListViewModel>()))
                    }
                },

                new MenuItemViewModel("Номенклатура")
                {
                    Items = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Список товаров",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<ProductListViewModel>(ProductType.Good))),
            
                        new MenuItemViewModel("Список услуг",
                            ReactiveCommand.Create(() => _navigationService.NavigateTo<ProductListViewModel>(ProductType.Service)))
                    }
                }
            };
    }

    private void SetupMain()
    {
        this.WhenAnyValue(x => x.IsDarkTheme)
                .Skip(1)
                .Subscribe(isDark => _settingsService.IsDarkTheme = isDark);

        _settingsService.CurrentFirmNameChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(name => CurrentFirmName = name ?? string.Empty);

        _settingsService.StoragePathChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(path => StoragePath = path ?? string.Empty);

        this.WhenAnyValue(x => x.SelectedFirm)
            .Subscribe(firm =>
            {
                if (firm != null)
                {
                    if (_settingsService.CurrentFirmId != firm.Id)
                    {
                        _settingsService.CurrentFirmId = firm.Id;
                        _settingsService.CurrentFirmName = firm.ShortName;

                        Task.Run(async () => await LoadWorkersForFirmAsync(firm.Id)).SafeFireAndForget();
                    }
                }
                else
                {
                    AvailableWorkers.Clear();
                    SelectedWorker = null;
                }
            });

        this.WhenAnyValue(x => x.SelectedWorker)
            .WhereNotNull()
            .Subscribe(worker => _settingsService.CurrentWorkerId = worker.Id);
    }

    public async Task InitializeAsync()
    {
        await LoadSettingsDataAsync();

        if (SelectedFirm != null)
            await LoadWorkersForFirmAsync(SelectedFirm.Id);

        _navigationService.NavigateTo<FirmListViewModel>();
    }

    private async Task OpenSettingsAsync()
    {
        if (_isSettingsLoaded) return;

        await LoadSettingsDataAsync();
        _isSettingsLoaded = true;
    }

    private async Task LoadSettingsDataAsync()
    {
        AvailableFirms.Clear();
        var firms = await _firmService.GetAllFirmsAsync();
        foreach (var f in firms)
            AvailableFirms.Add(f);

        if (_settingsService.CurrentFirmId.HasValue)
            SelectedFirm = AvailableFirms.FirstOrDefault(f => f.Id == _settingsService.CurrentFirmId.Value);
    }

    private async Task LoadWorkersForFirmAsync(int firmId)
    {
        AvailableWorkers.Clear();

        var workers = await _workerService.GetWorkersByFirmIdAsync(firmId);

        foreach (var w in workers)
            AvailableWorkers.Add(w);

        if (_settingsService.CurrentWorkerId.HasValue)
            SelectedWorker = AvailableWorkers.FirstOrDefault(w => w.Id == _settingsService.CurrentWorkerId.Value);
        else
            SelectedWorker = null;
    }

    private async Task ChangeStoragePathAsync()
    {
        var mainWindow = (AvaloniaApp.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;

        var result = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Выберите папку для хранения файлов",
            AllowMultiple = false
        });

        if (result != null && result.Count > 0)
        {
            string newPath = result[0].Path.LocalPath;
            _settingsService.StoragePath = newPath;
            await _dialogService.ShowMessageAsync($"Папка хранилища изменена на:\n{newPath}", "Успешно", UserMessageType.Info);
        }
    }

    private async Task CheckFilesAsync()
    {
        try
        {
            var discrepancies = await _fileService.CheckFilesComparability();

            if (discrepancies.Count == 0)
                await _dialogService.ShowMessageAsync("Ошибок не найдено. База данных и хранилище синхронизированы.", "Сверка файлов", UserMessageType.Info);
            else
            {
                string message = $"Найдено расхождений: {discrepancies.Count}\n\n" + string.Join("\n", discrepancies.Take(10));
                if (discrepancies.Count > 10) message += $"\n...и еще {discrepancies.Count - 10} ошибок.";
                await _dialogService.ShowMessageAsync(message, "Внимание", UserMessageType.Warning);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            await _dialogService.ShowMessageAsync($"Ошибка при проверке файлов!", "Ошибка", UserMessageType.Error);
        }
    }
}
