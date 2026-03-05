namespace ContractCreator.UI.ViewModels.Contracts.Documents
{
    public class ActEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IContractActService _actService;
        private readonly IContractInvoiceService _invoiceService;
        private readonly IContractService _contractService;
        private readonly ICounterpartyService _counterpartyService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        private int _contractorId;
        private int _customerId;

        public string PageTitle => Id == 0 ? "Создание Акта выполненных работ" : $"Акт № {ActNumber}";

        [Reactive] public int Id { get; set; }
        [Reactive] public int ContractId { get; set; }
        [Reactive] public string ActNumber { get; set; } = string.Empty;
        [Reactive] public DateOnly? ActDate { get; set; }

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

        public ObservableCollection<ContractActItemDto> ActItems { get; } = new();
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> AddItemCommand { get; }
        public ReactiveCommand<ContractActItemDto, Unit> RemoveItemCommand { get; }
        #endregion

        public ActEditorViewModel(
            IContractActService actService,
            IContractInvoiceService invoiceService,
            IContractService contractService,
            ICounterpartyService counterpartyService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _actService = actService;
            _invoiceService = invoiceService;
            _contractService = contractService;
            _counterpartyService = counterpartyService;
            _navigation = navigation;
            _dialogService = dialogService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            AddItemCommand = ReactiveCommand.Create(AddItemToAct);
            RemoveItemCommand = ReactiveCommand.Create<ContractActItemDto>(RemoveItemFromAct);

            SetupReactiveLogic();
        }

        private void SetupReactiveLogic()
        {
            this.WhenAnyValue(x => x.VATRate)
                .Subscribe(_ => RecalculateTotals());

            ActItems.CollectionChanged += (s, e) => RecalculateTotals();

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
            TotalAmount = ActItems.Sum(x => x.TotalAmount);
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
                    ActDate = DateOnly.FromDateTime(DateTime.Today);
                }
                else
                {
                    Id = param.Id;
                    await LoadActAsync();
                }

                this.RaisePropertyChanged(nameof(PageTitle));
                this.RaisePropertyChanged(nameof(ActDate));
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

                _contractorId = contract.FirmSignerId ?? 0;
                _customerId = contract.CounterpartySignerId ?? 0;

                var cp = await _counterpartyService.GetCounterpartyByIdAsync(contract.CounterpartyId);
                if (cp != null) 
                    PurchaserName = cp.FullName ?? cp.ShortName;

                var invoices = await _invoiceService.GetByContractIdAsync(ContractId);
                AvailableInvoices.Clear();
                foreach (var inv in invoices) AvailableInvoices.Add(inv);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки начальных данных (договор ID: {ContractId}) для Акта", ContractId);
            }
        }

        private async Task LoadItemsForSelectedInvoiceAsync(int invoiceId)
        {
            try
            {
                var fullInvoice = await _invoiceService.GetByIdAsync(invoiceId);
                if (fullInvoice == null) return;

                VATRate = fullInvoice.VATRate;
                var allActs = await _actService.GetByContractIdAsync(ContractId);
                var actsForThisInvoice = allActs.Where(a => a.InvoiceId == invoiceId && a.Id != Id).ToList();

                AvailableInvoiceItems.Clear();

                foreach (var invItem in fullInvoice.Items)
                {
                    var alreadyActed = actsForThisInvoice
                        .SelectMany(a => a.Items)
                        .Where(i => i.NomenclatureName == invItem.NomenclatureName)
                        .Sum(i => i.Quantity);

                    var remaining = invItem.Quantity - alreadyActed;

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
                Log.Error(ex, "Ошибка при загрузке позиций выбранного счета ID: {InvoiceId}", invoiceId);
            }
        }

        private async Task LoadActAsync()
        {
            try
            {
                var dto = await _actService.GetByIdAsync(Id);
                if (dto != null)
                {
                    ActNumber = dto.ActNumber;
                    ActDate = dto.ActDate;
                    VATRate = dto.VATRate;
                    CurrencyId = dto.CurrencyId;

                    if (dto.InvoiceId != 0)
                        SelectedInvoice = AvailableInvoices.FirstOrDefault(i => i.Id == dto.InvoiceId);

                    foreach (var item in dto.Items)
                        ActItems.Add(item);

                    RecalculateTotals();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки акта ID: {ActId}", Id);
                await _dialogService.ShowMessageAsync("Не удалось загрузить акт.", "Ошибка", UserMessageType.Error);
            }
        }

        private void AddItemToAct()
        {
            if (SelectedInvoice == null)
                throw new UserMessageException("Сначала выберите счет-основание.", "Внимание", UserMessageType.Warning);

            if (SelectedItemToAdd == null)
                throw new UserMessageException("Выберите позицию.", "Внимание", UserMessageType.Warning);

            if (QuantityToAdd <= 0)
                throw new UserMessageException("Количество должно быть больше нуля.", "Внимание", UserMessageType.Warning);

            if (QuantityToAdd > SelectedItemToAdd.RemainingQuantity)
                throw new UserMessageException($"Нельзя добавить больше остатка по счету.\nДоступно: {SelectedItemToAdd.RemainingQuantity}", "Внимание", UserMessageType.Warning);

            if (ActItems.Any(i => i.NomenclatureName == SelectedItemToAdd.NomenclatureName))
                throw new UserMessageException("Эта позиция уже добавлена.", "Внимание", UserMessageType.Warning);

            var newItem = new ContractActItemDto
            {
                ActId = Id,
                NomenclatureName = SelectedItemToAdd.NomenclatureName,
                UnitOfMeasure = SelectedItemToAdd.UnitOfMeasure,
                Quantity = QuantityToAdd,
                UnitPrice = SelectedItemToAdd.UnitPrice,
                TotalAmount = QuantityToAdd * SelectedItemToAdd.UnitPrice
            };

            ActItems.Add(newItem);
            RecalculateTotals();
            QuantityToAdd = 1;
        }

        private void RemoveItemFromAct(ContractActItemDto item)
        {
            if (item == null) return;

            var itemToRemove = ActItems.FirstOrDefault(i => i.NomenclatureName == item.NomenclatureName);
            if (itemToRemove != null)
            {
                ActItems.Remove(itemToRemove);
                RecalculateTotals();
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(ActNumber))
                throw new UserMessageException("Укажите номер акта.");

            if (SelectedInvoice == null)
                throw new UserMessageException("Выберите счет-основание для акта.");

            if (!ActItems.Any())
                throw new UserMessageException("Добавьте хотя бы одну позицию в акт!");
        }

        private async Task SaveAsync()
        {
            try
            {
                Validate();

                var dto = new ContractActDto
                {
                    Id = Id,
                    ContractId = ContractId,
                    InvoiceId = SelectedInvoice!.Id,
                    ActNumber = ActNumber,
                    ActDate = ActDate ?? DateOnly.FromDateTime(DateTime.Today),
                    TotalAmount = TotalAmount,
                    VATRate = VATRate,
                    VATAmount = VATAmount,
                    AggregateAmount = AggregateAmount,
                    CurrencyId = CurrencyId,
                    ContractorId = _contractorId,
                    CustomerId = _customerId,
                    Items = ActItems.ToList()
                };

                if (Id == 0)
                {
                    await _actService.CreateAsync(dto);
                    Log.Information("Создан новый акт № {ActNumber} по договору ID: {ContractId}", ActNumber, ContractId);
                }
                else 
                    await _actService.UpdateAsync(dto);

                _navigation.NavigateBack();
            }
            catch (UserMessageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка сохранения акта. Редактируемый ID: {ActId}, Номер: {ActNumber}", Id, ActNumber);
                await _dialogService.ShowMessageAsync("Не удалось сохранить акт.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}