namespace ContractCreator.UI.ViewModels.Contracts
{
    public class ContractDocumentsViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
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
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateInvoiceCommand { get; }
        public ReactiveCommand<ContractInvoiceDto, Unit> EditInvoiceCommand { get; }
        public ReactiveCommand<ContractInvoiceDto, Unit> DeleteInvoiceCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateActCommand { get; }
        public ReactiveCommand<ContractActDto, Unit> EditActCommand { get; }
        public ReactiveCommand<ContractActDto, Unit> DeleteActCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateWaybillCommand { get; }
        public ReactiveCommand<ContractWaybillDto, Unit> EditWaybillCommand { get; }
        public ReactiveCommand<ContractWaybillDto, Unit> DeleteWaybillCommand { get; }
        #endregion

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
            EditInvoiceCommand = ReactiveCommand.Create<ContractInvoiceDto>(EditInvoice);
            DeleteInvoiceCommand = ReactiveCommand.CreateFromTask<ContractInvoiceDto>(DeleteInvoiceAsync);

            CreateActCommand = ReactiveCommand.Create(OpenActEditor);
            EditActCommand = ReactiveCommand.Create<ContractActDto>(EditAct);
            DeleteActCommand = ReactiveCommand.CreateFromTask<ContractActDto>(DeleteActAsync);

            CreateWaybillCommand = ReactiveCommand.Create(OpenWaybillEditor);
            EditWaybillCommand = ReactiveCommand.Create<ContractWaybillDto>(EditWaybill);
            DeleteWaybillCommand = ReactiveCommand.CreateFromTask<ContractWaybillDto>(DeleteWaybillAsync);
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is int contractId)
                ContractId = contractId;

            if (ContractId > 0)
                await LoadWorkspaceAsync();
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadWorkspaceAsync()
        {
            try
            {
                ContractInfo = await _contractService.GetContractByIdAsync(ContractId);

                var invoices = await _invoiceService.GetByContractIdAsync(ContractId);
                Invoices.Clear();
                foreach (var inv in invoices) 
                    Invoices.Add(inv);

                var acts = await _actService.GetByContractIdAsync(ContractId);
                Acts.Clear();
                foreach (var act in acts) 
                    Acts.Add(act);

                var waybills = await _waybillService.GetByContractIdAsync(ContractId);
                Waybills.Clear();
                foreach (var waybill in waybills) 
                    Waybills.Add(waybill);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки рабочего места по контракту");
                await _dialogService.ShowMessageAsync("Не удалось загрузить документы.", "Ошибка", UserMessageType.Error);
            }
        }

        private void OpenInvoiceEditor() =>
            _navigation.NavigateTo<InvoiceEditorViewModel>(new EditorParams { Mode = EditorMode.Create, ParentId = ContractId });

        private void EditInvoice(ContractInvoiceDto invoice)
        {
            if (invoice != null)
                _navigation.NavigateTo<InvoiceEditorViewModel>(new EditorParams { Mode = EditorMode.Edit, Id = invoice.Id, ParentId = ContractId });
        }

        private async Task DeleteInvoiceAsync(ContractInvoiceDto invoice)
        {
            if (invoice == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync($"Вы действительно хотите удалить счет № {invoice.InvoiceNumber}?", "Удаление");
            if (!confirm) return;

            try
            {
                await _invoiceService.DeleteAsync(invoice.Id);
                Invoices.Remove(invoice);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении счета");
                await _dialogService.ShowMessageAsync("Не удалось удалить счет.", "Ошибка", UserMessageType.Error);
            }
        }

        private void OpenActEditor() =>
            _navigation.NavigateTo<ActEditorViewModel>(new EditorParams { Mode = EditorMode.Create, ParentId = ContractId });

        private void EditAct(ContractActDto act)
        {
            if (act != null)
                _navigation.NavigateTo<ActEditorViewModel>(new EditorParams { Mode = EditorMode.Edit, Id = act.Id, ParentId = ContractId });
        }

        private async Task DeleteActAsync(ContractActDto act)
        {
            if (act == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync($"Вы действительно хотите удалить акт № {act.ActNumber}?", "Удаление");
            if (!confirm) return;

            try
            {
                await _actService.DeleteAsync(act.Id);
                Acts.Remove(act);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении акта");
                await _dialogService.ShowMessageAsync("Не удалось удалить акт.", "Ошибка", UserMessageType.Error);
            }
        }

        private void OpenWaybillEditor() =>
            _navigation.NavigateTo<WaybillEditorViewModel>(new EditorParams { Mode = EditorMode.Create, ParentId = ContractId });

        private void EditWaybill(ContractWaybillDto waybill)
        {
            if (waybill != null)
                _navigation.NavigateTo<WaybillEditorViewModel>(new EditorParams { Mode = EditorMode.Edit, Id = waybill.Id, ParentId = ContractId });
        }

        private async Task DeleteWaybillAsync(ContractWaybillDto waybill)
        {
            if (waybill == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync($"Вы действительно хотите удалить накладную № {waybill.WaybillNumber}?", "Удаление");
            if (!confirm) return;

            try
            {
                await _waybillService.DeleteAsync(waybill.Id);
                Waybills.Remove(waybill);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении накладной");
                await _dialogService.ShowMessageAsync("Не удалось удалить накладную.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}