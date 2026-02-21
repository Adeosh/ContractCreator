namespace ContractCreator.UI.ViewModels.Products
{
    public class ProductListViewModel : EntityListViewModel<GoodsAndServiceDto>
    {
        #region Props
        private readonly IProductService _productService;
        private readonly INavigationService _navigation;
        private readonly IUserDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        [Reactive] public GoodsAndServiceDto? SelectedProduct { get; set; }
        [Reactive] public ProductType CurrentListType { get; set; }

        public string PageTitle => CurrentListType == ProductType.Good ? "Список товаров" : "Список услуг";
        public string AddButtonText => CurrentListType == ProductType.Good ? "Добавить товар" : "Добавить услугу";
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<GoodsAndServiceDto, Unit> EditCommand { get; }
        public ReactiveCommand<GoodsAndServiceDto, Unit> DeleteCommand { get; }
        #endregion

        public ProductListViewModel(
            IProductService productService,
            INavigationService navigation,
            IUserDialogService dialogService,
            ISettingsService settingsService)
        {
            _productService = productService;
            _navigation = navigation;
            _dialogService = dialogService;
            _settingsService = settingsService;

            CreateCommand = ReactiveCommand.Create(CreateProduct);
            EditCommand = ReactiveCommand.Create<GoodsAndServiceDto>(EditProduct);
            DeleteCommand = ReactiveCommand.CreateFromTask<GoodsAndServiceDto>(DeleteProductAsync);
        }

        public override async Task OnNavigatedToAsync(object? parameter = null)
        {
            if (parameter is ProductType type)
            {
                CurrentListType = type;

                this.RaisePropertyChanged(nameof(PageTitle));
                this.RaisePropertyChanged(nameof(AddButtonText));
            }

            await RefreshListAsync();
        }

        protected override async Task RefreshListAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var currentFirmId = _settingsService.CurrentFirmId;
                Items.Clear();

                if (currentFirmId == null)
                    return;

                var allData = await _productService.GetAllAsync();
                var filteredData = allData.Where(x => x.Type == CurrentListType && x.FirmId == currentFirmId.Value);

                foreach (var item in filteredData)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                await _dialogService.ShowMessageAsync("Не удалось загрузить данные.", "Ошибка", UserMessageType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CreateProduct()
        {
            try
            {
                var param = new EditorParams { Mode = EditorMode.Create, Payload = CurrentListType };
                _navigation.NavigateTo<ProductEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при сохранении товара или услуги!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void EditProduct(GoodsAndServiceDto product)
        {
            if (product == null) return;

            try
            {
                var param = new EditorParams { Mode = EditorMode.Edit, Id = product.Id, Payload = CurrentListType };
                _navigation.NavigateTo<ProductEditorViewModel>(param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при обновлении товара или услуги!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private async Task DeleteProductAsync(GoodsAndServiceDto product)
        {
            if (product == null) return;

            string typeName = CurrentListType == ProductType.Good ? "товар" : "услугу";
            bool confirm = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить {typeName} '{product.Name}'?", "Удаление");

            if (!confirm) return;

            try
            {
                await _productService.DeleteAsync(product.Id);
                Items.Remove(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                await _dialogService.ShowMessageAsync("Ошибка", "Не удалось удалить запись.", UserMessageType.Error);
            }
        }
    }
}
