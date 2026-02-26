using ContractCreator.UI.ViewModels.Contracts;

namespace ContractCreator.UI;

public partial class App : Avalonia.Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        ServiceProvider = services.BuildServiceProvider();

        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            var dialogService = ServiceProvider.GetRequiredService<IUserDialogService>();

            if (ex is UserMessageException userEx)
            {
                switch (userEx.Type)
                {
                    case UserMessageType.Info:
                        Log.Information($"Информация: {userEx.Message}");
                        break;
                    case UserMessageType.Warning:
                        Log.Warning($"Валидация: {userEx.Message}");
                        break;
                    case UserMessageType.Error:
                        Log.Error($"Логическая ошибка: {userEx.Message}");
                        break;
                }

                dialogService.ShowMessageAsync(userEx.Message, userEx.Title, userEx.Type).SafeFireAndForget();
            }
            else
            {
                Log.Error(ex, "Критическая ошибка системы");
                dialogService.ShowMessageAsync(
                    "Произошла непредвиденная ошибка.\nПодробности записаны в журнал.",
                    "Критическая ошибка",
                    UserMessageType.Error).SafeFireAndForget();
            }
        });

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainVM = ServiceProvider.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainVM
            };

            mainVM.InitializeAsync().SafeFireAndForget();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddApplication();

        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserDialogService, UserDialogService>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<AddressViewModel>();
        services.AddTransient<AttachedFilesViewModel>();
        services.AddTransient<BankAccountsViewModel>();
        services.AddTransient<EconomicActivitiesViewModel>();

        services.AddTransient<ContactListViewModel>();
        services.AddTransient<ContactEditorViewModel>();
        services.AddTransient<CounterpartyListViewModel>();
        services.AddTransient<CounterpartyEditorViewModel>();
        services.AddTransient<WorkerListViewModel>();
        services.AddTransient<WorkerEditorViewModel>();
        services.AddTransient<FirmListViewModel>();
        services.AddTransient<FirmEditorViewModel>();
        services.AddTransient<ProductListViewModel>();
        services.AddTransient<ProductEditorViewModel>();
        services.AddTransient<ContractListViewModel>();
        services.AddTransient<ContractEditorViewModel>();
        services.AddTransient<ContractDocumentsViewModel>();
    }
}