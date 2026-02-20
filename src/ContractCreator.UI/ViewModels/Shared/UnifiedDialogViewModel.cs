namespace ContractCreator.UI.ViewModels.Shared
{
    public class UnifiedDialogViewModel : ViewModelBase
    {
        #region Props
        public string Title { get; }
        public string Message { get; }
        public string Icon { get; set; } = "ℹ️";
        public IBrush HeaderColor { get; set; } = Brushes.LightGray;
        public string ConfirmText { get; set; } = "OK";
        public string CancelText { get; set; } = "Отмена";
        public bool DialogResult { get; private set; } = false;
        public Action? OnCloseRequest { get; set; }

        [Reactive] public bool IsConfirmation { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<object, Unit> CloseCommand { get; }
        #endregion

        public UnifiedDialogViewModel(string message, string title)
        {
            Message = message;
            Title = title;

            CloseCommand = ReactiveCommand.Create<object>(parameter =>
            {
                var result = Convert.ToBoolean(parameter);

                DialogResult = result;
                OnCloseRequest?.Invoke();
            });
        }
    }
}
