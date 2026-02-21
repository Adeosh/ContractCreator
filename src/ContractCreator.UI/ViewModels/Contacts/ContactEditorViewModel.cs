namespace ContractCreator.UI.ViewModels.Contacts
{
    public class ContactEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IContactService _contactService;
        private readonly ICounterpartyService _counterpartyService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        private ObservableCollection<ContactDto>? _inMemoryCollection;
        private ContactDto? _originalContact;

        [Reactive] public int Id { get; set; }
        [Reactive] public string FirstName { get; set; } = string.Empty;
        [Reactive] public string LastName { get; set; } = string.Empty;
        [Reactive] public string? MiddleName { get; set; }
        [Reactive] public string Position { get; set; } = string.Empty;
        [Reactive] public string Phone { get; set; } = string.Empty;
        [Reactive] public string? Email { get; set; }
        [Reactive] public string? Note { get; set; }
        [Reactive] public bool IsDirector { get; set; } = false;
        [Reactive] public bool IsAccountant { get; set; } = false;
        [Reactive] public int CounterpartyId { get; set; }
        [Reactive] public CounterpartyDto? SelectedCounterparty { get; set; }
        public ObservableCollection<CounterpartyDto> AvailableCounterparties { get; } = new();

        [Reactive] public bool IsCounterpartySelectionVisible { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        #endregion

        public ContactEditorViewModel(
            IContactService contactService,
            ICounterpartyService counterpartyService,
            ISettingsService settingsService,
            IUserDialogService dialogService,
            INavigationService navigation)
        {
            _contactService = contactService;
            _counterpartyService = counterpartyService;
            _settingsService = settingsService;
            _dialogService = dialogService;
            _navigation = navigation;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveContactAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is InMemoryEditorParams<ContactDto> memoryParams)
            {
                _inMemoryCollection = memoryParams.TargetCollection;
                _originalContact = memoryParams.ItemToEdit;
                IsCounterpartySelectionVisible = false;

                if (_originalContact != null)
                {
                    Id = _originalContact.Id;
                    FirstName = _originalContact.FirstName;
                    LastName = _originalContact.LastName;
                    MiddleName = _originalContact.MiddleName;
                    Position = _originalContact.Position;
                    Phone = _originalContact.Phone;
                    Email = _originalContact.Email;
                    Note = _originalContact.Note;
                    IsDirector = _originalContact.IsDirector;
                    IsAccountant = _originalContact.IsAccountant;
                    CounterpartyId = _originalContact.CounterpartyId;
                }
                return;
            }

            if (parameter is EditorParams param)
            {
                if (param.Mode == EditorMode.Create)
                {
                    if (param.ParentId > 0)
                    {
                        CounterpartyId = param.ParentId;
                        IsCounterpartySelectionVisible = false;
                    }
                    else
                    {
                        IsCounterpartySelectionVisible = true;
                        await LoadAvailableCounterpartiesAsync();
                    }
                }
                else if (param.Mode == EditorMode.Edit)
                {
                    IsCounterpartySelectionVisible = true;
                    await LoadAvailableCounterpartiesAsync();
                    await LoadContactAsync(param.Id);
                }
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadAvailableCounterpartiesAsync()
        {
            var firmId = _settingsService.CurrentFirmId;
            if (firmId == null) return;

            var list = await _counterpartyService.GetCounterpartiesByFirmIdAsync(firmId.Value);

            AvailableCounterparties.Clear();
            foreach (var cp in list)
                AvailableCounterparties.Add(cp);
        }

        private async Task LoadContactAsync(int id)
        {
            try
            {
                var dto = await _contactService.GetContactByIdAsync(id);
                if (dto != null)
                {
                    Id = dto.Id;
                    FirstName = dto.FirstName;
                    LastName = dto.LastName;
                    MiddleName = dto.MiddleName ?? string.Empty;
                    Position = dto.Position;
                    Phone = dto.Phone;
                    Email = dto.Email ?? string.Empty;
                    Note = dto.Note ?? string.Empty;
                    IsDirector = dto.IsDirector;
                    IsAccountant = dto.IsAccountant;
                    CounterpartyId = dto.CounterpartyId;
                    SelectedCounterparty = AvailableCounterparties.FirstOrDefault(x => x.Id == dto.CounterpartyId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке контакта!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (IsCounterpartySelectionVisible)
            {
                if (SelectedCounterparty == null)
                    throw new UserMessageException("Необходимо выбрать контрагента из списка.");

                CounterpartyId = SelectedCounterparty.Id;
            }

            if (string.IsNullOrWhiteSpace(FirstName))
                throw new UserMessageException("Не указано имя контактного лица.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new UserMessageException("Не указана фамилия контактного лица.");

            if (string.IsNullOrWhiteSpace(Position))
                throw new UserMessageException("Не указана должность контактного лица.");

            if (string.IsNullOrWhiteSpace(Phone))
                throw new UserMessageException("Не указан номер телефона контактного лица.");

            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
                throw new UserMessageException("Укажите корректный адрес электронной почты.");

            if (_inMemoryCollection == null && CounterpartyId <= 0)
                throw new UserMessageException("Не выбран контрагент для контакта.");
        }

        private async Task SaveContactAsync()
        {
            try
            {
                Validate();

                var dto = new ContactDto()
                {
                    Id = Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    Position = Position,
                    Phone = Phone,
                    Email = Email,
                    Note = Note,
                    IsDirector = IsDirector,
                    IsAccountant = IsAccountant,
                    CounterpartyId = CounterpartyId,
                    IsDeleted = false
                };

                if (_inMemoryCollection != null)
                {
                    if (_originalContact == null)
                    {
                        _inMemoryCollection.Add(dto);
                    }
                    else
                    {
                        var index = _inMemoryCollection.IndexOf(_originalContact);
                        if (index >= 0) _inMemoryCollection[index] = dto;
                    }

                    _navigation.NavigateBack();
                    return;
                }

                if (Id == 0)
                    await _contactService.CreateContactAsync(dto);
                else
                    await _contactService.UpdateContactAsync(dto);

                _navigation.NavigateBack();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при сохранении контакта!",
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}