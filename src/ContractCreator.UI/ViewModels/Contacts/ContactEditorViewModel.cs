namespace ContractCreator.UI.ViewModels.Contacts
{
    public class ContactEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IContactService _contactService;
        private readonly INavigationService _navigation;

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
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        #endregion

        public ContactEditorViewModel(IContactService contactService, INavigationService navigation)
        {
            _contactService = contactService;
            _navigation = navigation;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveContactAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is EditorParams param)
            {
                if (param.Mode == EditorMode.Create)
                    CounterpartyId = param.ParentId;
                else if (param.Mode == EditorMode.Edit)
                    await LoadContactAsync(param.Id);
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

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

            if (CounterpartyId <= 0)
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

                if (Id == 0)
                    await _contactService.CreateContactAsync(dto);
                else
                    await _contactService.UpdateContactAsync(dto);

                MessageBus.Current.SendMessage(new EntitySavedMessage<ContactDto>(dto));
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

