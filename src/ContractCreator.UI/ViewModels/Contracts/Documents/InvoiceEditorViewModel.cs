namespace ContractCreator.UI.ViewModels.Contracts.Documents
{
    public class InvoiceEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IContractInvoiceService _invoiceService;
        private readonly IContractService _contractService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICounterpartyService _counterpartyService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        public string PageTitle => Id == 0 ? "Выставление счета" : $"Счет № {InvoiceNumber}";

        [Reactive] public int Id { get; set; }
        [Reactive] public int ContractId { get; set; }
        [Reactive] public string InvoiceNumber { get; set; } = string.Empty;
        [Reactive] public DateOnly? InvoiceDate { get; set; }
        [Reactive] public string PurchaserINN { get; set; } = string.Empty;
        [Reactive] public string PurchaserKPP { get; set; } = string.Empty;
        [Reactive] public string PurchaserName { get; set; } = string.Empty;
        [Reactive] public string FirmName { get; set; } = string.Empty;
        [Reactive] public string FirmINN { get; set; } = string.Empty;
        [Reactive] public string FirmKPP { get; set; } = string.Empty;
        [Reactive] public decimal TotalAmount { get; set; }
        [Reactive] public decimal VATRate { get; set; } = 20m;
        [Reactive] public decimal VATAmount { get; set; }
        [Reactive] public decimal AggregateAmount { get; set; }
        [Reactive] public int CurrencyId { get; set; }

        public ObservableCollection<BankAccountDto> BankAccounts { get; } = new();
        [Reactive] public BankAccountDto? SelectedBankAccount { get; set; }

        public ObservableCollection<ContractSpecificationDto> AvailableSpecifications { get; } = new();
        [Reactive] public ContractSpecificationDto? SelectedSpecificationToAdd { get; set; }
        [Reactive] public int QuantityToAdd { get; set; } = 1;

        public ObservableCollection<ContractInvoiceItemDto> InvoiceItems { get; } = new();
        [Reactive] public ContractInvoiceItemDto? SelectedInvoiceItem { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> AddItemCommand { get; }
        public ReactiveCommand<ContractInvoiceItemDto, Unit> RemoveItemCommand { get; }
        #endregion

        public InvoiceEditorViewModel(
            IContractInvoiceService invoiceService,
            IContractService contractService,
            IBankAccountService bankAccountService,
            ICounterpartyService counterpartyService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _invoiceService = invoiceService;
            _contractService = contractService;
            _bankAccountService = bankAccountService;
            _counterpartyService = counterpartyService;
            _navigation = navigation;
            _dialogService = dialogService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            AddItemCommand = ReactiveCommand.Create(AddItemToInvoice);
            RemoveItemCommand = ReactiveCommand.Create<ContractInvoiceItemDto>(RemoveItemFromInvoice);

            SetupReactiveLogic();
        }

        private void SetupReactiveLogic()
        {
            this.WhenAnyValue(x => x.VATRate)
                .Subscribe(_ => RecalculateTotals());

            InvoiceItems.CollectionChanged += (s, e) => RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            TotalAmount = InvoiceItems.Sum(x => x.TotalAmount);
            VATAmount = TotalAmount * (VATRate / 100m);
            AggregateAmount = TotalAmount + VATAmount;
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is EditorParams param)
            {
                ContractId = param.ParentId;

                await LoadInitialDataAsync();

                if (param.Mode == EditorMode.Create)
                {
                    InvoiceDate = DateOnly.FromDateTime(DateTime.Today);
                    SelectedBankAccount = BankAccounts.FirstOrDefault();
                }
                else
                {
                    Id = param.Id;
                    await LoadInvoiceAsync();
                }

                this.RaisePropertyChanged(nameof(PageTitle));
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
                {
                    PurchaserName = cp.FullName ?? cp.ShortName;
                    PurchaserINN = cp.INN ?? string.Empty;
                    PurchaserKPP = cp.KPP ?? string.Empty;
                }

                var accounts = await _bankAccountService.GetByFirmIdAsync(contract.FirmId);

                BankAccounts.Clear();
                foreach (var acc in accounts) 
                    BankAccounts.Add(acc);

                AvailableSpecifications.Clear();
                foreach (var spec in contract.Specifications)
                {
                    AvailableSpecifications.Add(spec);

                    if (VATRate == 0 && spec.VATRate > 0)
                        VATRate = spec.VATRate;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки данных контракта");
            }
        }

        private async Task LoadInvoiceAsync()
        {
            try
            {
                var dto = await _invoiceService.GetByIdAsync(Id);
                if (dto != null)
                {
                    InvoiceNumber = dto.InvoiceNumber;
                    InvoiceDate = dto.InvoiceDate;
                    PurchaserINN = dto.PurchaserINN;
                    PurchaserKPP = dto.PurchaserKPP;
                    VATRate = dto.VATRate;
                    CurrencyId = dto.CurrencyId;
                    SelectedBankAccount = BankAccounts.FirstOrDefault(b => b.Id == dto.BankAccountId);

                    foreach (var item in dto.Items)
                        InvoiceItems.Add(item);

                    RecalculateTotals();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки счета");
                await _dialogService.ShowMessageAsync("Не удалось загрузить счет.", "Ошибка", UserMessageType.Error);
            }
        }

        private void AddItemToInvoice()
        {
            if (SelectedSpecificationToAdd == null)
            {
                _dialogService.ShowMessageAsync("Выберите позицию из спецификации.", "Внимание", UserMessageType.Warning);
                return;
            }

            if (QuantityToAdd <= 0)
            {
                _dialogService.ShowMessageAsync("Количество должно быть больше нуля.", "Внимание", UserMessageType.Warning);
                return;
            }

            if (InvoiceItems.Any(i => i.NomenclatureName == SelectedSpecificationToAdd.NomenclatureName))
            {
                _dialogService.ShowMessageAsync("Эта позиция уже добавлена в счет.", "Внимание", UserMessageType.Warning);
                return;
            }

            var newItem = new ContractInvoiceItemDto
            {
                InvoiceId = Id,
                NomenclatureName = SelectedSpecificationToAdd.NomenclatureName,
                UnitOfMeasure = SelectedSpecificationToAdd.UnitOfMeasure,
                Quantity = QuantityToAdd,
                UnitPrice = SelectedSpecificationToAdd.UnitPrice,
                TotalAmount = QuantityToAdd * SelectedSpecificationToAdd.UnitPrice,
                CurrencyId = CurrencyId
            };

            InvoiceItems.Add(newItem);
            RecalculateTotals();
        }

        private void RemoveItemFromInvoice(ContractInvoiceItemDto item)
        {
            if (item != null)
            {
                InvoiceItems.Remove(item);
                RecalculateTotals();
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(InvoiceNumber))
                throw new UserMessageException("Укажите номер счета.");

            if (SelectedBankAccount == null)
                throw new UserMessageException("Выберите расчетный счет.");

            if (!InvoiceItems.Any())
                throw new UserMessageException("Добавьте хотя бы одну позицию в счет!");
        }

        private async Task SaveAsync()
        {
            try
            {
                Validate();

                var dto = new ContractInvoiceDto
                {
                    Id = Id,
                    ContractId = ContractId,
                    BankAccountId = SelectedBankAccount!.Id,
                    InvoiceNumber = InvoiceNumber,
                    InvoiceDate = InvoiceDate ?? DateOnly.FromDateTime(DateTime.Today),
                    PurchaserINN = PurchaserINN,
                    PurchaserKPP = PurchaserKPP,
                    TotalAmount = TotalAmount,
                    VATRate = VATRate,
                    VATAmount = VATAmount,
                    AggregateAmount = AggregateAmount,
                    CurrencyId = CurrencyId,
                    Items = InvoiceItems.ToList() // Передаем элементы в сервис для сохранения
                };

                if (Id == 0)
                    await _invoiceService.CreateAsync(dto);
                else
                    await _invoiceService.UpdateAsync(dto);

                _navigation.NavigateBack();
            }
            catch (UserMessageException ex)
            {
                await _dialogService.ShowMessageAsync(ex.Message, "Внимание", UserMessageType.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка сохранения");
                await _dialogService.ShowMessageAsync("Не удалось сохранить счет.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
