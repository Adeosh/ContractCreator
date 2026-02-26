namespace ContractCreator.UI.ViewModels.Contracts
{
    public class ContractEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IContractService _contractService;
        private readonly ICounterpartyService _counterpartyService;
        private readonly IWorkerService _workerService;
        private readonly IClassifierService _classifierService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly IProductService _productService;

        public string PageTitle => Id == 0 ? "Создание документа" : $"Документ № {ContractNumber}";

        public AttachedFilesViewModel AttachedFilesVM { get; }

        #region Contract
        [Reactive] public int Id { get; set; }
        [Reactive] public int FirmId { get; set; }
        [Reactive] public ContractType SelectedType { get; set; } = ContractType.Contract;
        [Reactive] public ContractEnterpriseRole SelectedRole { get; set; } = ContractEnterpriseRole.Contractor;
        [Reactive] public ContractStageDto? SelectedStage { get; set; }
        [Reactive] public string ContractNumber { get; set; } = string.Empty;
        [Reactive] public decimal ContractPrice { get; set; }
        [Reactive] public string? ContractSubject { get; set; }
        [Reactive] public DateOnly? SubmissionDate { get; set; }
        [Reactive] public string? SubmissionCode { get; set; }
        [Reactive] public string? SubmissionLink { get; set; }
        [Reactive] public DateOnly? TenderDate { get; set; }
        [Reactive] public DateOnly? IssueDate { get; set; }
        [Reactive] public DateOnly? StartDate { get; set; }
        [Reactive] public DateOnly? EndDate { get; set; }
        [Reactive] public DateOnly? ExecutionDate { get; set; }
        [Reactive] public string? TerminationReason { get; set; }
        [Reactive] public TerminationInitiator? SelectedInitiator { get; set; }

        [Reactive] public bool IsSubmission { get; private set; }
        [Reactive] public bool IsTermination { get; private set; }
        [Reactive] public bool IsTenderStage { get; private set; }
        [Reactive] public bool IsConclusionStage { get; private set; }
        [Reactive] public bool IsCustomerRole { get; private set; }

        public ContractType[] ContractTypes => Enum.GetValues<ContractType>();
        public ContractEnterpriseRole[] EnterpriseRoles => Enum.GetValues<ContractEnterpriseRole>();
        public TerminationInitiator[] Initiators => Enum.GetValues<TerminationInitiator>();

        public ObservableCollection<CounterpartyDto> Counterparties { get; } = new();
        [Reactive] public CounterpartyDto? SelectedCounterparty { get; set; }

        public ObservableCollection<WorkerDto> Workers { get; } = new();
        [Reactive] public WorkerDto? SelectedFirmSigner { get; set; }

        public ObservableCollection<ContactDto> CounterpartyContacts { get; } = new();
        [Reactive] public ContactDto? SelectedCounterpartySigner { get; set; }

        public ObservableCollection<ClassifierDto> Currencies { get; } = new();
        [Reactive] public ClassifierDto? SelectedCurrency { get; set; }

        public ObservableCollection<ContractStageDto> AllStages { get; } = new();
        public ObservableCollection<ContractStageDto> AvailableStages { get; } = new();
        public ObservableCollection<ContractSpecificationDto> Specifications { get; } = new();
        #endregion
        #region Products & Steps
        [Reactive] public bool IsProductSelectorOpen { get; set; }
        [Reactive] public string ProductSearchText { get; set; } = string.Empty;

        public ObservableCollection<GoodsAndServiceDto> AllProducts { get; } = new();
        public ObservableCollection<GoodsAndServiceDto> FilteredProducts { get; } = new();

        [Reactive] public GoodsAndServiceDto? SelectedProductToAdd { get; set; }
        [Reactive] public int QuantityToAdd { get; set; } = 1;
        [Reactive] public decimal VatRateToAdd { get; set; } = 20m;

        public ObservableCollection<ContractStepDto> Steps { get; } = new();

        [Reactive] public bool IsStepEditorOpen { get; set; }
        [Reactive] public ContractStepDto? CurrentEditingStep { get; set; }
        [Reactive] public decimal CurrentStepTotalAmount { get; set; }

        public ObservableCollection<StepItemWrapper> StepEditorItems { get; } = new();
        #endregion
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> OpenSelectorCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseSelectorCommand { get; }
        public ReactiveCommand<Unit, Unit> AddSpecificationCommand { get; }
        public ReactiveCommand<ContractSpecificationDto, Unit> DeleteSpecificationCommand { get; }

        public ReactiveCommand<Unit, Unit> OpenNewStepCommand { get; }
        public ReactiveCommand<ContractStepDto, Unit> EditStepCommand { get; }
        public ReactiveCommand<ContractStepDto, Unit> DeleteStepCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveStepCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseStepCommand { get; }
        #endregion

        public ContractEditorViewModel(
            IContractService contractService,
            ICounterpartyService counterpartyService,
            IWorkerService workerService,
            IClassifierService classifierService,
            INavigationService navigation,
            IUserDialogService dialogService,
            ISettingsService settingsService,
            IProductService productService,
            AttachedFilesViewModel attachedFilesVM)
        {
            _contractService = contractService;
            _counterpartyService = counterpartyService;
            _workerService = workerService;
            _classifierService = classifierService;
            _navigation = navigation;
            _dialogService = dialogService;
            _settingsService = settingsService;
            _productService = productService;

            AttachedFilesVM = attachedFilesVM;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            OpenSelectorCommand = ReactiveCommand.CreateFromTask(OpenProductSelectorAsync);
            CloseSelectorCommand = ReactiveCommand.Create(() => { IsProductSelectorOpen = false; });
            AddSpecificationCommand = ReactiveCommand.Create(AddSelectedProductToSpecification);
            DeleteSpecificationCommand = ReactiveCommand.Create<ContractSpecificationDto>(DeleteSpecificationItem);

            OpenNewStepCommand = ReactiveCommand.Create(() => OpenStepEditor(null));
            EditStepCommand = ReactiveCommand.Create<ContractStepDto>(step => OpenStepEditor(step));
            DeleteStepCommand = ReactiveCommand.Create<ContractStepDto>(DeleteStep);
            SaveStepCommand = ReactiveCommand.Create(SaveStep);
            CloseStepCommand = ReactiveCommand.Create(() => { IsStepEditorOpen = false; });

            SetupReactiveLogic();
        }

        private void SetupReactiveLogic()
        {
            this.WhenAnyValue(x => x.SelectedCounterparty)
                .Subscribe(async cp => await LoadCounterpartyContactsAsync(cp?.Id));

            this.WhenAnyValue(x => x.SelectedType, x => x.SelectedRole)
                .Subscribe(_ => FilterAvailableStages());

            this.WhenAnyValue(x => x.SelectedStage)
                .Subscribe(stage =>
                {
                    if (stage == null) return;

                    int stageId = stage.Id;

                    IsSubmission = stageId == (int)ContractStageType.ApplicationSubmission;

                    IsTermination = stageId == (int)ContractStageType.Termination ||
                                    stageId == (int)ContractStageType.Terminated;

                    IsTenderStage = stageId == (int)ContractStageType.Tender;

                    IsConclusionStage = stageId == (int)ContractStageType.Conclusion ||
                                        stageId == (int)ContractStageType.Concluded ||
                                        stageId == (int)ContractStageType.Execution ||
                                        stageId == (int)ContractStageType.Executed ||
                                        stageId == (int)ContractStageType.Paid ||
                                        stageId == (int)ContractStageType.Termination ||
                                        stageId == (int)ContractStageType.Terminated;
                });

            this.WhenAnyValue(x => x.SelectedType)
                .Subscribe(type => AttachedFilesVM.CurrentFileType =
                    type == ContractType.Contract ? FileType.Contract : FileType.Agreement);

            this.WhenAnyValue(x => x.SelectedRole)
                .Subscribe(role => IsCustomerRole = role == ContractEnterpriseRole.Customer);

            this.WhenAnyValue(x => x.ProductSearchText)
                .Subscribe(FilterProducts);

            Specifications.CollectionChanged += (_, _) => CalculateTotalAmount();
        }

        private void FilterAvailableStages()
        {
            if (!AllStages.Any()) return;

            var excludedStages = new List<int>();

            //if (SelectedRole == ContractEnterpriseRole.Customer)
            //{
            //    excludedStages.AddRange(new[]
            //    {
            //        (int)ContractStageType.ApplicationSubmission,
            //        (int)ContractStageType.Tender,
            //        (int)ContractStageType.TenderLost
            //    });
            //}

            var filteredStages = AllStages
                .Where(s => s.TypeIds != null &&
                            s.TypeIds.Contains((int)SelectedType) &&
                            !excludedStages.Contains(s.Id))
                .OrderBy(s => s.Id)
                .ToList();

            AvailableStages.Clear();
            foreach (var stage in filteredStages) 
                AvailableStages.Add(stage);

            if (SelectedStage != null && !AvailableStages.Any(x => x.Id == SelectedStage.Id))
                SelectedStage = AvailableStages.FirstOrDefault();
        }

        private void CalculateTotalAmount()
        {
            if (!Specifications.Any())
            {
                ContractPrice = 0;
                return;
            }

            ContractPrice = Specifications.Sum(x => (x.Quantity * x.UnitPrice) + x.VATPrice);
        }

        private void OpenStepEditor(ContractStepDto? existingStep)
        {
            if (!Specifications.Any())
            {
                _dialogService.ShowMessageAsync("Внимание", "Сначала добавьте позиции во вкладке 'Спецификация'.", UserMessageType.Warning);
                return;
            }

            CurrentEditingStep = existingStep ?? new ContractStepDto
            {
                StepName = $"Этап {Steps.Count + 1}",
                StartStepDate = StartDate ?? DateOnly.FromDateTime(DateTime.Today),
                EndStepDate = EndDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(14)),
                Items = new List<ContractStepItemDto>()
            };

            StepEditorItems.Clear();

            foreach (var spec in Specifications)
            {
                int usedInOtherSteps = Steps
                    .Where(s => s != existingStep) // Исключаем текущий этап из подсчета
                    .SelectMany(s => s.Items)
                    .Where(i => i.NomenclatureName == spec.NomenclatureName)
                    .Sum(i => i.Quantity);

                int available = spec.Quantity - usedInOtherSteps;

                if (available > 0 || (existingStep != null && existingStep.Items.Any(i => i.NomenclatureName == spec.NomenclatureName)))
                {
                    int alreadyInThisStep = existingStep?.Items
                        .FirstOrDefault(i => i.NomenclatureName == spec.NomenclatureName)?.Quantity ?? 0;

                    var wrapper = new StepItemWrapper(spec)
                    {
                        AvailableQuantity = available,
                        QuantityToTake = alreadyInThisStep
                    };

                    wrapper.WhenAnyValue(x => x.QuantityToTake)
                           .Subscribe(_ => CalculateCurrentStepTotal());

                    StepEditorItems.Add(wrapper);
                }
            }

            CalculateCurrentStepTotal();
            IsStepEditorOpen = true;
        }

        private void CalculateCurrentStepTotal()
        {
            CurrentStepTotalAmount = StepEditorItems.Sum(x => x.StepTotalAmount);
        }

        private void SaveStep()
        {
            if (CurrentEditingStep == null) return;

            if (string.IsNullOrWhiteSpace(CurrentEditingStep.StepName))
            {
                _dialogService.ShowMessageAsync("Внимание", "Укажите наименование этапа.", UserMessageType.Warning);
                return;
            }

            // Проверяем, не ввел ли юзер больше, чем доступно
            if (StepEditorItems.Any(x => x.QuantityToTake > x.AvailableQuantity))
            {
                _dialogService.ShowMessageAsync("Ошибка", "Количество в этапе не может превышать доступный остаток по спецификации.", UserMessageType.Error);
                return;
            }

            var itemsToAdd = StepEditorItems.Where(x => x.QuantityToTake > 0).ToList();

            if (!itemsToAdd.Any())
            {
                _dialogService.ShowMessageAsync("Внимание", "Этап не может быть пустым. Укажите количество хотя бы для одной позиции.", UserMessageType.Warning);
                return;
            }

            var newStepItems = new List<ContractStepItemDto>();
            foreach (var wrapper in itemsToAdd)
            {
                var total = wrapper.QuantityToTake * wrapper.Price;
                var vatAmount = total * (wrapper.BaseSpec.VATRate / 100m);

                newStepItems.Add(new ContractStepItemDto
                {
                    NomenclatureName = wrapper.Name,
                    UnitOfMeasure = wrapper.Unit,
                    Quantity = wrapper.QuantityToTake,
                    UnitPrice = wrapper.Price,
                    TotalAmount = total
                });
            }

            CurrentEditingStep.Items = newStepItems;
            CurrentEditingStep.TotalAmount = newStepItems.Sum(x => x.TotalAmount);

            if (!Steps.Contains(CurrentEditingStep))
                Steps.Add(CurrentEditingStep);

            IsStepEditorOpen = false;
        }

        private void DeleteStep(ContractStepDto step)
        {
            if (step != null) 
                Steps.Remove(step);
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is EditorParams param)
            {
                FirmId = param.ParentId > 0 ? param.ParentId : _settingsService.CurrentFirmId ?? 0;

                if (FirmId == 0)
                {
                    await _dialogService.ShowMessageAsync("Ошибка", "Не определена активная фирма.", UserMessageType.Error);
                    _navigation.NavigateBack();
                    return;
                }

                await LoadDictionariesAsync();

                if (param.Mode == EditorMode.Edit)
                {
                    await LoadDataAsync(param.Id);
                }
                else
                {
                    IssueDate = DateOnly.FromDateTime(DateTime.Now);
                    SelectedCurrency = Currencies.FirstOrDefault(c => c.Code == "RUB" || c.Code == "643");
                    SelectedStage = AvailableStages.FirstOrDefault(); // Обычно первая стадия - Черновик
                }

                this.RaisePropertyChanged(nameof(PageTitle));
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadDictionariesAsync()
        {
            try
            {
                var currenciesTask = _classifierService.GetCurrenciesAsync();
                var stagesTask = _contractService.GetAllStagesAsync();
                var counterpartiesTask = _counterpartyService.GetAllCounterpartiesAsync();
                var workersTask = _workerService.GetWorkersByFirmIdAsync(_settingsService.CurrentFirmId ?? throw new UserMessageException("Фирма не выбрана!"));

                await Task.WhenAll(currenciesTask, counterpartiesTask, workersTask, stagesTask);

                Currencies.Clear();
                foreach (var currency in await currenciesTask) 
                    Currencies.Add(currency);

                Counterparties.Clear();
                foreach (var counterparty in await counterpartiesTask) 
                    Counterparties.Add(counterparty);

                Workers.Clear();
                foreach (var worker in await workersTask) 
                    Workers.Add(worker);

                AllStages.Clear();
                foreach (var stage in await stagesTask) 
                    AllStages.Add(stage);

                FilterAvailableStages();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки справочников для контракта");
            }
        }

        private async Task OpenProductSelectorAsync()
        {
            if (!AllProducts.Any())
            {
                try
                {
                    var products = await _productService.GetAllAsync();
                    var firmProducts = products.Where(p => p.FirmId == FirmId).ToList();

                    AllProducts.Clear();
                    foreach (var product in firmProducts) 
                        AllProducts.Add(product);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка загрузки номенклатуры");
                    await _dialogService.ShowMessageAsync("Ошибка", "Не удалось загрузить список номенклатуры.", UserMessageType.Error);
                    return;
                }
            }

            ProductSearchText = string.Empty;
            FilterProducts(string.Empty);
            SelectedProductToAdd = null;
            QuantityToAdd = 1;

            IsProductSelectorOpen = true;
        }

        private void FilterProducts(string searchText)
        {
            FilteredProducts.Clear();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                foreach (var p in AllProducts) 
                    FilteredProducts.Add(p);
            }
            else
            {
                var lowerSearch = searchText.ToLower();
                var filtered = AllProducts.Where(p => p.Name.ToLower().Contains(lowerSearch)).ToList();
                foreach (var p in filtered) FilteredProducts.Add(p);
            }
        }

        private void AddSelectedProductToSpecification()
        {
            if (SelectedProductToAdd == null)
            {
                _dialogService.ShowMessageAsync("Внимание", "Выберите позицию из списка.", UserMessageType.Warning);
                return;
            }

            if (QuantityToAdd <= 0)
            {
                _dialogService.ShowMessageAsync("Внимание", "Количество должно быть больше 0.", UserMessageType.Warning);
                return;
            }

            if (Specifications.Any(s => s.NomenclatureName == SelectedProductToAdd.Name))
            {
                _dialogService.ShowMessageAsync("Внимание", "Эта позиция уже есть в спецификации.", UserMessageType.Warning);
                return;
            }

            var price = SelectedProductToAdd.Price;
            var totalAmount = price * QuantityToAdd;
            var vatPrice = totalAmount * (VatRateToAdd / 100m);

            var spec = new ContractSpecificationDto
            {
                ContractId = Id,
                NomenclatureName = SelectedProductToAdd.Name,
                UnitOfMeasure = SelectedProductToAdd.UnitOfMeasure ?? "шт.",
                Quantity = QuantityToAdd,
                UnitPrice = price,
                TotalAmount = totalAmount,
                VATRate = VatRateToAdd,
                VATPrice = vatPrice,
                TotalVATAmount = totalAmount + vatPrice
            };

            Specifications.Add(spec);
            IsProductSelectorOpen = false;
        }

        public void DeleteSpecificationItem(ContractSpecificationDto spec)
        {
            if (spec != null)
                Specifications.Remove(spec);
        }

        private async Task LoadCounterpartyContactsAsync(int? counterpartyId)
        {
            CounterpartyContacts.Clear();
            SelectedCounterpartySigner = null;

            if (counterpartyId == null) return;

            try
            {
                var counterparty = await _counterpartyService.GetCounterpartyByIdAsync(counterpartyId.Value);
                if (counterparty?.Contacts != null)
                {
                    foreach (var contact in counterparty.Contacts)
                        CounterpartyContacts.Add(contact);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки контактов контрагента");
            }
        }

        private async Task LoadDataAsync(int id)
        {
            try
            {
                var dto = await _contractService.GetContractByIdAsync(id);
                if (dto != null)
                {
                    Id = dto.Id;
                    FirmId = dto.FirmId;
                    SelectedType = dto.Type;
                    SelectedRole = dto.EnterpriseRole;
                    ContractNumber = dto.ContractNumber;
                    ContractPrice = dto.ContractPrice;
                    ContractSubject = dto.ContractSubject;
                    SubmissionDate = dto.SubmissionDate;
                    SubmissionCode = dto.SubmissionCode;
                    SubmissionLink = dto.SubmissionLink;
                    TenderDate = dto.TenderDate;
                    IssueDate = dto.IssueDate;
                    StartDate = dto.StartDate;
                    EndDate = dto.EndDate;
                    ExecutionDate = dto.ExecutionDate;
                    TerminationReason = dto.TerminationReason;

                    if (dto.Initiator.HasValue)
                        SelectedInitiator = dto.Initiator.Value;

                    SelectedCounterparty = Counterparties.FirstOrDefault(c => c.Id == dto.CounterpartyId);
                    SelectedFirmSigner = Workers.FirstOrDefault(w => w.Id == dto.FirmSignerId);
                    SelectedCurrency = Currencies.FirstOrDefault(c => c.Id == dto.CurrencyId);
                    SelectedStage = AllStages.FirstOrDefault(s => s.Id == dto.StageTypeId);

                    if (dto.CounterpartySignerId.HasValue)
                    {
                        await LoadCounterpartyContactsAsync(dto.CounterpartyId);
                        SelectedCounterpartySigner = CounterpartyContacts.FirstOrDefault(c => c.Id == dto.CounterpartySignerId);
                    }

                    if (dto.Specifications != null)
                        foreach (var spec in dto.Specifications) 
                            Specifications.Add(spec);

                    if (dto.Steps != null)
                        foreach (var step in dto.Steps) 
                            Steps.Add(step);

                    if (dto.Files != null && dto.Files.Any())
                        await AttachedFilesVM.LoadExistingFilesAsync(dto.Files);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки контракта");
                await _dialogService.ShowMessageAsync("Ошибка", "Не удалось загрузить данные контракта.", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (SelectedCounterparty == null) throw new UserMessageException("Контрагент не выбран.");
            if (SelectedCounterpartySigner == null) throw new UserMessageException("Подписант со стороны контрагента не выбран.");
            if (SelectedFirmSigner == null) throw new UserMessageException("Подписант со стороны предприятия не выбран.");
            if (SelectedStage == null) throw new UserMessageException("Статус не выбран.");

            int stageId = SelectedStage.Id;

            if (SelectedRole == ContractEnterpriseRole.Contractor)
            {
                if (stageId == (int)ContractStageType.Tender ||
                    stageId == (int)ContractStageType.TenderLost ||
                    stageId == (int)ContractStageType.Conclusion ||
                    stageId == (int)ContractStageType.Concluded)
                {
                    ValidateCommonFields();
                    ValidateContractorFields();
                }
            }
            else if (SelectedRole == ContractEnterpriseRole.Customer)
            {
                if (stageId == (int)ContractStageType.Tender ||
                    stageId == (int)ContractStageType.TenderLost ||
                    stageId == (int)ContractStageType.Conclusion ||
                    stageId == (int)ContractStageType.Concluded ||
                    stageId == (int)ContractStageType.Execution ||
                    stageId == (int)ContractStageType.Executed ||
                    stageId == (int)ContractStageType.Paid)
                {
                    ValidateCommonFields();
                    ValidateCustomerFields();
                }
            }

            if (IsSubmission)
            {
                if (SubmissionDate == null) throw new UserMessageException("Дата подачи заявки не заполнена.");
                if (string.IsNullOrEmpty(SubmissionCode)) throw new UserMessageException("Идентификационный код закупки не заполнен.");
            }

            if (IsTermination)
            {
                if (string.IsNullOrEmpty(TerminationReason)) throw new UserMessageException("Причина расторжения не заполнена.");
                if (SelectedInitiator == null) throw new UserMessageException("Инициатор расторжения не выбран.");
            }
        }

        private void ValidateCommonFields()
        {
            if (string.IsNullOrWhiteSpace(ContractNumber)) throw new UserMessageException("Введите номер контракта или договора.");
            if (IssueDate == null) throw new UserMessageException("Дата подписания не выбрана.");
            if (StartDate == null) throw new UserMessageException("Дата начала не выбрана.");
            if (EndDate == null) throw new UserMessageException("Дата окончания не выбрана.");
        }

        private void ValidateContractorFields()
        {
            if (Specifications.Count == 0) throw new UserMessageException("Добавьте номенклатуру товара или услуги!");
        }

        private void ValidateCustomerFields()
        {
            if (ContractPrice <= 0) throw new UserMessageException("Введите сумму контракта или договора.");
        }

        private async Task SaveAsync()
        {
            try
            {
                Validate();

                var currentWorkerId = _settingsService.CurrentWorkerId;
                if (currentWorkerId == 0)
                    throw new UserMessageException("Не определен текущий пользователь системы.");

                var contractFiles = await AttachedFilesVM.GetFilesForCommitAsync(Id);

                var dto = new ContractDto
                {
                    Id = Id,
                    FirmId = FirmId,
                    Type = SelectedType,
                    EnterpriseRole = SelectedRole,
                    ContractNumber = ContractNumber,
                    ContractPrice = ContractPrice,
                    ContractSubject = ContractSubject,
                    SubmissionDate = SubmissionDate,
                    SubmissionCode = SubmissionCode,
                    SubmissionLink = SubmissionLink,
                    TenderDate = TenderDate,
                    IssueDate = IssueDate,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    ExecutionDate = ExecutionDate,
                    TerminationReason = TerminationReason,
                    Initiator = SelectedInitiator.HasValue ? SelectedInitiator.Value : null,
                    CounterpartyId = SelectedCounterparty!.Id,
                    CounterpartySignerId = SelectedCounterpartySigner?.Id,
                    FirmSignerId = SelectedFirmSigner?.Id ?? 0,
                    CurrencyId = SelectedCurrency!.Id,
                    StageTypeId = SelectedStage!.Id,
                    Files = contractFiles
                };

                await _contractService.SaveContractWithDetailsAsync(
                    dto,
                    Specifications.ToList(),
                    Steps.ToList(),
                    currentWorkerId ?? 0
                );

                _navigation.NavigateBack();
            }
            catch (UserMessageException ex)
            {
                await _dialogService.ShowMessageAsync("Внимание", ex.Message, UserMessageType.Warning);
            }
            catch (Exception ex)
            {
                await AttachedFilesVM.RollbackCommitAsync();

                Log.Error(ex, "Ошибка сохранения контракта");
                await _dialogService.ShowMessageAsync("Ошибка", "Произошла ошибка при сохранении документа.", UserMessageType.Error);
            }
        }
    }

    public class StepItemWrapper : ReactiveObject
    {
        public ContractSpecificationDto BaseSpec { get; }

        public string Name => BaseSpec.NomenclatureName;
        public string Unit => BaseSpec.UnitOfMeasure ?? "шт.";
        public decimal Price => BaseSpec.UnitPrice;
        public int TotalQuantity => BaseSpec.Quantity;
        public decimal StepTotalAmount => QuantityToTake * Price;

        [Reactive] public int AvailableQuantity { get; set; }
        [Reactive] public int QuantityToTake { get; set; }

        public StepItemWrapper(ContractSpecificationDto spec)
        {
            BaseSpec = spec;

            this.WhenAnyValue(x => x.QuantityToTake) // Пересчитываем сумму при каждом изменении количества
                .Subscribe(_ => this.RaisePropertyChanged(nameof(StepTotalAmount)));
        }
    }
}
