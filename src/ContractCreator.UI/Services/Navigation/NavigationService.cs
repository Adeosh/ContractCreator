namespace ContractCreator.UI.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private ViewModelBase _currentView;
        private readonly Stack<ViewModelBase> _history = new();

        public ViewModelBase CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                CurrentViewChanged?.Invoke(_currentView);
            }
        }

        public event Action<ViewModelBase>? CurrentViewChanged;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<T>() where T : ViewModelBase => NavigateToInternal(typeof(T), null);

        public void NavigateTo<T>(object parameter) where T : ViewModelBase => NavigateToInternal(typeof(T), parameter);

        public void NavigateTo(Type viewModelType) => NavigateToInternal(viewModelType, null);

        private void NavigateToInternal(Type viewModelType, object? parameter)
        {
            if (_currentView is INavigatedAware oldView) // Уведомляем текущую страницу, что мы с нее уходим
                oldView.OnNavigatedFromAsync().SafeFireAndForget();

            if (_currentView != null)
                _history.Push(_currentView);

            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType); // Создаем новую страницу
            CurrentView = viewModel;

            if (CurrentView is INavigatedAware newView) // Уведомляем новую страницу, что мы на нее пришли
                newView.OnNavigatedToAsync(parameter).SafeFireAndForget();
        }

        public void NavigateBack()
        {
            if (_history.Count > 0)
            {
                if (_currentView is INavigatedAware oldView)
                    oldView.OnNavigatedFromAsync().SafeFireAndForget();

                CurrentView = _history.Pop();

                if (CurrentView is INavigatedAware returnedView)
                    returnedView.OnNavigatedToAsync(null).SafeFireAndForget();
            }
        }
    }
}
