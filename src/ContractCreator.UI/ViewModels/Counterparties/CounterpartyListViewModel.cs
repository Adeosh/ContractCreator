namespace ContractCreator.UI.ViewModels.Counterparties
{
    public class CounterpartyListViewModel : EntityListViewModel<CounterpartyDto>
    {
        #region Props
        private readonly ICounterpartyService _counterpartyService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public CounterpartyDto? SelectedCounterparty { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<CounterpartyDto, Unit> EditCommand { get; }
        public ReactiveCommand<CounterpartyDto, Unit> DeleteCommand { get; }
        #endregion

        public CounterpartyListViewModel(
            ICounterpartyService counterpartyService,
            INavigationService navigation,
            ISettingsService settingsService,
            IUserDialogService dialogService)
        {
            _counterpartyService = counterpartyService;
            _navigation = navigation;
            _settingsService = settingsService;
            _dialogService = dialogService;

            CreateCommand = ReactiveCommand.Create(CreateCounterparty);
            EditCommand = ReactiveCommand.Create<CounterpartyDto>(EditCounterparty);
            DeleteCommand = ReactiveCommand.CreateFromTask<CounterpartyDto>(DeleteCounterpartyAsync);
        }

        private void CreateCounterparty()
        {
            var currentFirmId = _settingsService.CurrentFirmId;
            if (currentFirmId == null)
            {
                _dialogService.ShowMessageAsync("Сначала выберите рабочую фирму в настройках.", "Внимание", UserMessageType.Warning);
                return;
            }

            try
            {
                var param = new EditorParams { Mode = EditorMode.Create, ParentId = currentFirmId.Value };
                _navigation.NavigateTo<CounterpartyEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при добавлении контрагентов!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void EditCounterparty(CounterpartyDto counterparty)
        {
            if (counterparty == null) return;

            try
            {
                var param = new EditorParams { Mode = EditorMode.Edit, Id = counterparty.Id };
                _navigation.NavigateTo<CounterpartyEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при обновлении контрагентов!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private async Task DeleteCounterpartyAsync(CounterpartyDto counterparty)
        {
            bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить контрагента \"{counterparty.ShortName}\"?\nЭто действие нельзя отменить.",
                "Подтверждение удаления");

            if (!isConfirmed) return;

            try
            {
                await _counterpartyService.DeleteCounterpartyAsync(counterparty.Id);
                Items.Remove(counterparty);
                await _dialogService.ShowMessageAsync($"Контрагент \"{counterparty.ShortName}\" успешно удален.", "Успех", UserMessageType.Info);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении контрагента.");
                await _dialogService.ShowMessageAsync("Не удалось удалить контрагента.", "Ошибка", UserMessageType.Error);
            }
        }

        protected override async Task RefreshListAsync()
        {
            var firmId = _settingsService.CurrentFirmId;
            if (firmId == null)
            {
                Items.Clear();
                return;
            }

            IsBusy = true;
            try
            {
                var data = await _counterpartyService.GetCounterpartiesByFirmIdAsync(firmId.Value);
                
                Items.Clear();
                if (data != null)
                    foreach (var item in data)
                        Items.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке контрагентов!", 
                    "Ошибка", UserMessageType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
