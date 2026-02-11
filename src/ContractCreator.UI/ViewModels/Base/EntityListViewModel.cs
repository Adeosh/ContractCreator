namespace ContractCreator.UI.ViewModels.Base
{
    public abstract class EntityListViewModel<TDto> : ViewModelBase
    {
        [Reactive] public bool IsBusy { get; set; }
        public ObservableCollection<TDto> Items { get; } = new();

        protected EntityListViewModel()
        {
            MessageBus.Current.Listen<EntitySavedMessage<TDto>>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async _ => await RefreshListAsync());
        }

        protected abstract Task RefreshListAsync();
    }
}
