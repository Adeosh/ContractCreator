namespace ContractCreator.UI.ViewModels.Workers
{
    public class WorkerEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IWorkerService _workerService;
        private readonly INavigationService _navigation;

        [Reactive] public int Id { get; set; }
        [Reactive] public string FirstName { get; set; } = string.Empty;
        [Reactive] public string LastName { get; set; } = string.Empty;
        [Reactive] public string MiddleName { get; set; }
        [Reactive] public string Position { get; set; } = string.Empty;
        [Reactive] public string Inn { get; set; } = string.Empty;
        [Reactive] public string Phone { get; set; } = string.Empty;
        [Reactive] public string Email { get; set; }
        [Reactive] public string? Note { get; set; }
        [Reactive] public bool IsDirector { get; set; }
        [Reactive] public bool IsAccountant { get; set; }
        [Reactive] public int FirmId { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        #endregion

        public WorkerEditorViewModel(IWorkerService workerService, INavigationService navigation)
        {
            _workerService = workerService;
            _navigation = navigation;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveWorkerAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is EditorParams param)
            {
                if (param.Mode == EditorMode.Create)
                    FirmId = param.ParentId;
                else if (param.Mode == EditorMode.Edit)
                    await LoadWorkerAsync(param.Id);
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadWorkerAsync(int id)
        {
            try
            {
                var dto = await _workerService.GetWorkerByIdAsync(id);
                if (dto != null)
                {
                    Id = dto.Id;
                    FirmId = dto.FirmId;
                    FirstName = dto.FirstName;
                    LastName = dto.LastName;
                    MiddleName = dto.MiddleName ?? string.Empty;
                    Position = dto.Position;
                    Inn = dto.INN;
                    Phone = dto.Phone;
                    Email = dto.Email ?? string.Empty;
                    Note = dto.Note ?? string.Empty;
                    IsDirector = dto.IsDirector;
                    IsAccountant = dto.IsAccountant;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке сотрудника!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new UserMessageException("Не указано имя сотрудника.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new UserMessageException("Не указана фамилия сотрудника.");

            if (string.IsNullOrWhiteSpace(Position))
                throw new UserMessageException("Не указана должность сотрудника.");

            if (string.IsNullOrWhiteSpace(Inn))
                throw new UserMessageException("Не указан ИНН сотрудника.");

            if (Inn.Length != 12)
                throw new UserMessageException("ИНН сотрудника должен содержать 12 цифр.");

            if (string.IsNullOrWhiteSpace(Phone))
                throw new UserMessageException("Не указан номер телефона сотрудника.");

            if (!Email.Contains("@"))
                throw new UserMessageException("Укажите корректный адрес электронной почты.");
        }


        private async Task SaveWorkerAsync()
        {
            try
            {
                Validate();

                var dto = new WorkerDto
                {
                    Id = Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    Position = Position,
                    INN = Inn,
                    Phone = Phone,
                    Email = Email,
                    Note = Note,
                    IsDirector = IsDirector,
                    IsAccountant = IsAccountant,
                    FirmId = FirmId,
                    IsDeleted = false
                };

                if (Id == 0)
                    await _workerService.CreateWorkerAsync(dto);
                else
                    await _workerService.UpdateWorkerAsync(dto);

                MessageBus.Current.SendMessage(new EntitySavedMessage<WorkerDto>(dto));
                _navigation.NavigateBack();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при сохранении сотрудника!",
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}
