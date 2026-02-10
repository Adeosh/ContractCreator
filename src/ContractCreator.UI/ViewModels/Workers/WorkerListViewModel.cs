using ContractCreator.UI.ViewModels.Base;
using System.Reactive.Concurrency;

namespace ContractCreator.UI.ViewModels.Workers
{
    public class WorkerListViewModel : EntityListViewModel<WorkerDto>
    {
        #region Props
        private readonly IWorkerService _workerService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;

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
            INavigationService navigation)
        {
            _workerService = workerService;
            _settingsService = settingsService;
            _navigation = navigation;

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
                Items.Remove(worker);
            });

            RxApp.MainThreadScheduler.Schedule(async () => await RefreshListAsync());
        }

        protected override async Task RefreshListAsync()
        {
            IsBusy = true;
            try
            {
                var data = await _workerService.GetWorkersByFirmIdAsync(_settingsService.CurrentFirmId
                    ?? throw new UserMessageException("Ошибка при загрузке сотрудников! Фирма не выбрана!", "Ошибка", UserMessageType.Error));

                if (data != null)
                {
                    Items.Clear();
                    foreach (var item in data)
                        Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке сотрудников!",
                    "Ошибка", UserMessageType.Error);
            }
            finally 
            { 
                IsBusy = false; 
            }
        }
    }
}
