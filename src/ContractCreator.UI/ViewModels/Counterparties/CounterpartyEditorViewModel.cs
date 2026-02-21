namespace ContractCreator.UI.ViewModels.Counterparties
{
    public class CounterpartyEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly ICounterpartyService _counterpartyService;
        private readonly IContactService _contactService;
        private readonly INavigationService _navigation;
        private readonly ISettingsService _settingsService;
        private readonly IGarService _garService;
        private readonly IUserDialogService _dialogService;
        private readonly IFileService _fileService;

        [Reactive] public int Id { get; set; }
        [Reactive] public int FirmId { get; set; }
        [Reactive] public string FullName { get; set; } = "";
        [Reactive] public string ShortName { get; set; } = "";
        [Reactive] public string Phone { get; set; } = "";
        [Reactive] public string Email { get; set; } = "";
        [Reactive] public string Inn { get; set; } = "";
        [Reactive] public string? Kpp { get; set; }
        [Reactive] public string? Ogrn { get; set; }
        [Reactive] public string? Oktmo { get; set; }
        [Reactive] public string? Okpo { get; set; }
        [Reactive] public string? ExtraInformation { get; set; }

        public AddressViewModel LegalAddressVM { get; }
        public AddressViewModel ActualAddressVM { get; }
        public BankAccountsViewModel BankAccountVM { get; }
        public AttachedFilesViewModel AttachedFilesVM { get; }

        public LegalFormType[] LegalForms => Enum.GetValues<LegalFormType>();
        [Reactive] public LegalFormType SelectedLegalForm { get; set; }

        public ObservableCollection<ContactDto> Contacts { get; } = new();
        [Reactive] public ContactDto? SelectedContact { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> AddContactCommand { get; }
        public ReactiveCommand<ContactDto, Unit> EditContactCommand { get; }
        public ReactiveCommand<ContactDto, Unit> DeleteContactCommand { get; }
        #endregion

        public CounterpartyEditorViewModel(
            ICounterpartyService counterpartyService,
            IContactService contactService,
            INavigationService navigation,
            ISettingsService settingsService,
            IGarService garService,
            IBankAccountService bankAccountService,
            IBicService bicService,
            IUserDialogService dialogService,
            IFileService fileService)
        {
            _counterpartyService = counterpartyService;
            _contactService = contactService;
            _navigation = navigation;
            _settingsService = settingsService;
            _garService = garService;
            _dialogService = dialogService;
            _fileService = fileService;

            LegalAddressVM = new AddressViewModel(_garService);
            ActualAddressVM = new AddressViewModel(_garService);

            BankAccountVM = new BankAccountsViewModel(bankAccountService, bicService, dialogService);
            AttachedFilesVM = new AttachedFilesViewModel(fileService, dialogService, FileType.Counterparty);

            SaveCommand = ReactiveCommand.CreateFromTask(SaveCounterpartyAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());

            AddContactCommand = ReactiveCommand.Create(AddContact);
            EditContactCommand = ReactiveCommand.Create<ContactDto>(EditContact);
            DeleteContactCommand = ReactiveCommand.CreateFromTask<ContactDto>(DeleteContactAsync);
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is EditorParams param)
            {
                if (param.Mode == EditorMode.Create)
                {
                    FirmId = param.ParentId;
                    await BankAccountVM.LoadDataAsync(0, OwnerType.Counterparty);
                }
                else if (param.Mode == EditorMode.Edit)
                {
                    await LoadCounterpartyAsync(param.Id);
                }
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadCounterpartyAsync(int counterpartyId)
        {
            try
            {
                var dto = await _counterpartyService.GetCounterpartyByIdAsync(counterpartyId);
                if (dto != null)
                {
                    Id = dto.Id;
                    FirmId = dto.FirmId;
                    FullName = dto.FullName;
                    ShortName = dto.ShortName;
                    Phone = dto.Phone ?? string.Empty;
                    Email = dto.Email ?? string.Empty;
                    Inn = dto.INN;
                    Kpp = dto.KPP;
                    Ogrn = dto.OGRN;
                    Oktmo = dto.OKTMO;
                    Okpo = dto.OKPO;
                    ExtraInformation = dto.ExtraInformation;
                    SelectedLegalForm = dto.LegalForm;
                    LegalAddressVM.SetData(dto.LegalAddress);
                    ActualAddressVM.SetData(dto.ActualAddress);

                    Contacts.Clear();
                    if (dto.Contacts != null && dto.Contacts.Any())
                        foreach (var contact in dto.Contacts)
                            Contacts.Add(contact);

                    if (dto.Files?.Any() == true)
                        await AttachedFilesVM.LoadExistingFilesAsync(dto.Files);

                    await BankAccountVM.LoadDataAsync(counterpartyId, OwnerType.Counterparty);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке контрагента!", "Ошибка", UserMessageType.Error);
            }
        }

        private void AddContact()
        {
            _navigation.NavigateTo<ContactEditorViewModel>(new InMemoryEditorParams<ContactDto>
            {
                TargetCollection = Contacts,
                ItemToEdit = null
            });
        }

        private void EditContact(ContactDto contact)
        {
            if (contact == null) return;

            _navigation.NavigateTo<ContactEditorViewModel>(new InMemoryEditorParams<ContactDto>
            {
                TargetCollection = Contacts,
                ItemToEdit = contact
            });
        }

        private async Task DeleteContactAsync(ContactDto contact)
        {
            if (contact == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить контакт {contact.LastName}?", "Удаление");

            if (!confirm) return;

            try
            {
                if (contact.Id > 0)
                    await _contactService.DeleteContactAsync(contact.Id);

                Contacts.Remove(contact);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении контакта");
                await _dialogService.ShowMessageAsync("Ошибка", "Не удалось удалить контакт.", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (FirmId == 0) 
                throw new UserMessageException("Не выбрана текущая фирма для привязки контрагента.");

            if (string.IsNullOrWhiteSpace(FullName)) 
                throw new UserMessageException("Не заполнено полное наименование.");

            if (string.IsNullOrWhiteSpace(ShortName)) 
                throw new UserMessageException("Не заполнено краткое наименование.");

            if (string.IsNullOrWhiteSpace(Inn) || (Inn.Length != 10 && Inn.Length != 12))
                throw new UserMessageException("ИНН должен содержать 10 или 12 цифр.");

            if (SelectedLegalForm == 0) 
                throw new UserMessageException("Не выбрана организационно-правовая форма.");

            if (string.IsNullOrWhiteSpace(LegalAddressVM.SearchText)) 
                throw new UserMessageException("Юридический адрес не заполнен.");

            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
                throw new UserMessageException("Укажите корректный адрес электронной почты.");
        }

        private async Task SaveCounterpartyAsync()
        {
            try
            {
                Validate();

                var counterpartyFiles = await AttachedFilesVM.GetFilesForCommitAsync(Id);
                var dto = new CounterpartyDto
                {
                    Id = Id,
                    FirmId = FirmId,
                    LegalForm = SelectedLegalForm,
                    FullName = FullName,
                    ShortName = ShortName,
                    Phone = Phone,
                    Email = Email,
                    INN = Inn,
                    KPP = Kpp,
                    OGRN = Ogrn,
                    OKTMO = Oktmo,
                    OKPO = Okpo,
                    ExtraInformation = ExtraInformation,
                    LegalAddress = LegalAddressVM.GetData(),
                    ActualAddress = ActualAddressVM.GetData(),
                    Files = counterpartyFiles,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    IsDeleted = false
                };

                if (Id == 0)
                {
                    var newId = await _counterpartyService.CreateCounterpartyAsync(dto);
                    Id = newId;

                    await BankAccountVM.CommitPendingAccountsAsync(newId);

                    foreach (var contact in Contacts)
                    {
                        contact.CounterpartyId = newId;
                        await _contactService.CreateContactAsync(contact);
                    }
                }
                else
                {
                    await _counterpartyService.UpdateCounterpartyAsync(dto);

                    foreach (var contact in Contacts)
                    {
                        contact.CounterpartyId = Id;

                        if (contact.Id == 0)
                            await _contactService.CreateContactAsync(contact);
                        else
                            await _contactService.UpdateContactAsync(contact);
                    }
                }

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
                throw new UserMessageException("Ошибка при сохранении контрагента!", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
