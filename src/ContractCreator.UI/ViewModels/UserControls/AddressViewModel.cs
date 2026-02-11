namespace ContractCreator.UI.ViewModels.UserControls
{
    public class AddressViewModel : ViewModelBase
    {
        #region Props
        private readonly IGarService _garService;
        private bool _suppressSearch;

        [Reactive] public string SearchText { get; set; } = string.Empty;
        [Reactive] public string House { get; set; } = string.Empty;
        [Reactive] public string Building { get; set; } = string.Empty;
        [Reactive] public string Flat { get; set; } = string.Empty;
        [Reactive] public string PostalIndex { get; set; } = string.Empty;

        [Reactive] public bool IsDropDownOpen { get; set; }
        [Reactive] public bool IsBusy { get; set; }
        [Reactive] public long CurrentObjectId { get; set; }

        public ObservableCollection<AddressSearchResultDto> SearchResults { get; } = new();
        #endregion

        public AddressViewModel(IGarService garService)
        {
            _garService = garService;
            SetupAddress();
        }

        private void SetupAddress()
        {
            this.WhenAnyValue(x => x.SearchText) // поиск
                .Skip(1)
                .Where(_ => !_suppressSearch)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.TaskpoolScheduler)
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term) && term.Length >= 2)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ => IsBusy = true)
                .SelectMany(async term =>
                {
                    try
                    {
                        return await _garService.SearchAddressAsync(term!, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Ошибка внутри потока поиска адреса");
                        return Enumerable.Empty<AddressSearchResultDto>();
                    }
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(results =>
                {
                    IsBusy = false;
                    SearchResults.Clear();

                    var list = results.ToList();
                    if (list.Any())
                    {
                        foreach (var item in list) SearchResults.Add(item);
                        IsDropDownOpen = true;
                    }
                    else
                        IsDropDownOpen = false;
                });

            this.WhenAnyValue(x => x.SearchText) // сброс
                .Where(_ => !_suppressSearch)
                .Subscribe(_ =>
                {
                    if (CurrentObjectId != 0)
                        CurrentObjectId = 0;

                    House = string.Empty;
                    Building = string.Empty;
                    Flat = string.Empty;
                    PostalIndex = string.Empty;

                    IsDropDownOpen = false;
                });
        }

        public void SelectAddress(AddressSearchResultDto selected)
        {
            if (selected == null) return;

            _suppressSearch = true;
            SearchText = selected.FullAddress;
            CurrentObjectId = selected.ObjectId;

            House = string.Empty;
            Building = string.Empty;
            Flat = string.Empty;
            PostalIndex = selected.PostalIndex ?? string.Empty;

            IsDropDownOpen = false;
            SearchResults.Clear();
            _suppressSearch = false;
        }

        public AddressDto GetData() => new AddressDto
        {
            ObjectId = CurrentObjectId,
            FullAddress = SearchText,
            House = House,
            Building = Building,
            Flat = Flat,
            PostalIndex = PostalIndex
        };

        public void SetData(AddressDto dto)
        {
            if (dto == null) return;

            _suppressSearch = true;
            CurrentObjectId = dto.ObjectId;
            SearchText = dto.FullAddress;

            House = dto.House;
            Building = dto.Building;
            Flat = dto.Flat;
            PostalIndex = dto.PostalIndex;

            _suppressSearch = false;
        }
    }
}
