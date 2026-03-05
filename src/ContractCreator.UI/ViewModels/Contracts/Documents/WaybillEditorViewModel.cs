namespace ContractCreator.UI.ViewModels.Contracts.Documents
{
    public class WaybillEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IContractWaybillService _waybillService;
        private readonly IContractInvoiceService _invoiceService;
        private readonly IContractService _contractService;
        private readonly ICounterpartyService _counterpartyService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        public string PageTitle => Id == 0 ? "Создание Накладной (УПД)" : $"Накладная № {WaybillNumber}";

        [Reactive] public int Id { get; set; }
        [Reactive] public int ContractId { get; set; }
        [Reactive] public string WaybillNumber { get; set; } = string.Empty;
        [Reactive] public DateOnly? WaybillDate { get; set; }

        public ObservableCollection<ContractInvoiceDto> AvailableInvoices { get; } = new();
        [Reactive] public ContractInvoiceDto? SelectedInvoice { get; set; }

        [Reactive] public string PurchaserName { get; set; } = string.Empty;
        [Reactive] public string FirmName { get; set; } = string.Empty;
        [Reactive] public decimal TotalAmount { get; set; }
        [Reactive] public decimal VATRate { get; set; } = 20m;
        [Reactive] public decimal VATAmount { get; set; }
        [Reactive] public decimal AggregateAmount { get; set; }
        [Reactive] public int CurrencyId { get; set; }

        public class AvailableInvoiceItemDisplay
        {
            public string NomenclatureName { get; set; } = string.Empty;
            public string? UnitOfMeasure { get; set; }
            public int RemainingQuantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public ObservableCollection<AvailableInvoiceItemDisplay> AvailableInvoiceItems { get; } = new();
        [Reactive] public AvailableInvoiceItemDisplay? SelectedItemToAdd { get; set; }
        [Reactive] public int QuantityToAdd { get; set; } = 1;

        public ObservableCollection<ContractWaybillItemDto> WaybillItems { get; } = new();
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> AddItemCommand { get; }
        public ReactiveCommand<ContractWaybillItemDto, Unit> RemoveItemCommand { get; }
        #endregion

        public WaybillEditorViewModel(
            IContractWaybillService waybillService,
            IContractInvoiceService invoiceService,
            IContractService contractService,
            ICounterpartyService counterpartyService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _waybillService = waybillService;
            _invoiceService = invoiceService;
            _contractService = contractService;
            _counterpartyService = counterpartyService;
            _navigation = navigation;
            _dialogService = dialogService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            AddItemCommand = ReactiveCommand.Create(AddItemToWaybill);
            RemoveItemCommand = ReactiveCommand.Create<ContractWaybillItemDto>(RemoveItemFromWaybill);

            SetupReactiveLogic();
        }

        private void SetupReactiveLogic()
        {
            this.WhenAnyValue(x => x.VATRate)
                .Subscribe(_ => RecalculateTotals());

            WaybillItems.CollectionChanged += (s, e) => RecalculateTotals();

            this.WhenAnyValue(x => x.SelectedInvoice)
                .Subscribe(inv =>
                {
                    if (inv != null)
                        LoadItemsForSelectedInvoiceAsync(inv.Id).SafeFireAndForget();
                    else
                        AvailableInvoiceItems.Clear();
                });
        }

        private void RecalculateTotals()
        {
            TotalAmount = WaybillItems.Sum(x => x.TotalAmount);
            VATAmount = TotalAmount * (VATRate / 100m);
            AggregateAmount = TotalAmount + VATAmount;

            this.RaisePropertyChanged(nameof(TotalAmount));
            this.RaisePropertyChanged(nameof(VATAmount));
            this.RaisePropertyChanged(nameof(AggregateAmount));
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is EditorParams param)
            {
                ContractId = param.ParentId;

                await LoadInitialDataAsync();

                if (param.Mode == EditorMode.Create)
                {
                    WaybillDate = DateOnly.FromDateTime(DateTime.Today);
                }
                else
                {
                    Id = param.Id;
                    await LoadWaybillAsync();
                }

                this.RaisePropertyChanged(nameof(PageTitle));
                this.RaisePropertyChanged(nameof(WaybillDate));
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadInitialDataAsync()
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(ContractId);
                if (contract == null) return;

                CurrencyId = contract.CurrencyId;

                var cp = await _counterpartyService.GetCounterpartyByIdAsync(contract.CounterpartyId);
                if (cp != null) 
                    PurchaserName = cp.FullName ?? cp.ShortName;

                var invoices = await _invoiceService.GetByContractIdAsync(ContractId);
                AvailableInvoices.Clear();
                foreach (var inv in invoices) AvailableInvoices.Add(inv);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки начальных данных (договор ID: {ContractId}) для Накладной", ContractId);
            }
        }

        private async Task LoadItemsForSelectedInvoiceAsync(int invoiceId)
        {
            try
            {
                var fullInvoice = await _invoiceService.GetByIdAsync(invoiceId);
                if (fullInvoice == null) return;

                VATRate = fullInvoice.VATRate;

                var allWaybills = await _waybillService.GetByContractIdAsync(ContractId);
                var waybillsForThisInvoice = allWaybills.Where(w => w.InvoiceId == invoiceId && w.Id != Id).ToList();

                AvailableInvoiceItems.Clear();

                foreach (var invItem in fullInvoice.Items)
                {
                    var alreadyDelivered = waybillsForThisInvoice
                        .SelectMany(w => w.Items)
                        .Where(i => i.NomenclatureName == invItem.NomenclatureName)
                        .Sum(i => i.Quantity);

                    var remaining = invItem.Quantity - alreadyDelivered;

                    if (remaining > 0)
                    {
                        AvailableInvoiceItems.Add(new AvailableInvoiceItemDisplay
                        {
                            NomenclatureName = invItem.NomenclatureName,
                            UnitOfMeasure = invItem.UnitOfMeasure,
                            RemainingQuantity = remaining,
                            UnitPrice = invItem.UnitPrice
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке позиций счета ID: {InvoiceId} для Накладной", invoiceId);
            }
        }

        private async Task LoadWaybillAsync()
        {
            try
            {
                var dto = await _waybillService.GetByIdAsync(Id);
                if (dto != null)
                {
                    WaybillNumber = dto.WaybillNumber;
                    WaybillDate = dto.WaybillDate;
                    VATRate = dto.VATRate;
                    CurrencyId = dto.CurrencyId;

                    if (dto.InvoiceId != 0)
                        SelectedInvoice = AvailableInvoices.FirstOrDefault(i => i.Id == dto.InvoiceId);

                    foreach (var item in dto.Items)
                        WaybillItems.Add(item);

                    RecalculateTotals();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки накладной ID: {WaybillId}", Id);
                await _dialogService.ShowMessageAsync("Не удалось загрузить накладную.", "Ошибка", UserMessageType.Error);
            }
        }

        private void AddItemToWaybill()
        {
            if (SelectedInvoice == null)
                throw new UserMessageException("Сначала выберите счет-основание.", "Внимание", UserMessageType.Warning);

            if (SelectedItemToAdd == null)
                throw new UserMessageException("Выберите позицию из выбранного счета.", "Внимание", UserMessageType.Warning);

            if (QuantityToAdd <= 0)
                throw new UserMessageException("Количество должно быть больше нуля.", "Внимание", UserMessageType.Warning);

            if (QuantityToAdd > SelectedItemToAdd.RemainingQuantity)
                throw new UserMessageException($"Нельзя отгрузить больше доступного остатка по счету.\nДоступно: {SelectedItemToAdd.RemainingQuantity}", "Внимание", UserMessageType.Warning);

            if (WaybillItems.Any(i => i.NomenclatureName == SelectedItemToAdd.NomenclatureName))
                throw new UserMessageException("Эта позиция уже добавлена в накладную.", "Внимание", UserMessageType.Warning);

            var newItem = new ContractWaybillItemDto
            {
                WaybillId = Id,
                NomenclatureName = SelectedItemToAdd.NomenclatureName,
                UnitOfMeasure = SelectedItemToAdd.UnitOfMeasure,
                Quantity = QuantityToAdd,
                UnitPrice = SelectedItemToAdd.UnitPrice,
                TotalAmount = QuantityToAdd * SelectedItemToAdd.UnitPrice
            };

            WaybillItems.Add(newItem);
            RecalculateTotals();

            QuantityToAdd = 1;
        }

        private void RemoveItemFromWaybill(ContractWaybillItemDto item)
        {
            if (item == null) return;

            var itemToRemove = WaybillItems.FirstOrDefault(i => i.NomenclatureName == item.NomenclatureName);
            if (itemToRemove != null)
            {
                WaybillItems.Remove(itemToRemove);
                RecalculateTotals();
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(WaybillNumber))
                throw new UserMessageException("Укажите номер накладной.");

            if (SelectedInvoice == null)
                throw new UserMessageException("Выберите счет-основание для накладной.");

            if (!WaybillItems.Any())
                throw new UserMessageException("Добавьте хотя бы одну позицию в накладную!");
        }

        private async Task SaveAsync()
        {
            try
            {
                Validate();

                var dto = new ContractWaybillDto
                {
                    Id = Id,
                    ContractId = ContractId,
                    InvoiceId = SelectedInvoice!.Id,
                    WaybillNumber = WaybillNumber,
                    WaybillDate = WaybillDate ?? DateOnly.FromDateTime(DateTime.Today),
                    TotalAmount = TotalAmount,
                    VATRate = VATRate,
                    VATAmount = VATAmount,
                    AggregateAmount = AggregateAmount,
                    CurrencyId = CurrencyId,
                    Items = WaybillItems.ToList()
                };

                if (Id == 0)
                {
                    await _waybillService.CreateAsync(dto);
                    Log.Information("Создана новая накладная № {WaybillNumber} по договору ID: {ContractId}", WaybillNumber, ContractId);
                }
                else
                    await _waybillService.UpdateAsync(dto);

                _navigation.NavigateBack();
            }
            catch (UserMessageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка сохранения накладной. Редактируемый ID: {WaybillId}, Номер: {WaybillNumber}", Id, WaybillNumber);
                await _dialogService.ShowMessageAsync("Не удалось сохранить накладную.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}