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
        public ReactiveCommand<FirmDto, Unit> DeleteCommand { get; }
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

            CreateCommand = ReactiveCommand.Create(CreateFirm);
            EditCommand = ReactiveCommand.Create<FirmDto>(EditFirm);
            DeleteCommand = ReactiveCommand.CreateFromTask<FirmDto>(DeleteFirmAsync);
        }

        public override async Task OnNavigatedToAsync(object? parameter = null)
        {
            CurrentActiveFirmId = _settingsService.CurrentFirmId;
            await base.OnNavigatedToAsync(parameter);
        }

        private void CreateFirm()
        {
            try
            {
                var param = new EditorParams { Mode = EditorMode.Create };
                _navigation.NavigateTo<FirmEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при переходе на форму добавления фирмы.");
                _dialogService.ShowMessageAsync("Ошибка при создании фирмы!", "Ошибка", UserMessageType.Error).SafeFireAndForget();
            }
        }

        private void EditFirm(FirmDto firm)
        {
            if (firm == null) return;

            try
            {
                var param = new EditorParams
                {
                    Mode = EditorMode.Edit,
                    Id = firm.Id
                };
                _navigation.NavigateTo<FirmEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при переходе на форму редактирования фирмы ID: {FirmId}", firm.Id);
                _dialogService.ShowMessageAsync("Ошибка при переходе к редактированию фирмы!", "Ошибка", UserMessageType.Error).SafeFireAndForget();
            }
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
                Log.Error(ex, "Ошибка при загрузке списка фирм.");
                await _dialogService.ShowMessageAsync("Ошибка при загрузке списка фирм!", "Ошибка", UserMessageType.Error);
            }
            finally 
            { 
                IsBusy = false; 
            }
        }

        private async Task DeleteFirmAsync(FirmDto firm)
        {
            if (firm.Id == _settingsService.CurrentFirmId)
            {
                await _dialogService.ShowMessageAsync(
                    "Текущую активную фирму удалять нельзя. Сначала выберите другую рабочую фирму.",
                    "Внимание",
                    UserMessageType.Warning);
                return;
            }

            bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить фирму \"{firm.ShortName}\"?\nЭто действие нельзя отменить.",
                "Подтверждение удаления");

            if (!isConfirmed) return;

            try
            {
                await _firmService.DeleteFirmAsync(firm.Id);
                Items.Remove(firm);

                Log.Information("Фирма успешно удалена: {ShortName} (ID: {FirmId})", firm.ShortName, firm.Id);
                await _dialogService.ShowMessageAsync($"Фирма \"{firm.ShortName}\" успешно удалена.", "Успех", UserMessageType.Info);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении фирмы ID: {FirmId}", firm.Id);
                await _dialogService.ShowMessageAsync("Не удалось удалить фирму.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
