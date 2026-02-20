namespace ContractCreator.UI.ViewModels.Shared
{
    public class MenuItemViewModel : ReactiveObject
    {
        public string Header { get; set; }
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }

        public ObservableCollection<MenuItemViewModel> Items { get; set; } = new();

        public MenuItemViewModel(string header, ICommand? command = null, object? parameter = null)
        {
            Header = header;
            Command = command;
            CommandParameter = parameter;
        }
    }
}
