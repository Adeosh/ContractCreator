namespace ContractCreator.UI.Services.Abstractions
{
    public interface IUserDialogService
    {
        Task ShowErrorAsync(string message, string title = "Ошибка");
        Task ShowMessageAsync(string message, string title, UserMessageType type);
        Task<bool> ShowConfirmationAsync(string message, string title = "Подтверждение");
    }
}
