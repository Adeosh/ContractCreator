namespace ContractCreator.UI.ViewModels.Contracts
{
    public class ContractListViewModel : EntityListViewModel<ContractDto>
    {
        #region Props
        private readonly IContractService _contractService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        [Reactive] public ContractDto? SelectedContract { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<ContractDto, Unit> EditCommand { get; }
        public ReactiveCommand<ContractDto, Unit> DeleteCommand { get; }
        #endregion

        public ContractListViewModel(
            IContractService contractService,
            ISettingsService settingsService,
            INavigationService navigation,
            IUserDialogService dialogService)
        {
            _contractService = contractService;
            _settingsService = settingsService;
            _navigation = navigation;
            _dialogService = dialogService;

            CreateCommand = ReactiveCommand.Create(CreateContract);
            EditCommand = ReactiveCommand.Create<ContractDto>(EditContract);
            DeleteCommand = ReactiveCommand.CreateFromTask<ContractDto>(DeleteContractAsync);
        }

        protected override async Task RefreshListAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var currentFirmId = _settingsService.CurrentFirmId;
                if (currentFirmId == null)
                {
                    Items.Clear();
                    return;
                }

                var data = await _contractService.GetContractsByFirmIdAsync(currentFirmId.Value);

                Items.Clear();
                foreach (var item in data)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке контрактов");
                await _dialogService.ShowMessageAsync("Не удалось загрузить список контрактов.", "Ошибка", UserMessageType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CreateContract()
        {
            var currentFirmId = _settingsService.CurrentFirmId;
            if (currentFirmId == null)
            {
                _dialogService.ShowMessageAsync("Сначала выберите рабочую фирму в настройках.", "Внимание", UserMessageType.Warning);
                return;
            }

            var param = new EditorParams { Mode = EditorMode.Create, ParentId = currentFirmId.Value };
            _navigation.NavigateTo<ContractEditorViewModel>(param);
        }

        private void EditContract(ContractDto contract)
        {
            if (contract == null) return;
            var param = new EditorParams { Mode = EditorMode.Edit, Id = contract.Id };
            _navigation.NavigateTo<ContractEditorViewModel>(param);
        }

        private async Task DeleteContractAsync(ContractDto contract)
        {
            if (contract == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить документ № {contract.ContractNumber}?", "Удаление");

            if (!confirm) return;

            try
            {
                await _contractService.DeleteContractAsync(contract.Id);
                Items.Remove(contract);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка удаления контракта");
                await _dialogService.ShowMessageAsync("Не удалось удалить документ.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
