namespace ContractCreator.UI.ViewModels.Firms
{
    public class FirmEditorViewModel : ViewModelBase, IParametrizedViewModel
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly INavigationService _navigation;
        private readonly ISettingsService _settingsService;
        private readonly IGarService _garService;
        private readonly IClassifierService _classifierService;

        [Reactive] public int Id { get; set; }
        [Reactive] public string FullName { get; set; } = "";
        [Reactive] public string ShortName { get; set; } = "";
        [Reactive] public string Phone { get; set; } = "";
        [Reactive] public string Email { get; set; } = "";
        [Reactive] public string Inn { get; set; } = "";
        [Reactive] public string Kpp { get; set; } = "";
        [Reactive] public string Ogrn { get; set; } = "";
        [Reactive] public string Oktmo { get; set; } = "";
        [Reactive] public string Okpo { get; set; } = "";
        [Reactive] public string Erns { get; set; } = "";
        [Reactive] public string ExtraInformation { get; set; } = "";
        [Reactive] public bool IsVatPayment { get; set; } = true;

        public AddressViewModel LegalAddressVM { get; }
        public AddressViewModel ActualAddressVM { get; }
        public BankAccountsViewModel BankAccountVM { get; }

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
        #endregion

        public FirmEditorViewModel(
            IFirmService firmService,
            INavigationService navigation,
            ISettingsService settingsService,
            IGarService garService,
            IBankAccountService bankAccountService,
            IBicService bicService,
            IUserDialogService dialogService,
            IClassifierService classifierService)
        {
            _firmService = firmService;
            _navigation = navigation;
            _settingsService = settingsService;
            _garService = garService;
            _classifierService = classifierService;

            LegalAddressVM = new AddressViewModel(_garService);
            ActualAddressVM = new AddressViewModel(_garService);

            BankAccountVM = new BankAccountsViewModel(
                bankAccountService,
                bicService,
                dialogService);

            SaveCommand = ReactiveCommand.CreateFromTask(SaveFirmAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            SetupFirm();
        }

        private void SetupFirm()
        {

        }

        public async Task ApplyParameterAsync(object parameter)
        {
            await LoadDictionaries();

            if (parameter is int id && id != 0)
                await LoadFirmAsync(id);
            else if (parameter is EditorParams param && param.Mode == EditorMode.Edit)
                await LoadFirmAsync(param.Id);
            else
                await BankAccountVM.LoadDataAsync(0, OwnerType.Firm);
        }

        private async Task LoadDictionaries()
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
                Log.Error(ex, "Ошибка загрузки справочников");
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
                    Inn = dto.INN;
                    Kpp = dto.KPP ?? "";
                    Ogrn = dto.OGRN ?? "";
                    Phone = dto.Phone;
                    Email = dto.Email;
                    Oktmo = dto.OKTMO ?? "";
                    Okpo = dto.OKPO ?? "";
                    Erns = dto.ERNS ?? "";
                    ExtraInformation = dto.ExtraInformation ?? "";
                    IsVatPayment = dto.IsVATPayment;
                    SelectedLegalForm = (LegalFormType)dto.LegalFormType;
                    SelectedTaxationType = (TaxationSystemType)dto.TaxationType;
                    SelectedOkopf = OkopfList.FirstOrDefault(x => x.Id == dto.OkopfId);
                    LegalAddressVM.SetAddressDto(dto.LegalAddress);
                    ActualAddressVM.SetAddressDto(dto.ActualAddress);

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

        private async Task SaveFirmAsync()
        {
            try
            {
                var dto = new FirmDto
                {
                    Id = Id,
                    LegalFormType = (byte)SelectedLegalForm,
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
                    TaxationType = (byte)SelectedTaxationType,
                    LegalAddress = LegalAddressVM.GetAddressDto(),
                    ActualAddress = ActualAddressVM.GetAddressDto(),
                    ExtraInformation = ExtraInformation,
                    IsVATPayment = IsVatPayment,
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
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при сохранении фирмы!",
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}
