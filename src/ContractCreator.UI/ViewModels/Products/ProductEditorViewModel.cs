namespace ContractCreator.UI.ViewModels.Products
{
    public class ProductEditorViewModel : ViewModelBase, INavigatedAware
    {
        #region Props
        private readonly IProductService _productService;
        private readonly IClassifierService _classifierService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        [Reactive] public int Id { get; set; }
        [Reactive] public ProductType CurrentType { get; set; }
        [Reactive] public string Name { get; set; } = string.Empty;
        [Reactive] public string? Description { get; set; }
        [Reactive] public string? UnitOfMeasure { get; set; }
        [Reactive] public decimal Price { get; set; }
        [Reactive] public int FirmId { get; set; }

        public ObservableCollection<ClassifierDto> Currencies { get; } = new();
        [Reactive] public ClassifierDto? SelectedCurrency { get; set; }

        public string PageTitle => CurrentType == ProductType.Good ? "Карточка товара" : "Карточка услуги";
        public bool IsUnitOfMeasureVisible => CurrentType == ProductType.Good; // Для услуг единица измерения часто не нужна, но можно оставить
        public string NameWatermark => CurrentType == ProductType.Good ? "Например: Кирпич строительный" : "Например: Консультация юриста";
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        #endregion

        public ProductEditorViewModel(
            IProductService productService,
            IClassifierService classifierService,
            INavigationService navigation,
            IUserDialogService dialogService,
            ISettingsService settingsService)
        {
            _productService = productService;
            _classifierService = classifierService;
            _navigation = navigation;
            _dialogService = dialogService;
            _settingsService = settingsService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
        }

        public async Task OnNavigatedToAsync(object? parameter = null)
        {
            await LoadCurrenciesAsync();

            if (parameter is EditorParams param)
            {
                if (param.Payload is ProductType type)
                    CurrentType = type;

                if (param.Mode == EditorMode.Edit)
                {
                    await LoadDataAsync(param.Id);
                }
                else
                {
                    var currentFirmId = _settingsService.CurrentFirmId;
                    if (currentFirmId == null)
                    {
                        await _dialogService.ShowMessageAsync("Не выбрана активная фирма в настройках.", "Ошибка", UserMessageType.Error);
                        _navigation.NavigateBack();
                        return;
                    }
                    FirmId = currentFirmId.Value;
                    SelectedCurrency = Currencies.FirstOrDefault(c => c.Id == ClassifierOkv.RUB) ?? Currencies.FirstOrDefault();
                }

                this.RaisePropertyChanged(nameof(PageTitle));
                this.RaisePropertyChanged(nameof(IsUnitOfMeasureVisible));
                this.RaisePropertyChanged(nameof(NameWatermark));
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadCurrenciesAsync()
        {
            try
            {
                var list = await _classifierService.GetCurrenciesAsync();

                Currencies.Clear();
                foreach (var c in list) 
                    Currencies.Add(c);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                await _dialogService.ShowMessageAsync("Ошибка", "Не удалось загрузить справочник валют.", UserMessageType.Error);
            }
        }

        private async Task LoadDataAsync(int id)
        {
            try
            {
                var dto = await _productService.GetByIdAsync(id);
                if (dto != null)
                {
                    Id = dto.Id;
                    CurrentType = dto.Type;
                    Name = dto.Name;
                    Description = dto.Description;
                    UnitOfMeasure = dto.UnitOfMeasure;
                    Price = dto.Price;
                    FirmId = dto.FirmId;

                    SelectedCurrency = Currencies.FirstOrDefault(c => c.Id == dto.CurrencyId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                await _dialogService.ShowMessageAsync("Не удалось загрузить данные.", "Ошибка", UserMessageType.Error);
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new UserMessageException("Наименование обязательно для заполнения.");

            if (Price < 0)
                throw new UserMessageException("Цена не может быть отрицательной.");

            if (SelectedCurrency == null)
                throw new UserMessageException("Необходимо выбрать валюту.");
        }

        private async Task SaveAsync()
        {
            try
            {
                Validate();

                var dto = new GoodsAndServiceDto
                {
                    Id = Id,
                    Type = CurrentType,
                    Name = Name,
                    Description = Description,
                    UnitOfMeasure = UnitOfMeasure,
                    Price = Price,
                    CurrencyId = SelectedCurrency!.Id,
                    FirmId = FirmId,
                    IsDeleted = false
                };

                if (Id == 0)
                    await _productService.CreateAsync(dto);
                else
                    await _productService.UpdateAsync(dto);

                _navigation.NavigateBack();
            }
            catch (UserMessageException ex)
            {
                await _dialogService.ShowMessageAsync(ex.Title, ex.Message, UserMessageType.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                await _dialogService.ShowMessageAsync("Произошла ошибка при сохранении.", "Ошибка", UserMessageType.Error);
            }
        }
    }
}
