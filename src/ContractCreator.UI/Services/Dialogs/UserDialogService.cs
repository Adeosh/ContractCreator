using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using AvaloniaApp = Avalonia.Application;

namespace ContractCreator.UI.Services.Dialogs
{
    public class UserDialogService : IUserDialogService
    {
        public Task ShowErrorAsync(string message, string title = "Ошибка")
        {
            return ShowMessageAsync(message, title, UserMessageType.Error);
        }

        public async Task ShowMessageAsync(string message, string title, UserMessageType type)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var viewModel = new ErrorDialogViewModel(message, title);

                switch (type)
                {
                    case UserMessageType.Info:
                        viewModel.HeaderColor = "#2196F3";
                        viewModel.Icon = "ℹ️";
                        break;
                    case UserMessageType.Warning:
                        viewModel.HeaderColor = "#FF9800";
                        viewModel.Icon = "⚠️";
                        break;
                    case UserMessageType.Error:
                        viewModel.HeaderColor = "#F44336";
                        viewModel.Icon = "⛔";
                        break;
                }

                var view = new ErrorDialogView { DataContext = viewModel };

                if (AvaloniaApp.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var owner = desktop.MainWindow;

                    if (owner != null && owner.IsVisible)
                        await view.ShowDialog(owner);
                    else
                        view.Show();
                }
            });
        }
    }
}
