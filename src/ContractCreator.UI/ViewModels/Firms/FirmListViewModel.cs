using System.Reactive.Concurrency;

namespace ContractCreator.UI.ViewModels.Firms
{
    public class FirmListViewModel : EntityListViewModel<FirmDto>
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public FirmDto? SelectedFirm { get; set; }
        [Reactive] public int? CurrentActiveFirmId { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<FirmDto, Unit> EditCommand { get; }
        public ReactiveCommand<FirmDto, Unit> SetCurrentCommand { get; }
        #endregion

        public FirmListViewModel(
            IFirmService firmService,
            INavigationService navigation,
            ISettingsService settingsService,
            IUserDialogService dialogService)
        {
            _firmService = firmService;
            _navigation = navigation;
            _settingsService = settingsService;
            _dialogService = dialogService;

            CurrentActiveFirmId = _settingsService.CurrentFirmId;

            CreateCommand = ReactiveCommand.Create(() =>
            {
                var param = new EditorParams { Mode = EditorMode.Create };
                _navigation.NavigateTo<FirmEditorViewModel>(param);
            });

            EditCommand = ReactiveCommand.Create<FirmDto>(firm =>
            {
                var param = new EditorParams
                {
                    Mode = EditorMode.Edit,
                    Id = firm.Id
                };
                _navigation.NavigateTo<FirmEditorViewModel>(param);
            });

            SetCurrentCommand = ReactiveCommand.Create<FirmDto>(async firm =>
            {
                _settingsService.CurrentFirmId = firm.Id;
                _settingsService.CurrentFirmName = firm.ShortName;
                CurrentActiveFirmId = firm.Id;

                await _dialogService.ShowMessageAsync($"Рабочая фирма изменена на: " +
                    $"{firm.ShortName}", "Успех", UserMessageType.Info);
            });

            RxApp.MainThreadScheduler.Schedule(async () => await RefreshListAsync());
        }

        protected override async Task RefreshListAsync()
        {
            IsBusy = true;
            try
            {
                var data = await _firmService.GetAllFirmsAsync();
                Items.Clear();
                if (data != null)
                {
                    foreach (var item in data)
                        Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке фирм!",
                    "Ошибка", UserMessageType.Error);
            }
            finally 
            { 
                IsBusy = false; 
            }
        }
    }
}
