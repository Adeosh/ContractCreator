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

        public static async Task<IEnumerable<(string Name, string LocalPath)>?> PickFilesAsync(
            string title = "Выберите файлы",
            bool allowMultiple = true)
        {
            var mainWindow = (AvaloniaApp.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow == null) return null;

            var topLevel = TopLevel.GetTopLevel(mainWindow);
            if (topLevel == null) return null;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = allowMultiple,
                FileTypeFilter = new[] { FilePickerFileTypes.All }
            });

            if (files.Count > 0)
                return files.Select(f => (f.Name, f.Path.LocalPath));

            return null;
        }

        public static async Task<string?> SaveFileAsync(string defaultFileName, string title = "Сохранить файл как...")
        {
            var mainWindow = (AvaloniaApp.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow == null) return null;

            var topLevel = TopLevel.GetTopLevel(mainWindow);
            if (topLevel == null) return null;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                SuggestedFileName = defaultFileName
            });

            return file?.Path.LocalPath;
        }
    }
}
