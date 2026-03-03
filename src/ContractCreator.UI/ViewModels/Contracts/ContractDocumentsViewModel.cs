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
        private readonly IDocumentPrintService _printService;

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
        public ReactiveCommand<ContractInvoiceDto, Unit> PrintInvoiceCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateActCommand { get; }
        public ReactiveCommand<ContractActDto, Unit> EditActCommand { get; }
        public ReactiveCommand<ContractActDto, Unit> DeleteActCommand { get; }
        public ReactiveCommand<ContractActDto, Unit> PrintActCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateWaybillCommand { get; }
        public ReactiveCommand<ContractWaybillDto, Unit> EditWaybillCommand { get; }
        public ReactiveCommand<ContractWaybillDto, Unit> DeleteWaybillCommand { get; }
        public ReactiveCommand<ContractWaybillDto, Unit> PrintWaybillCommand { get; }
        #endregion

        public ContractDocumentsViewModel(
            IContractService contractService,
            IContractInvoiceService invoiceService,
            IContractActService actService,
            IContractWaybillService waybillService,
            INavigationService navigation,
            IUserDialogService dialogService,
            IDocumentPrintService printService)
        {
            _contractService = contractService;
            _invoiceService = invoiceService;
            _actService = actService;
            _waybillService = waybillService;
            _navigation = navigation;
            _dialogService = dialogService;
            _printService = printService;

            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            CreateInvoiceCommand = ReactiveCommand.Create(OpenInvoiceEditor);
            EditInvoiceCommand = ReactiveCommand.Create<ContractInvoiceDto>(EditInvoice);
            DeleteInvoiceCommand = ReactiveCommand.CreateFromTask<ContractInvoiceDto>(DeleteInvoiceAsync);
            PrintInvoiceCommand = ReactiveCommand.CreateFromTask<ContractInvoiceDto>(PrintInvoiceAsync);

            CreateActCommand = ReactiveCommand.Create(OpenActEditor);
            EditActCommand = ReactiveCommand.Create<ContractActDto>(EditAct);
            DeleteActCommand = ReactiveCommand.CreateFromTask<ContractActDto>(DeleteActAsync);
            PrintActCommand = ReactiveCommand.CreateFromTask<ContractActDto>(PrintActAsync);

            CreateWaybillCommand = ReactiveCommand.Create(OpenWaybillEditor);
            EditWaybillCommand = ReactiveCommand.Create<ContractWaybillDto>(EditWaybill);
            DeleteWaybillCommand = ReactiveCommand.CreateFromTask<ContractWaybillDto>(DeleteWaybillAsync);
            PrintWaybillCommand = ReactiveCommand.CreateFromTask<ContractWaybillDto>(PrintWaybillAsync);
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

        private async Task PrintInvoiceAsync(ContractInvoiceDto invoice)
        {
            if (invoice == null) return;

            try
            {
                string htmlContent = await _printService.GenerateHtmlAsync(invoice.Id, DocumentType.Invoice);
                string tempPath = Path.Combine(
                    Path.GetTempPath(),
                    $"Счет_{invoice.InvoiceNumber}_{DateTime.Now:yyyy_MM_dd}.html"
                );

                await File.WriteAllTextAsync(tempPath, htmlContent, System.Text.Encoding.UTF8);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при печати счета");
                await _dialogService.ShowMessageAsync($"Ошибка печати: {ex.Message}", "Ошибка", UserMessageType.Error);
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

        private async Task PrintActAsync(ContractActDto act)
        {
            if (act == null) return;

            try
            {
                string htmlContent = await _printService.GenerateHtmlAsync(act.Id, DocumentType.Act);
                string tempPath = Path.Combine(
                    Path.GetTempPath(),
                    $"Счет_{act.ActNumber}_{DateTime.Now:yyyy_MM_dd}.html"
                );

                await File.WriteAllTextAsync(tempPath, htmlContent, System.Text.Encoding.UTF8);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при печати акта");
                await _dialogService.ShowMessageAsync($"Ошибка печати: {ex.Message}", "Ошибка", UserMessageType.Error);
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

        private async Task PrintWaybillAsync(ContractWaybillDto waybill)
        {
            if (waybill == null) return;

            try
            {
                string htmlContent = await _printService.GenerateHtmlAsync(waybill.Id, DocumentType.Waybill);
                string tempPath = Path.Combine(
                    Path.GetTempPath(),
                    $"Счет_{waybill.WaybillNumber}_{DateTime.Now:yyyy_MM_dd}.html"
                );

                await File.WriteAllTextAsync(tempPath, htmlContent, System.Text.Encoding.UTF8);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при печати накладной");
                await _dialogService.ShowMessageAsync($"Ошибка печати: {ex.Message}", "Ошибка", UserMessageType.Error);
            }
        }
    }
}