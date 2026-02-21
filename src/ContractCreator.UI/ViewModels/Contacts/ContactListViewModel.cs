namespace ContractCreator.UI.ViewModels.Contacts
{
    public class ContactListViewModel : EntityListViewModel<ContactDto>
    {
        #region Props
        private readonly IContactService _contactService;
        private readonly ICounterpartyService _counterpartyService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public ContactDto? SelectedContact { get; set; }

        public ObservableCollection<CounterpartyDto> FilterCounterparties { get; } = new();
        [Reactive] public CounterpartyDto? SelectedFilterCounterparty { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<ContactDto, Unit> EditCommand { get; }
        public ReactiveCommand<ContactDto, Unit> DeleteCommand { get; }
        #endregion

        public ContactListViewModel(
            IContactService contactService,
            ICounterpartyService counterpartyService,
            ISettingsService settingsService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _contactService = contactService;
            _counterpartyService = counterpartyService;
            _settingsService = settingsService;
            _navigation = navigation;
            _dialogService = dialogService;

            SetupContacts();

            CreateCommand = ReactiveCommand.Create(CreateContact);
            EditCommand = ReactiveCommand.Create<ContactDto>(EditContact);
            DeleteCommand = ReactiveCommand.CreateFromTask<ContactDto>(DeleteContactAsync);
        }

        private void SetupContacts()
        {
            this.WhenAnyValue(x => x.SelectedFilterCounterparty)
                .Subscribe(async _ => await RefreshListAsync());
        }

        public override async Task OnNavigatedToAsync(object? parameter = null)
        {
            await LoadFilterDataAsync();
        }

        private async Task LoadFilterDataAsync()
        {
            var firmId = _settingsService.CurrentFirmId;
            if (firmId == null) return;

            var list = await _counterpartyService.GetCounterpartiesByFirmIdAsync(firmId.Value);

            FilterCounterparties.Clear();
            FilterCounterparties.Add(new CounterpartyDto { Id = 0, ShortName = "Все контрагенты" });

            foreach (var cp in list)
                FilterCounterparties.Add(cp);

            if (SelectedFilterCounterparty == null)
                SelectedFilterCounterparty = FilterCounterparties[0];
            else
                await RefreshListAsync();
        }

        private void CreateContact()
        {
            try
            {
                var param = new EditorParams { Mode = EditorMode.Create, ParentId = 0 };
                _navigation.NavigateTo<ContactEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при добавлении контактов!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void EditContact(ContactDto contact)
        {
            if (contact == null) return;

            try
            {
                var editParams = new EditorParams { Mode = EditorMode.Edit, Id = contact.Id };
                _navigation.NavigateTo<ContactEditorViewModel>(editParams);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при обновлении контактов!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private async Task DeleteContactAsync(ContactDto contact)
        {
            if (contact == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить контактное лицо {contact.LastName} {contact.FirstName}?",
                "Удаление контакта");

            if (!confirm) return;

            try
            {
                await _contactService.DeleteContactAsync(contact.Id);
                Items.Remove(contact);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при удалении контактов!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        protected override async Task RefreshListAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                Items.Clear();
                IEnumerable<ContactDto> data;

                if (SelectedFilterCounterparty == null || SelectedFilterCounterparty.Id == 0)
                    data = await _contactService.GetAllContactsAsync();
                else
                    data = await _contactService.GetContactsByCounterpartyIdAsync(SelectedFilterCounterparty.Id);

                if (data != null)
                    foreach (var item in data) 
                        Items.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке контактов!",
                    "Ошибка", UserMessageType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}