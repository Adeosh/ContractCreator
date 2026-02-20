namespace ContractCreator.UI.Services.Abstractions
{
    public interface INavigatedAware
    {
        /// <summary> Вызывается при переходе НА эту страницу (и при возврате на нее тоже). </summary>
        Task OnNavigatedToAsync(object? parameter = null);

        /// <summary> Вызывается при уходе с этой страницы (можно использовать для отписки от событий). </summary>
        Task OnNavigatedFromAsync();
    }
}
