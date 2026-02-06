namespace ContractCreator.UI.ViewModels.Workers
{
    public class WorkerEditorViewModel : ViewModelBase, IParametrizedViewModel
    {
        #region Props
        private readonly IWorkerService _workerService;
        private readonly INavigationService _navigation;

        [Reactive] public int Id { get; set; }
        [Reactive] public string FirstName { get; set; } = string.Empty;
        [Reactive] public string LastName { get; set; } = string.Empty;
        [Reactive] public string MiddleName { get; set; } = string.Empty;
        [Reactive] public string Position { get; set; } = string.Empty;
        [Reactive] public string Inn { get; set; } = string.Empty;
        [Reactive] public string Phone { get; set; } = string.Empty;
        [Reactive] public string Email { get; set; } = string.Empty;
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

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
        }

        public async Task ApplyParameterAsync(object parameter)
        {
            if (parameter is EditorParams param)
            {
                if (param.Mode == EditorMode.Create)
                {
                    FirmId = param.ParentId;
                }
                else if (param.Mode == EditorMode.Edit)
                {
                    await LoadWorkerAsync(param.Id);
                }
            }
        }

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
                    Note = dto.Note;
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

        private async Task SaveAsync()
        {
            try
            {
                var dto = new WorkerDto
                {
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

                await _workerService.CreateWorkerAsync(dto);

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
