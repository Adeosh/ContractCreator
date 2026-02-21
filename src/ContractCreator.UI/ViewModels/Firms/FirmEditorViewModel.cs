namespace ContractCreator.UI.ViewModels.Firms
{
    public class FirmEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly INavigationService _navigation;
        private readonly ISettingsService _settingsService;
        private readonly IGarService _garService;
        private readonly IClassifierService _classifierService;
        private readonly IUserDialogService _dialogService;
        private readonly IFileService _fileService;

        [Reactive] public int Id { get; set; }
        [Reactive] public string FullName { get; set; } = "";
        [Reactive] public string ShortName { get; set; } = "";
        [Reactive] public string Phone { get; set; } = "";
        [Reactive] public string Email { get; set; } = "";
        [Reactive] public string Inn { get; set; } = "";
        [Reactive] public string? Kpp { get; set; }
        [Reactive] public string? Ogrn { get; set; }
        [Reactive] public string? Oktmo { get; set; }
        [Reactive] public string? Okpo { get; set; }
        [Reactive] public string? Erns { get; set; }
        [Reactive] public string? ExtraInformation { get; set; }
        [Reactive] public bool IsVatPayment { get; set; } = true;
        [Reactive] public string? FacsimileName { get; set; }
        [Reactive] public byte[]? FacsimileSeal { get; set; }
        [Reactive] public Bitmap? FacsimileSealBitmap { get; set; }

        public AddressViewModel LegalAddressVM { get; }
        public AddressViewModel ActualAddressVM { get; }
        public BankAccountsViewModel BankAccountVM { get; }
        public EconomicActivitiesViewModel OkvedVM { get; }
        public AttachedFilesViewModel AttachedFilesVM { get; }

        public ObservableCollection<ClassifierDto> OkopfList { get; } = new();
        [Reactive] public ClassifierDto? SelectedOkopf { get; set; }

        public LegalFormType[] LegalForms => Enum.GetValues<LegalFormType>();
        [Reactive] public LegalFormType SelectedLegalForm { get; set; }

        public TaxationSystemType[] TaxationTypes => Enum.GetValues<TaxationSystemType>();
        [Reactive] public TaxationSystemType SelectedTaxationType { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> UploadSealCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveSealCommand { get; }
        #endregion

        public FirmEditorViewModel(
            IFirmService firmService,
            INavigationService navigation,
            ISettingsService settingsService,
            IGarService garService,
            IBankAccountService bankAccountService,
            IBicService bicService,
            IUserDialogService dialogService,
            IClassifierService classifierService,
            IFileService fileService)
        {
            _firmService = firmService;
            _navigation = navigation;
            _settingsService = settingsService;
            _garService = garService;
            _classifierService = classifierService;
            _dialogService = dialogService;
            _fileService = fileService;

            LegalAddressVM = new AddressViewModel(_garService);
            ActualAddressVM = new AddressViewModel(_garService);
            BankAccountVM = new BankAccountsViewModel(
                bankAccountService,
                bicService,
                dialogService);
            OkvedVM = new EconomicActivitiesViewModel(classifierService);
            AttachedFilesVM = new AttachedFilesViewModel(fileService, dialogService, FileType.Firm);

            SaveCommand = ReactiveCommand.CreateFromTask(SaveFirmAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            UploadSealCommand = ReactiveCommand.CreateFromTask(UploadSealAsync);

            RemoveSealCommand = ReactiveCommand.Create(() =>
            {
                FacsimileSeal = null;
                FacsimileSealBitmap = null;
            });
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            await LoadOkopfAsync();
            await OkvedVM.LoadDictionaryAsync();

            if (parameter is int id && id != 0)
                await LoadFirmAsync(id);
            else if (parameter is EditorParams param && param.Mode == EditorMode.Edit)
                await LoadFirmAsync(param.Id);
            else
                await BankAccountVM.LoadDataAsync(0, OwnerType.Firm);
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadOkopfAsync()
        {
            try
            {
                var okopfs = await _classifierService.GetOkopfsAsync();

                OkopfList.Clear();
                foreach (var item in okopfs)
                    OkopfList.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка загрузки справочников!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private async Task UploadSealAsync()
        {
            try
            {
                var result = await FileHelper.PickImageAsync("Выберите скан печати");

                if (result != null)
                {
                    FacsimileName = result.Value.FileName;
                    FacsimileSeal = result.Value.Data;

                    using var imageStream = new MemoryStream(FacsimileSeal);
                    FacsimileSealBitmap = new Bitmap(imageStream);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузки факсимиле!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        public async Task LoadFirmAsync(int firmId)
        {
            try
            {
                var dto = await _firmService.GetFirmByIdAsync(firmId);
                if (dto != null)
                {
                    Id = dto.Id;
                    FullName = dto.FullName;
                    ShortName = dto.ShortName;
                    Phone = dto.Phone;
                    Email = dto.Email;
                    Inn = dto.INN;
                    Kpp = dto.KPP;
                    Ogrn = dto.OGRN;
                    Oktmo = dto.OKTMO;
                    Okpo = dto.OKPO;
                    Erns = dto.ERNS;
                    ExtraInformation = dto.ExtraInformation;
                    IsVatPayment = dto.IsVATPayment;
                    FacsimileName = dto.FacsimileName;
                    FacsimileSeal = dto.FacsimileSeal;
                    SelectedLegalForm = (LegalFormType)dto.LegalFormType;
                    SelectedTaxationType = (TaxationSystemType)dto.TaxationType;
                    SelectedOkopf = OkopfList.FirstOrDefault(x => x.Id == dto.OkopfId);
                    LegalAddressVM.SetData(dto.LegalAddress);
                    ActualAddressVM.SetData(dto.ActualAddress);
                    OkvedVM.SetData(dto.EconomicActivities);

                    if (FacsimileSeal != null && FacsimileSeal.Length > 0)
                    {
                        try
                        {
                            using var stream = new MemoryStream(FacsimileSeal);
                            FacsimileSealBitmap = new Bitmap(stream);
                        }
                        catch
                        {
                            FacsimileSealBitmap = null;
                        }
                    }
                    else
                        FacsimileSealBitmap = null;

                    if (dto.Files?.Any() == true)
                        await AttachedFilesVM.LoadExistingFilesAsync(dto.Files);

                    await BankAccountVM.LoadDataAsync(firmId, OwnerType.Firm);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке фирмы!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                throw new UserMessageException("Не заполнено полное наименование фирмы.");

            if (string.IsNullOrWhiteSpace(ShortName))
                throw new UserMessageException("Не заполнено краткое наименование фирмы.");

            if (string.IsNullOrWhiteSpace(Inn) || (Inn.Length != 10 && Inn.Length != 12))
                throw new UserMessageException("ИНН должен содержать 10 цифр (для ООО) или 12 цифр (для ИП).");

            if (string.IsNullOrWhiteSpace(Phone))
                throw new UserMessageException("Укажите номер телефона для связи.");

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                throw new UserMessageException("Укажите корректный адрес электронной почты.");

            if (SelectedOkopf == null)
                throw new UserMessageException("Необходимо выбрать ОКОПФ из справочника.");

            if (SelectedLegalForm == 0)
                throw new UserMessageException("Не выбрана организационно-правовая форма.");

            if (BankAccountVM.Accounts.Count == 0)
                throw new UserMessageException("Добавьте хотя бы один расчетный счет.");

            if (OkvedVM.SelectedActivities.Count == 0)
                throw new UserMessageException("Необходимо выбрать хотя бы один код ОКВЭД.");

            if (string.IsNullOrWhiteSpace(LegalAddressVM.SearchText))
                throw new UserMessageException("Юридический адрес не заполнен.");

            if (string.IsNullOrWhiteSpace(ActualAddressVM.SearchText))
                throw new UserMessageException("Фактический адрес не заполнен.");
        }

        private async Task SaveFirmAsync()
        {
            try
            {
                Validate();

                var firmFiles = await AttachedFilesVM.GetFilesForCommitAsync(Id);
                var dto = new FirmDto
                {
                    Id = Id,
                    LegalFormType = SelectedLegalForm,
                    OkopfId = SelectedOkopf!.Id,
                    FullName = FullName,
                    ShortName = ShortName,
                    Phone = Phone,
                    Email = Email,
                    INN = Inn,
                    KPP = Kpp,
                    OGRN = Ogrn,
                    OKTMO = Oktmo,
                    OKPO = Okpo,
                    ERNS = Erns,
                    TaxationType = SelectedTaxationType,
                    ExtraInformation = ExtraInformation,
                    IsVATPayment = IsVatPayment,
                    FacsimileName = FacsimileName,
                    FacsimileSeal = FacsimileSeal,
                    LegalAddress = LegalAddressVM.GetData(),
                    ActualAddress = ActualAddressVM.GetData(),
                    EconomicActivities = OkvedVM.GetData(),
                    Files = firmFiles,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    IsDeleted = false
                };

                if (Id == 0)
                {
                    var newId = await _firmService.CreateFirmAsync(dto);
                    Id = newId;

                    if (_settingsService.CurrentFirmId == null)
                    {
                        _settingsService.CurrentFirmId = newId;
                        _settingsService.CurrentFirmName = dto.ShortName;
                    }

                    await BankAccountVM.CommitPendingAccountsAsync(newId);
                }
                else
                    await _firmService.UpdateFirmAsync(dto);

                _navigation.NavigateBack();
            }
            catch (UserMessageException ex)
            {
                await AttachedFilesVM.RollbackCommitAsync();
                await _dialogService.ShowMessageAsync(ex.Title, ex.Message, UserMessageType.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при сохранении фирмы!",
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}
