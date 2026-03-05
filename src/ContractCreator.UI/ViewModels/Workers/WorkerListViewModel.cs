namespace ContractCreator.UI.ViewModels.Workers
{
    public class WorkerListViewModel : EntityListViewModel<WorkerDto>
    {
        #region Props
        private readonly IWorkerService _workerService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public WorkerDto? SelectedWorker { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<WorkerDto, Unit> EditCommand { get; }
        public ReactiveCommand<WorkerDto, Unit> DeleteCommand { get; }
        #endregion

        public WorkerListViewModel(
            IWorkerService workerService,
            ISettingsService settingsService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _workerService = workerService;
            _settingsService = settingsService;
            _navigation = navigation;
            _dialogService = dialogService;

            CreateCommand = ReactiveCommand.Create(CreateWorker);
            EditCommand = ReactiveCommand.Create<WorkerDto>(EditWorker);
            DeleteCommand = ReactiveCommand.CreateFromTask<WorkerDto>(DeleteWorkerAsync);
        }

        private void CreateWorker()
        {
            var currentFirmId = _settingsService.CurrentFirmId;
            if (currentFirmId == null)
                throw new UserMessageException("Не выбрана активная фирма. Выберите её в списке.", "Внимание", UserMessageType.Warning);

            try
            {
                var creationParams = new EditorParams { Mode = EditorMode.Create, ParentId = currentFirmId.Value };
                _navigation.NavigateTo<WorkerEditorViewModel>(creationParams);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при переходе на форму добавления сотрудника.");
                _dialogService.ShowMessageAsync("Ошибка при добавлении сотрудников!", "Ошибка", UserMessageType.Error).SafeFireAndForget();
            }
        }

        private void EditWorker(WorkerDto worker)
        {
            if (worker == null) return;

            try
            {
                var editParams = new EditorParams { Mode = EditorMode.Edit, Id = worker.Id };
                _navigation.NavigateTo<WorkerEditorViewModel>(editParams);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при переходе на форму редактирования сотрудника ID: {WorkerId}", worker.Id);
                _dialogService.ShowMessageAsync("Ошибка при обновлении сотрудников!", "Ошибка", UserMessageType.Error).SafeFireAndForget();
            }
        }

        private async Task DeleteWorkerAsync(WorkerDto worker)
        {
            if (worker == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить сотрудника {worker.LastName} {worker.FirstName}?",
                "Удаление сотрудника");

            if (!confirm) return;

            try
            {
                await _workerService.DeleteWorkerAsync(worker.Id);
                Items.Remove(worker);

                Log.Information("Сотрудник успешно удален: {LastName} {FirstName} (ID: {WorkerId})", worker.LastName, worker.FirstName, worker.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении сотрудника ID: {WorkerId}", worker.Id);
                await _dialogService.ShowMessageAsync("Ошибка при удалении сотрудника!", "Ошибка", UserMessageType.Error);
            }
        }

        protected override async Task RefreshListAsync()
        {
            var currentFirmId = _settingsService.CurrentFirmId;
            if (currentFirmId == null)
                throw new UserMessageException("Ошибка при загрузке сотрудников! Фирма не выбрана!", "Ошибка", UserMessageType.Error);

            IsBusy = true;
            try
            {
                var data = await _workerService.GetWorkersByFirmIdAsync(currentFirmId.Value);

                if (data != null)
                {
                    Items.Clear();
                    foreach (var item in data)
                        Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке списка сотрудников для фирмы ID: {FirmId}", currentFirmId.Value);
                await _dialogService.ShowMessageAsync("Ошибка при загрузке сотрудников!", "Ошибка", UserMessageType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
