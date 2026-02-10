using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
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
                var viewModel = CreateViewModel(title, message, type);

                viewModel.IsConfirmation = false;
                viewModel.ConfirmText = "OK";

                var view = new UnifiedDialogView { DataContext = viewModel };
                await ShowWindow(view);
            });
        }

        public async Task<bool> ShowConfirmationAsync(string message, string title = "Подтверждение")
        {
            return await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var viewModel = CreateViewModel(title, message, UserMessageType.Warning);

                viewModel.IsConfirmation = true;
                viewModel.ConfirmText = "Да";
                viewModel.CancelText = "Нет";

                var view = new UnifiedDialogView { DataContext = viewModel };
                await ShowWindow(view);

                return viewModel.DialogResult;
            });
        }

        private UnifiedDialogViewModel CreateViewModel(string title, string message, UserMessageType type)
        {
            var vm = new UnifiedDialogViewModel(title, message);

            switch (type)
            {
                case UserMessageType.Info:
                    vm.HeaderColor = SolidColorBrush.Parse("#62bed9");
                    vm.Icon = "ℹ️";
                    break;
                case UserMessageType.Warning:
                    vm.HeaderColor = SolidColorBrush.Parse("#ffe169");
                    vm.Icon = "⚠️";
                    break;
                case UserMessageType.Error:
                    vm.HeaderColor = SolidColorBrush.Parse("#ef233c");
                    vm.Icon = "⛔";
                    break;
            }

            return vm;
        }

        private async Task ShowWindow(Window view)
        {
            if (AvaloniaApp.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var owner = desktop.MainWindow;

                if (owner != null && owner.IsVisible)
                    await view.ShowDialog(owner);
                else
                    view.Show();
            }
        }
    }
}
