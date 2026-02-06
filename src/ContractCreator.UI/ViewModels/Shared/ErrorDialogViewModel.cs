namespace ContractCreator.UI.ViewModels.Shared
{
    public class ErrorDialogViewModel : ViewModelBase
    {
        [Reactive] public string Title { get; set; } = "Ошибка";
        [Reactive] public string Message { get; set; } = "";
        [Reactive] public string HeaderColor { get; set; }
        [Reactive] public string Icon { get; set; }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public event Action? RequestClose;

        public ErrorDialogViewModel(string message, string title = "Ошибка")
        {
            Message = message;
            Title = title;

            CloseCommand = ReactiveCommand.Create(() =>
            {
                RequestClose?.Invoke();
            });
        }
    }
}
