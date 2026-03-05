namespace ContractCreator.UI.ViewModels.UserControls
{
    public class AttachedFilesViewModel : ViewModelBase
    {
        #region Props
        private readonly IFileService _fileService;
        private readonly IUserDialogService _dialogService;
        private readonly List<int> _filesToDelete = new();
        private readonly List<int> _newlyUploadedFileIds = new();

        public FileType CurrentFileType { get; set; }

        public sealed class AttachedFileModel : ReactiveObject
        {
            public int FileId { get; set; }
            public string? LocalFilePath { get; set; }
            public bool IsPendingUpload => FileId == 0;
            public string DisplayStatus => IsPendingUpload ? "(Новый)" :
                                           IsPendingUpdate ? "(Изменен)" : "(Загружен)";

            [Reactive] public bool IsPendingUpdate { get; set; }
            [Reactive] public string FileName { get; set; } = string.Empty;
            [Reactive] public bool IsEncrypted { get; set; }
            [Reactive] public string? Description { get; set; }
        }

        public ObservableCollection<AttachedFileModel> Files { get; } = new();
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> AddFileCommand { get; }
        public ReactiveCommand<AttachedFileModel, Unit> RemoveFileCommand { get; }
        public ReactiveCommand<AttachedFileModel, Unit> OpenFileCommand { get; }
        public ReactiveCommand<AttachedFileModel, Unit> DownloadFileCommand { get; }
        #endregion

        public AttachedFilesViewModel(
            IFileService fileService, 
            IUserDialogService userDialogService,
            FileType fileType = FileType.None)
        {
            _fileService = fileService;
            _dialogService = userDialogService;
            CurrentFileType = fileType;

            AddFileCommand = ReactiveCommand.CreateFromTask(AddFileAsync);
            RemoveFileCommand = ReactiveCommand.Create<AttachedFileModel>(RemoveFile);
            OpenFileCommand = ReactiveCommand.CreateFromTask<AttachedFileModel>(OpenFileAsync);
            DownloadFileCommand = ReactiveCommand.CreateFromTask<AttachedFileModel>(DownloadFileAsync);
        }

        private async Task AddFileAsync()
        {
            var selectedFiles = await FileHelper.PickFilesAsync("Выберите файлы", allowMultiple: true);
            if (selectedFiles == null) return;

            try
            {
                foreach (var file in selectedFiles)
                {
                    if (Files.Any(f => f.LocalFilePath == file.LocalPath)) continue;
                    Files.Add(new AttachedFileModel { FileId = 0, FileName = file.Name, LocalFilePath = file.LocalPath, IsEncrypted = true });
                    Log.Information("Файл добавлен в очередь на загрузку: {FileName}", file.Name);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при добавлении файлов в список. Было выбрано: {Count} шт.", selectedFiles.Count());
                await _dialogService.ShowMessageAsync("Не удалось добавить файл.", "Ошибка", UserMessageType.Error);
            }
        }

        private void RemoveFile(AttachedFileModel file)
        {
            Files.Remove(file);
            Log.Information("Файл удален из списка: {FileName} (ID: {FileId})", file.FileName, file.FileId);

            if (!file.IsPendingUpload && file.FileId > 0)
                _filesToDelete.Add(file.FileId);
        }

        private async Task OpenFileAsync(AttachedFileModel file)
        {
            string targetPath;

            try
            {
                if (file.IsPendingUpload || file.IsPendingUpdate)
                {
                    targetPath = file.LocalFilePath!;
                }
                else
                {
                    var data = await _fileService.DownloadFileAsync(file.FileId);
                    if (data == null)
                    {
                        Log.Error("Не удалось получить данные файла ID: {FileId} из хранилища.", file.FileId);
                        return;
                    }

                    targetPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{data.FileName}"); // Создаем временный файл в расшифрованном виде
                    await File.WriteAllBytesAsync(targetPath, data.Content);
                }

                var beforeModified = File.GetLastWriteTimeUtc(targetPath);

                var psi = new ProcessStartInfo
                {
                    FileName = targetPath,
                    UseShellExecute = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    await process.WaitForExitAsync();

                    var afterModified = File.GetLastWriteTimeUtc(targetPath); // Проверка изменений

                    if (afterModified > beforeModified) // При вызове CommitAsync сервис зашифрует его заново перед записью в хранилище.
                    {
                        file.LocalFilePath = targetPath;

                        if (!file.IsPendingUpload)
                            file.IsPendingUpdate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при открытии файла {FileName}", file.FileName);
                await _dialogService.ShowMessageAsync("Не удалось открыть файл для редактирования.", "Ошибка", UserMessageType.Error);
            }
        }

        /// <summary> Загружает данные о существующих файлах </summary>
        public async Task LoadExistingFilesAsync(List<EntityFileDto> existingFiles)
        {
            Files.Clear();
            _filesToDelete.Clear();
            _newlyUploadedFileIds.Clear();

            if (existingFiles == null || !existingFiles.Any()) return;

            try
            {
                var fileIds = existingFiles.Select(f => f.FileId).ToList();
                var fileInfos = await _fileService.GetFilesByIdsAsync(fileIds);

                foreach (var info in fileInfos)
                {
                    var description = existingFiles.FirstOrDefault(x => x.FileId == info.FileId)?.Description;

                    Files.Add(new AttachedFileModel
                    {
                        FileId = info.FileId,
                        FileName = info.FileName,
                        IsEncrypted = info.IsEncrypted,
                        Description = description
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке информации о существующих файлах.");
                await _dialogService.ShowMessageAsync("Не удалось загрузить файлы.", "Ошибка", UserMessageType.Error);
            }
        }

        private async Task DownloadFileAsync(AttachedFileModel file)
        {
            if (file.IsPendingUpload || file.IsPendingUpdate) return;

            try
            {
                var data = await _fileService.DownloadFileAsync(file.FileId);
                if (data == null) return;

                var savePath = await FileHelper.SaveFileAsync(data.FileName);
                if (savePath != null)
                {
                    await File.WriteAllBytesAsync(savePath, data.Content);
                    Log.Information("Файл {FileName} (ID: {FileId}) успешно скачан на диск: {SavePath}", file.FileName, file.FileId, savePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при скачивании файла {FileName}", file.FileName);
                await _dialogService.ShowMessageAsync("Не удалось скачать файл.", "Ошибка", UserMessageType.Error);
            }
        }

        /// <summary>
        /// Физически загружает новые файлы и удаляет старые. <br/>
        /// Возвращает список актуальных ID файлов для привязки к сущности.
        /// </summary>
        public async Task<List<int>> CommitAsync()
        {
            try
            {
                _newlyUploadedFileIds.Clear();

                if (_filesToDelete.Any())
                {
                    await _fileService.DeleteFilesByIdsAsync(_filesToDelete);
                    _filesToDelete.Clear();
                }

                var finalFileIds = new List<int>();

                foreach (var file in Files)
                {
                    if (file.IsPendingUpload && !string.IsNullOrEmpty(file.LocalFilePath))
                    {
                        using var stream = File.OpenRead(file.LocalFilePath);
                        int newId = file.IsEncrypted
                            ? await _fileService.UploadEncryptedFileAsync(stream, CurrentFileType, file.FileName, DateTime.Now)
                            : await _fileService.UploadFileAsync(stream, CurrentFileType, file.FileName, DateTime.Now);

                        _newlyUploadedFileIds.Add(newId);
                        file.FileId = newId;
                        file.LocalFilePath = null;
                        finalFileIds.Add(newId);
                    }
                    else if (file.IsPendingUpdate && !string.IsNullOrEmpty(file.LocalFilePath))
                    {
                        using var stream = File.OpenRead(file.LocalFilePath);

                        if (file.IsEncrypted)
                            await _fileService.UpdateEncryptedFileAsync(file.FileId, stream, file.FileName, DateTime.Now);
                        else
                            await _fileService.UpdateFileAsync(file.FileId, stream, file.FileName, DateTime.Now);

                        file.IsPendingUpdate = false;
                        file.LocalFilePath = null;
                        finalFileIds.Add(file.FileId);
                    }
                    else
                        finalFileIds.Add(file.FileId);
                }

                return finalFileIds;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Непредвиденная ошибка при сохранении/обновлении файлов (Commit).");
                await _dialogService.ShowMessageAsync("Не удалось сохранить файл.", "Ошибка", UserMessageType.Error);
                return new List<int>();
            }
        }

        public async Task<List<EntityFileDto>> GetFilesForCommitAsync(int entityId)
        {
            var fileIds = await CommitAsync();

            return Files.Select(f => new EntityFileDto
            {
                EntityId = entityId,
                FileId = f.FileId,
                Description = f.Description
            }).ToList();
        }

        public async Task RollbackCommitAsync()
        {
            if (_newlyUploadedFileIds.Any())
            {
                await _fileService.DeleteFilesByIdsAsync(_newlyUploadedFileIds);
                _newlyUploadedFileIds.Clear();
            }
        }
    }
}
