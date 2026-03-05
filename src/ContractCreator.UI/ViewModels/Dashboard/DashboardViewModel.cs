namespace ContractCreator.UI.ViewModels.Dashboard
{
    public class DashboardViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly IContractService _contractService;
        private readonly IContractInvoiceService _invoiceService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public bool IsFirmSelected { get; set; }
        [Reactive] public string FirmName { get; set; } = "Фирма не выбрана";
        [Reactive] public string FirmInnKpp { get; set; } = string.Empty;
        [Reactive] public int TotalActiveContracts { get; set; }
        [Reactive] public decimal TotalActiveContractsValue { get; set; }
        [Reactive] public int ExpiringContractsCount { get; set; }
        [Reactive] public decimal TotalPendingInvoicesValue { get; set; }

        public ObservableCollection<ContractDto> RecentContracts { get; } = new();
        public ObservableCollection<ChartDataItem> ContractsByStageChart { get; } = new();
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToContractsCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateContractCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeFirmCommand { get; }
        #endregion

        public DashboardViewModel(
            IFirmService firmService,
            IContractService contractService,
            IContractInvoiceService invoiceService,
            ISettingsService settingsService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _firmService = firmService;
            _contractService = contractService;
            _invoiceService = invoiceService;
            _settingsService = settingsService;
            _navigation = navigation;
            _dialogService = dialogService;

            RefreshCommand = ReactiveCommand.CreateFromTask(LoadDashboardDataAsync);

            GoToContractsCommand = ReactiveCommand.Create(() =>
                _navigation.NavigateTo<ContractListViewModel>(ContractListMode.All));

            CreateContractCommand = ReactiveCommand.Create(CreateNewContract);

            ChangeFirmCommand = ReactiveCommand.Create(() =>
                _navigation.NavigateTo<FirmListViewModel>());
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            await LoadDashboardDataAsync();
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadDashboardDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var currentFirmId = _settingsService.CurrentFirmId;

                if (currentFirmId == null || currentFirmId == 0)
                {
                    IsFirmSelected = false;
                    FirmName = "Рабочая фирма не выбрана";
                    FirmInnKpp = "Перейдите в настройки или список фирм, чтобы начать работу.";
                    ClearData();
                    return;
                }

                IsFirmSelected = true;

                var firm = await _firmService.GetFirmByIdAsync(currentFirmId.Value);
                if (firm != null)
                {
                    FirmName = firm.ShortName ?? firm.FullName;
                    FirmInnKpp = string.IsNullOrWhiteSpace(firm.KPP)
                        ? $"ИНН: {firm.INN}"
                        : $"ИНН: {firm.INN} / КПП: {firm.KPP}";
                }

                var contracts = await _contractService.GetContractsByFirmIdAsync(currentFirmId.Value);
                var activeContracts = contracts.Where(c =>
                    c.StageTypeId == (int)ContractStageType.Execution ||
                    c.StageTypeId == (int)ContractStageType.Concluded).ToList();

                TotalActiveContracts = activeContracts.Count;
                TotalActiveContractsValue = activeContracts.Sum(c => c.ContractPrice);

                var thirtyDaysFromNow = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
                ExpiringContractsCount = activeContracts.Count(c =>
                    c.EndDate.HasValue &&
                    c.EndDate.Value >= DateOnly.FromDateTime(DateTime.Today) &&
                    c.EndDate.Value <= thirtyDaysFromNow);

                RecentContracts.Clear();
                var recent = contracts.OrderByDescending(c => c.Id).Take(5);
                foreach (var contract in recent)
                    RecentContracts.Add(contract);

                ContractsByStageChart.Clear();
                var groupedByStage = contracts
                    .GroupBy(c => c.StageTypeId)
                    .Select(g => new ChartDataItem
                    {
                        Label = GetStageNameById(g.Key),
                        Value = g.Count()
                    });

                foreach (var item in groupedByStage)
                    ContractsByStageChart.Add(item);

                var invoices = await _invoiceService.GetByContractIdAsync(currentFirmId.Value);
                if (invoices != null)
                {
                    TotalPendingInvoicesValue = invoices
                        .Where(i => i.InvoiceDate.Year == DateTime.Today.Year)
                        .Sum(i => i.TotalAmount ?? 0m);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке данных дашборда.");
                await _dialogService.ShowMessageAsync("Не удалось загрузить данные стартовой страницы.", "Ошибка", UserMessageType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CreateNewContract()
        {
            if (!IsFirmSelected)
            {
                throw new UserMessageException("Сначала выберите рабочую фирму.", "Внимание", UserMessageType.Warning);
            }

            var param = new EditorParams { Mode = EditorMode.Create, ParentId = _settingsService.CurrentFirmId!.Value };
            _navigation.NavigateTo<ContractEditorViewModel>(param);
        }

        private void ClearData()
        {
            TotalActiveContracts = 0;
            TotalActiveContractsValue = 0;
            ExpiringContractsCount = 0;
            TotalPendingInvoicesValue = 0;
            RecentContracts.Clear();
            ContractsByStageChart.Clear();
        }

        private string GetStageNameById(int stageId)
        {
            return stageId switch
            {
                (int)ContractStageType.Draft => "Черновик",
                (int)ContractStageType.Agreement => "Согласование",
                (int)ContractStageType.ApplicationSubmission => "Подача заявок",
                (int)ContractStageType.Tender => "Торги",
                (int)ContractStageType.TenderLost => "Торги проиграны",
                (int)ContractStageType.Conclusion => "Заключение",
                (int)ContractStageType.Concluded => "Заключен",
                (int)ContractStageType.Execution => "Исполнение",
                (int)ContractStageType.Executed => "Исполнен",
                (int)ContractStageType.Paid => "Оплачен",
                (int)ContractStageType.Termination => "Расторжение",
                (int)ContractStageType.Terminated => "Расторгнут",
                _ => "Неизвестно"
            };
        }
    }

    public class ChartDataItem
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}