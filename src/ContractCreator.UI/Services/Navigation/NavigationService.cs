using Microsoft.Extensions.DependencyInjection;

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

        public void NavigateTo<T>() where T : ViewModelBase
        {
            if (_currentView != null)
                _history.Push(_currentView);

            var viewModel = _serviceProvider.GetRequiredService<T>();
            CurrentView = viewModel;
        }

        public void NavigateTo<T>(object parameter) where T : ViewModelBase
        {
            if (_currentView != null)
                _history.Push(_currentView);

            var viewModel = _serviceProvider.GetRequiredService<T>();
            if (viewModel is IParametrizedViewModel parametrizedVm)
                parametrizedVm.ApplyParameterAsync(parameter).SafeFireAndForget();

            CurrentView = viewModel;
        }

        public void NavigateTo(Type viewModelType)
        {
            if (_currentView != null)
                _history.Push(_currentView);

            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);
            CurrentView = viewModel;
        }

        public void NavigateBack()
        {
            if (_history.Count > 0)
                CurrentView = _history.Pop();
        }
    }
}
