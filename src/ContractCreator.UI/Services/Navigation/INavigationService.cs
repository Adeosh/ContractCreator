namespace ContractCreator.UI.Services.Navigation
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }

        event Action<ViewModelBase> CurrentViewChanged;
        void NavigateTo<T>() where T : ViewModelBase;
        void NavigateTo<T>(object parameter) where T : ViewModelBase;
        void NavigateTo(Type viewModelType);
        void NavigateBack();
    }
}
