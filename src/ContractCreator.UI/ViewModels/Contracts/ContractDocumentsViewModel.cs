namespace ContractCreator.UI.ViewModels.Contracts
{
    public class ContractDocumentsViewModel : ViewModelBase, INavigatedAware
    {
        private readonly IContractService _contractService;
        private readonly IContractInvoiceService _invoiceService;
        private readonly IContractActService _actService;
        private readonly IContractWaybillService _waybillService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public int ContractId { get; set; }
        [Reactive] public ContractDto? ContractInfo { get; set; }

        public ObservableCollection<ContractInvoiceDto> Invoices { get; } = new();
        public ObservableCollection<ContractActDto> Acts { get; } = new();
        public ObservableCollection<ContractWaybillDto> Waybills { get; } = new();

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateInvoiceCommand { get; }

        public ContractDocumentsViewModel(
            IContractService contractService,
            IContractInvoiceService invoiceService,
            IContractActService actService,
            IContractWaybillService waybillService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _contractService = contractService;
            _invoiceService = invoiceService;
            _actService = actService;
            _waybillService = waybillService;
            _navigation = navigation;
            _dialogService = dialogService;

            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
            CreateInvoiceCommand = ReactiveCommand.Create(OpenInvoiceEditor);
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is int contractId)
            {
                ContractId = contractId;
                await LoadWorkspaceAsync();
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadWorkspaceAsync()
        {
            try
            {
                ContractInfo = await _contractService.GetContractByIdAsync(ContractId);

                var invoices = await _invoiceService.GetByContractIdAsync(ContractId);
                var acts = await _actService.GetByContractIdAsync(ContractId);
                var waybills = await _waybillService.GetByContractIdAsync(ContractId);

                Invoices.Clear();
                foreach (var inv in invoices) Invoices.Add(inv);

                Acts.Clear();
                foreach (var act in acts) Acts.Add(act);

                Waybills.Clear();
                foreach (var wb in waybills) Waybills.Add(wb);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки рабочего места по контракту");
                await _dialogService.ShowMessageAsync("Ошибка", "Не удалось загрузить документы.", UserMessageType.Error);
            }
        }

        private void OpenInvoiceEditor()
        {

        }
    }
}
