namespace ContractCreator.UI.ViewModels.Base
{
    public abstract class EntityListViewModel<TDto> : ViewModelBase, INavigatedAware
    {
        [Reactive] public bool IsBusy { get; set; }
        public ObservableCollection<TDto> Items { get; } = new();

        protected abstract Task RefreshListAsync();

        public virtual async Task OnNavigatedToAsync(object? parameter = null) => await RefreshListAsync();

        public virtual Task OnNavigatedFromAsync() => Task.CompletedTask;
    }
}
