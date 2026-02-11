namespace ContractCreator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Props
    private readonly INavigationService _navigationService;
    private readonly ISettingsService _settingsService;

    [Reactive] public ViewModelBase CurrentPage { get; set; }
    [Reactive] public bool IsDarkTheme { get; set; }
    [Reactive] public string CurrentFirmName { get; set; } = "Фирма не выбрана";

    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    #endregion

    public MainWindowViewModel(
        INavigationService navigationService,
        ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _navigationService = navigationService;
        _navigationService.CurrentViewChanged += (viewModel) => CurrentPage = viewModel;

        SetupMain();

        IsDarkTheme = _settingsService.IsDarkTheme;
        CurrentFirmName = _settingsService.CurrentFirmName;

        MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("Документы")
                {
                    Items = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Договоры/контракты"),
                        new MenuItemViewModel("Счета"),
                        new MenuItemViewModel("Акты"),
                        new MenuItemViewModel("Накладные")
                    }
                },

                new MenuItemViewModel("Контрагенты")
                {
                    Items = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Список контрагентов"),
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
                        new MenuItemViewModel("Список товаров"),
                        new MenuItemViewModel("Список услуг")
                    }
                },
            };

        _navigationService.NavigateTo<FirmListViewModel>();
    }

    private void SetupMain()
    {
        this.WhenAnyValue(x => x.IsDarkTheme)
                .Skip(1)
                .Subscribe(isDark =>
                {
                    _settingsService.IsDarkTheme = isDark;
                });

        _settingsService.CurrentFirmNameChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(name => CurrentFirmName = name ?? string.Empty);
    }
}
