namespace ContractCreator.UI.ViewModels.Contacts
{
    public class ContactListViewModel : EntityListViewModel<ContactDto>
    {
        #region Props
        private readonly IContactService _contactService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;

        [Reactive] public ContactDto? SelectedContact { get; set; }
        public ObservableCollection<ContactDto> Contacts { get; } = new();
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<ContactDto, Unit> EditCommand { get; }
        public ReactiveCommand<ContactDto, Unit> DeleteCommand { get; }
        #endregion

        public ContactListViewModel(
            IContactService contactService,
            ISettingsService settingsService,
            INavigationService navigation)
        {
            _contactService = contactService;
            _settingsService = settingsService;
            _navigation = navigation;

            LoadDataCommand = ReactiveCommand.CreateFromTask(RefreshListAsync);

            CreateCommand = ReactiveCommand.Create(() =>
            {
                var creationParams = new EditorParams { Mode = EditorMode.Create };
                _navigation.NavigateTo<ContactEditorViewModel>(creationParams);
            });

            EditCommand = ReactiveCommand.Create<ContactDto>(contact =>
            {
                var editParams = new EditorParams { Mode = EditorMode.Edit, Id = contact.Id };
                _navigation.NavigateTo<ContactEditorViewModel>(editParams);
            });

            DeleteCommand = ReactiveCommand.CreateFromTask<ContactDto>(async contact =>
            {
                await _contactService.DeleteContactAsync(contact.Id);
                Contacts.Remove(contact);
            });

            LoadDataCommand.Execute().Subscribe();
        }

        protected override async Task RefreshListAsync()
        {
            IsBusy = true;
            try
            {
                Contacts.Clear();
                var data = await _contactService.GetAllContactsAsync();

                if (data != null)
                {
                    foreach (var item in data)
                        Contacts.Add(item);
                }
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