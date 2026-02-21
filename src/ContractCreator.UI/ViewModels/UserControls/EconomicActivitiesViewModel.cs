namespace ContractCreator.UI.ViewModels.UserControls
{
    public class EconomicActivitiesViewModel : ViewModelBase
    {
        #region Props
        private readonly IClassifierService _classifierService;
        private List<ClassifierDto> _allOkvedsCache = new();

        [Reactive] public string SearchText { get; set; } = "";

        public ObservableCollection<ClassifierDto> FilteredAvailableActivities { get; } = new();
        [Reactive] public ClassifierDto? SelectedAvailableItem { get; set; }

        public ObservableCollection<FirmEconomicActivityDto> SelectedActivities { get; } = new();
        [Reactive] public FirmEconomicActivityDto? SelectedChosenItem { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<ClassifierDto, Unit> AddCommand { get; }
        public ReactiveCommand<FirmEconomicActivityDto, Unit> RemoveCommand { get; }
        public ReactiveCommand<FirmEconomicActivityDto, Unit> SetMainCommand { get; }
        #endregion

        public EconomicActivitiesViewModel(IClassifierService classifierService)
        {
            _classifierService = classifierService;

            SetupActivities();

            AddCommand = ReactiveCommand.Create<ClassifierDto>(AddActivity);
            RemoveCommand = ReactiveCommand.Create<FirmEconomicActivityDto>(RemoveActivity);
            SetMainCommand = ReactiveCommand.Create<FirmEconomicActivityDto>(SetMainActivity);
        }

        private void SetupActivities()
        {
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.TaskpoolScheduler)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => ApplyFilter());
        }

        private void AddActivity(ClassifierDto dto)
        {
            if (dto == null) return;
            if (SelectedActivities.Any(x => x.EconomicActivityId == dto.Id)) return;

            var newActivity = new FirmEconomicActivityDto
            {
                EconomicActivityId = dto.Id,
                Code = dto.Code,
                Name = dto.Name,
                IsMain = !SelectedActivities.Any()
            };

            SelectedActivities.Add(newActivity);
        }

        private void RemoveActivity(FirmEconomicActivityDto item)
        {
            if (item == null) return;

            SelectedActivities.Remove(item);

            if (item.IsMain && SelectedActivities.Any())
            {
                var first = SelectedActivities.First();
                first.IsMain = true;

                var idx = SelectedActivities.IndexOf(first);
                SelectedActivities[idx] = first;
            }
        }

        private void SetMainActivity(FirmEconomicActivityDto item)
        {
            if (item == null || item.IsMain) return;

            foreach (var act in SelectedActivities)
                act.IsMain = (act == item);

            var list = SelectedActivities.ToList();
            SelectedActivities.Clear();

            foreach (var i in list)
                SelectedActivities.Add(i);
        }

        public async Task LoadDictionaryAsync()
        {
            try
            {
                _allOkvedsCache = await _classifierService.GetAllOkvedsAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка загрузки справочника ОКВЭД!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private void ApplyFilter()
        {
            FilteredAvailableActivities.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var item in _allOkvedsCache)
                    FilteredAvailableActivities.Add(item);

                return;
            }

            var searchText = SearchText.Trim();

            var query = _allOkvedsCache
                .Where(a => a.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                         || a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a =>
                {
                    // Приоритет 0: Код начинается с введенного текста (самое важное)
                    if (a.Code.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                        return 0;

                    // Приоритет 1: Название начинается с введенного текста
                    if (a.Name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                        return 1;

                    // Приоритет 2: Текст где-то в середине
                    return 2;
                })
                .ThenBy(a => a.Code)
                .ThenBy(a => a.Name);

            foreach (var item in query)
                FilteredAvailableActivities.Add(item);
        }

        public void SetData(IEnumerable<FirmEconomicActivityDto> activities)
        {
            SelectedActivities.Clear();
            foreach (var item in activities) SelectedActivities.Add(item);
            ApplyFilter();
        }

        public List<FirmEconomicActivityDto> GetData() => SelectedActivities.ToList();
    }
}
