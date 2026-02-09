using Avalonia.Controls;

namespace ContractCreator.UI.ViewModels.Firms
{
    public class FirmListViewModel : ViewModelBase
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;

        public ObservableCollection<FirmDto> Firms { get; } = new();
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
            ISettingsService settingsService)
        {
            _firmService = firmService;
            _navigation = navigation;
            _settingsService = settingsService;

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

            SetCurrentCommand = ReactiveCommand.Create<FirmDto>(firm =>
            {
                _settingsService.CurrentFirmId = firm.Id;
                CurrentActiveFirmId = firm.Id;
                throw new UserMessageException($"Рабочая фирма изменена на: " +
                    $"{firm.ShortName}", "Успех", UserMessageType.Info);
            });

            LoadDataAsync().ConfigureAwait(false);
        }

        public async Task LoadDataAsync()
        {
            try
            {
                Firms.Clear();
                var list = await _firmService.GetAllFirmsAsync();
                foreach (var item in list)
                    Firms.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке фирм!",
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}
