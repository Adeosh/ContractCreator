using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaApp = Avalonia.Application;

namespace ContractCreator.UI.Helpers
{
    public static class FileHelper
    {
        public static async Task<(string FileName, byte[] Data)?> PickImageAsync(string title = "Выберите изображение")
        {
            var mainWindow = (AvaloniaApp.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (mainWindow == null) return null;

            var topLevel = TopLevel.GetTopLevel(mainWindow);
            if (topLevel == null) return null;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            if (files.Count >= 1)
            {
                var file = files[0];
                var name = file.Name;

                await using var stream = await file.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                return (name, memoryStream.ToArray());
            }

            return null;
        }
    }
}
