namespace ContractCreator.UI.ViewModels.Workers
{
    public class WorkerListViewModel : ViewModelBase
    {
        #region Props
        private readonly IWorkerService _workerService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;

        [Reactive] public WorkerDto? SelectedWorker { get; set; }
        public ObservableCollection<WorkerDto> Workers { get; } = new();
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<WorkerDto, Unit> EditCommand { get; }
        public ReactiveCommand<WorkerDto, Unit> DeleteCommand { get; }
        #endregion

        public WorkerListViewModel(
            IWorkerService workerService,
            ISettingsService settingsService,
            INavigationService navigation)
        {
            _workerService = workerService;
            _settingsService = settingsService;
            _navigation = navigation;

            LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);

            CreateCommand = ReactiveCommand.Create(() =>
            {
                var currentFirmId = _settingsService.CurrentFirmId;
                if (currentFirmId == null)
                    throw new UserMessageException("Не выбрана активная фирма. Выберите её в списке.", "Внимание", UserMessageType.Warning);

                var creationParams = new EditorParams { Mode = EditorMode.Create, ParentId = currentFirmId.Value };
                _navigation.NavigateTo<WorkerEditorViewModel>(creationParams);
            });

            EditCommand = ReactiveCommand.Create<WorkerDto>(worker =>
            {
                var editParams = new EditorParams { Mode = EditorMode.Edit, Id = worker.Id };
                _navigation.NavigateTo<WorkerEditorViewModel>(editParams);
            });

            DeleteCommand = ReactiveCommand.CreateFromTask<WorkerDto>(async worker =>
            {
                await _workerService.DeleteWorkerAsync(worker.Id);
                Workers.Remove(worker);
            });

            LoadDataCommand.Execute().Subscribe();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                Workers.Clear();
                var currentFirm = _settingsService.CurrentFirmId;
                if (currentFirm != null)
                {
                    var list = await _workerService.GetWorkersByFirmIdAsync(currentFirm.Value);
                    foreach (var item in list) 
                        Workers.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке сотрудников!", 
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}
