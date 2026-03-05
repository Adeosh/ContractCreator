namespace ContractCreator.UI.ViewModels.Counterparties
{
    public class CounterpartyEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly ICounterpartyService _counterpartyService;
        private readonly IContactService _contactService;
        private readonly INavigationService _navigation;
        private readonly ISettingsService _settingsService;
        private readonly IUserDialogService _dialogService;

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
            IUserDialogService dialogService,
            AddressViewModel legalAddressVM,
            AddressViewModel actualAddressVM,
            BankAccountsViewModel bankAccountVM,
            AttachedFilesViewModel attachedFilesVM)
        {
            _counterpartyService = counterpartyService;
            _contactService = contactService;
            _navigation = navigation;
            _settingsService = settingsService;
            _dialogService = dialogService;

            LegalAddressVM = legalAddressVM;
            ActualAddressVM = actualAddressVM;
            BankAccountVM = bankAccountVM;
            AttachedFilesVM = attachedFilesVM;
            AttachedFilesVM.CurrentFileType = FileType.Counterparty;

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
                Log.Error(ex, "Ошибка при загрузке данных контрагента ID: {CounterpartyId}", counterpartyId);
                await _dialogService.ShowMessageAsync("Ошибка при загрузке контрагента!", "Ошибка", UserMessageType.Error);
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

                Log.Information("Контакт {LastName} удален из карточки контрагента", contact.LastName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении контакта ID: {ContactId} из карточки контрагента", contact.Id);
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
                    Log.Information("Создан новый контрагент: {ShortName} (ИНН: {INN}) с ID: {CounterpartyId}", ShortName, Inn, newId);
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
                    Log.Information("Обновлен контрагент ID: {CounterpartyId} ({ShortName})", Id, ShortName);
                }

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

                Log.Error(ex, "Ошибка при сохранении контрагента. Редактируемый ID: {CounterpartyId}, ИНН: {INN}", Id, Inn);
                await _dialogService.ShowMessageAsync("Ошибка при сохранении контрагента!", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
