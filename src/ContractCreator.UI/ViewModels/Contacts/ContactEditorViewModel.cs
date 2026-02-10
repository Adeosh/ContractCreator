namespace ContractCreator.UI.ViewModels.Contacts;

public class ContactEditorViewModel : ViewModelBase, IParametrizedViewModel
{
    #region Props
    private readonly IContactService _contactService;
    private readonly INavigationService _navigation;

    [Reactive] public int Id { get; set; }
    [Reactive] public string FirstName { get; set; } = string.Empty;
    [Reactive] public string LastName { get; set; } = string.Empty;
    [Reactive] public string MiddleName { get; set; } = string.Empty;
    [Reactive] public string Position { get; set; } = string.Empty;
    [Reactive] public string Phone { get; set; } = string.Empty;
    [Reactive] public string Email { get; set; } = string.Empty;
    [Reactive] public string? Note { get; set; }
    [Reactive] public bool IsDirector { get; set; }
    [Reactive] public bool IsAccountant { get; set; }
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

    public async Task ApplyParameterAsync(object parameter)
    {
        if (parameter is EditorParams param)
        {
            if (param.Mode == EditorMode.Create)
                CounterpartyId = param.ParentId;
            else if (param.Mode == EditorMode.Edit)
                await LoadContactAsync(param.Id);
        }
    }

    private async Task LoadContactAsync(int id)
    {
        try
        {
            var dto = await _contactService.GetContactByIdAsync(id);
            if (dto != null)
            {
                Id = dto.Id;
                CounterpartyId = dto.CounterpartyId;
                FirstName = dto.FirstName;
                LastName = dto.LastName;
                MiddleName = dto.MiddleName ?? string.Empty;
                Position = dto.Position;
                Phone = dto.Phone;
                Email = dto.Email ?? string.Empty;
                Note = dto.Note;
                IsDirector = dto.IsDirector;
                IsAccountant = dto.IsAccountant;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw new UserMessageException("Ошибка при загрузке контакта!",
                "Ошибка", UserMessageType.Error);
        }
    }

    private async Task SaveContactAsync()
    {
        try
        {
            var dto = new ContactDto()
            {
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

            await _contactService.CreateContactAsync(dto);

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