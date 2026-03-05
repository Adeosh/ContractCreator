namespace ContractCreator.UI.ViewModels.Firms
{
    public class FirmEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly INavigationService _navigation;
        private readonly ISettingsService _settingsService;
        private readonly IClassifierService _classifierService;
        private readonly IUserDialogService _dialogService;

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
            IUserDialogService dialogService,
            IClassifierService classifierService,
            AddressViewModel legalAddressVM,
            AddressViewModel actualAddressVM,
            BankAccountsViewModel bankAccountVM,
            EconomicActivitiesViewModel okvedVM,
            AttachedFilesViewModel attachedFilesVM)
        {
            _firmService = firmService;
            _navigation = navigation;
            _settingsService = settingsService;
            _classifierService = classifierService;
            _dialogService = dialogService;

            LegalAddressVM = legalAddressVM;
            ActualAddressVM = actualAddressVM;
            BankAccountVM = bankAccountVM;
            OkvedVM = new EconomicActivitiesViewModel(classifierService);
            AttachedFilesVM = attachedFilesVM;
            AttachedFilesVM.CurrentFileType = FileType.Firm;

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
                Log.Error(ex, "Ошибка при загрузке справочника ОКОПФ.");
                await _dialogService.ShowMessageAsync("Ошибка загрузки справочников!", "Ошибка", UserMessageType.Error);
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
                Log.Error(ex, "Ошибка при загрузке файла факсимиле (печати).");
                await _dialogService.ShowMessageAsync("Ошибка при загрузке факсимиле!", "Ошибка", UserMessageType.Error);
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
                Log.Error(ex, "Ошибка при загрузке данных фирмы ID: {FirmId}", firmId);
                await _dialogService.ShowMessageAsync("Ошибка при загрузке фирмы!", "Ошибка", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                throw new UserMessageException("Не заполнено полное наименование фирмы.");

            if (string.IsNullOrWhiteSpace(ShortName))
                throw new UserMessageException("Не заполнено краткое наименование фирмы.");

            if (SelectedLegalForm == 0)
                throw new UserMessageException("Не выбрана организационно-правовая форма.");

            bool isPhysicalPerson = SelectedLegalForm == LegalFormType.IndividualPerson ||
                                    SelectedLegalForm == LegalFormType.SelfEmployed;

            if (string.IsNullOrWhiteSpace(Inn) || !Inn.All(char.IsDigit))
                throw new UserMessageException("ИНН должен состоять только из цифр.");

            if (isPhysicalPerson && Inn.Length != 12)
                throw new UserMessageException("Для ИП и Самозанятых ИНН должен состоять ровно из 12 цифр.");
            else if (!isPhysicalPerson && Inn.Length != 10)
                throw new UserMessageException("Для юридических лиц ИНН должен состоять ровно из 10 цифр.");

            if (isPhysicalPerson && !string.IsNullOrWhiteSpace(Kpp))
                throw new UserMessageException("У ИП и Самозанятых не бывает КПП. Пожалуйста, очистите это поле.");

            if (!isPhysicalPerson && (string.IsNullOrWhiteSpace(Kpp) || Kpp.Length != 9 || !Kpp.All(char.IsDigit)))
                throw new UserMessageException("Для юридических лиц КПП обязателен и должен состоять ровно из 9 цифр.");

            if (!string.IsNullOrWhiteSpace(Ogrn))
            {
                if (!Ogrn.All(char.IsDigit))
                    throw new UserMessageException("ОГРН/ОГРНИП должен состоять только из цифр.");

                if (isPhysicalPerson && Ogrn.Length != 15)
                    throw new UserMessageException("ОГРНИП (для ИП) должен состоять ровно из 15 цифр.");
                else if (!isPhysicalPerson && Ogrn.Length != 13)
                    throw new UserMessageException("ОГРН (для юр. лиц) должен состоять ровно из 13 цифр.");
            }

            if (!string.IsNullOrWhiteSpace(Oktmo) && (Oktmo.Length != 8 && Oktmo.Length != 11 || !Oktmo.All(char.IsDigit)))
                throw new UserMessageException("ОКТМО должен состоять из 8 или 11 цифр.");

            if (!string.IsNullOrWhiteSpace(Okpo) && (Okpo.Length != 8 && Okpo.Length != 10 || !Okpo.All(char.IsDigit)))
                throw new UserMessageException("ОКПО должен состоять из 8 или 10 цифр.");

            // 5. Контакты и связи
            if (string.IsNullOrWhiteSpace(Phone))
                throw new UserMessageException("Укажите номер телефона для связи.");

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                throw new UserMessageException("Укажите корректный адрес электронной почты.");

            if (SelectedOkopf == null)
                throw new UserMessageException("Необходимо выбрать ОКОПФ из справочника.");

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
                    Log.Information("Успешно создана новая фирма: {ShortName} (ИНН: {INN}) с ID: {FirmId}", ShortName, Inn, newId);
                }
                else
                    await _firmService.UpdateFirmAsync(dto);

                _navigation.NavigateBack();
            }
            catch (UserMessageException)
            {
                await AttachedFilesVM.RollbackCommitAsync();
                throw;
            }
            catch (Exception ex)
            {
                await AttachedFilesVM.RollbackCommitAsync();

                Log.Error(ex, "Ошибка при сохранении фирмы. Редактируемый ID: {FirmId}, ИНН: {INN}", Id, Inn);
                await _dialogService.ShowMessageAsync("Ошибка при сохранении фирмы!", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
