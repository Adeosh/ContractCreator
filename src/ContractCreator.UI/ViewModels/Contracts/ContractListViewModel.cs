namespace ContractCreator.UI.ViewModels.Contracts
{
    public class ContractListViewModel : EntityListViewModel<ContractDto>
    {
        #region Props
        private readonly IContractService _contractService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;

        public string? PageTitle { get; set; }

        [Reactive] public ContractDto? SelectedContract { get; set; }
        [Reactive] public ContractListMode CurrentMode { get; set; } = ContractListMode.All;
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<ContractDto, Unit> EditCommand { get; }
        public ReactiveCommand<ContractDto, Unit> DeleteCommand { get; }
        public ReactiveCommand<ContractDto, Unit> OpenDocumentsWorkspaceCommand { get; }
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
            OpenDocumentsWorkspaceCommand = ReactiveCommand.Create<ContractDto>(OpenDocumentsWorkspace);
        }

        public override async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is ContractListMode mode)
            {
                CurrentMode = mode;
                PageTitle = mode switch
                {
                    ContractListMode.Execution => "Работа с документами (На исполнении)",
                    ContractListMode.Archive => "Архив документов",
                    _ => "Договоры и контракты (Все)"
                };
            }
            await RefreshListAsync();
        }

        private void OpenDocumentsWorkspace(ContractDto contract)
        {
            if (contract == null) return;

            _navigation.NavigateTo<ContractDocumentsViewModel>(contract.Id);
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
                IEnumerable<ContractDto> filteredData = data;

                if (CurrentMode == ContractListMode.Execution)
                {
                    filteredData = data.Where(c =>
                        c.StageTypeId == (int)ContractStageType.Execution ||
                        c.StageTypeId == (int)ContractStageType.Concluded);
                }
                else if (CurrentMode == ContractListMode.Archive)
                {
                    filteredData = data.Where(c =>
                        c.StageTypeId == (int)ContractStageType.Paid ||
                        c.StageTypeId == (int)ContractStageType.Executed ||
                        c.StageTypeId == (int)ContractStageType.Terminated);
                }

                Items.Clear();
                foreach (var item in filteredData)
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
