using ContractCreator.Application.Interfaces.Infrastructure;
using ContractCreator.Domain.Models;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Services.Files.Helpers;
using ContractCreator.Shared.DTOs.Data;
using ContractCreator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ContractCreator.Infrastructure.Services.Files
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _context;
        private readonly ISettingsService _settingsService;

        public FileService(AppDbContext context, ISettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        public async Task<int> UploadEncryptedFileAsync(Stream fileStream, FileType fileType, string fileName, DateTime uploadDate)
        {
            var fileGUID = Guid.NewGuid();
            await SavePhysicalFileAsync(fileStream, fileGUID, fileType, encrypt: true);

            var fileStorage = new FileStorage
            {
                StorageFileGuid = fileGUID,
                FileName = fileName,
                Type = fileType,
                UploadDate = DateTime.SpecifyKind(uploadDate, DateTimeKind.Utc),
                IsEncrypted = true
            };

            _context.Files.Add(fileStorage);
            await _context.SaveChangesAsync();

            return fileStorage.Id;
        }

        public async Task<int> UploadFileAsync(Stream fileStream, FileType fileType, string fileName, DateTime uploadDate)
        {
            var fileGUID = Guid.NewGuid();
            await SavePhysicalFileAsync(fileStream, fileGUID, fileType, encrypt: false);

            var fileStorage = new FileStorage
            {
                StorageFileGuid = fileGUID,
                FileName = fileName,
                Type = fileType,
                UploadDate = DateTime.SpecifyKind(uploadDate, DateTimeKind.Utc),
                IsEncrypted = false
            };

            _context.Files.Add(fileStorage);
            await _context.SaveChangesAsync();

            return fileStorage.Id;
        }

        public async Task<FileDataDto?> DownloadFileAsync(int fileId)
        {
            var file = await _context.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Id == fileId);
            if (file == null) return null;

            var filePath = GetPhysicalFilePath(file.Type, file.StorageFileGuid);
            if (!File.Exists(filePath)) return null;

            byte[] content = file.IsEncrypted
                ? await FileEncryptionHelper.DecryptFileToBytesAsync(filePath)
                : await File.ReadAllBytesAsync(filePath);

            return new FileDataDto
            {
                FileId = file.Id,
                FileName = file.FileName,
                Content = content,
                IsEncrypted = file.IsEncrypted
            };
        }

        public async Task<List<FileDataDto>> GetFileDataByIDsAsync(List<int> fileIds)
        {
            var files = await _context.Files.AsNoTracking().Where(f => fileIds.Contains(f.Id)).ToListAsync();
            var result = new List<FileDataDto>();

            foreach (var file in files)
            {
                var filePath = GetPhysicalFilePath(file.Type, file.StorageFileGuid);
                if (!File.Exists(filePath)) continue;

                byte[] content = file.IsEncrypted
                    ? await FileEncryptionHelper.DecryptFileToBytesAsync(filePath)
                    : await File.ReadAllBytesAsync(filePath);

                result.Add(new FileDataDto
                {
                    FileId = file.Id,
                    FileName = file.FileName,
                    Content = content,
                    IsEncrypted = file.IsEncrypted
                });
            }

            return result;
        }

        public async Task UpdateEncryptedFileAsync(int fileId, Stream fileStream, string fileName, DateTime changeDate)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null) 
                throw new InvalidOperationException("Файл не найден в базе данных");

            await SavePhysicalFileAsync(fileStream, file.StorageFileGuid, file.Type, encrypt: true);

            file.FileName = fileName;
            file.ChangeDate = DateTime.SpecifyKind(changeDate, DateTimeKind.Utc);

            _context.Files.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFileAsync(int fileId, Stream fileStream, string fileName, DateTime changeDate)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null) throw new InvalidOperationException("Файл не найден в базе данных");

            await SavePhysicalFileAsync(fileStream, file.StorageFileGuid, file.Type, encrypt: false);

            file.FileName = fileName;
            file.ChangeDate = DateTime.SpecifyKind(changeDate, DateTimeKind.Utc);

            _context.Files.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null) return false;

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            DeletePhysicalFile(file.Type, file.StorageFileGuid);
            return true;
        }

        public async Task DeleteFilesByIDsAsync(List<int> fileIds)
        {
            var files = await _context.Files.Where(f => fileIds.Contains(f.Id)).ToListAsync();
            if (!files.Any()) return;

            _context.Files.RemoveRange(files);
            await _context.SaveChangesAsync();

            foreach (var file in files)
            {
                DeletePhysicalFile(file.Type, file.StorageFileGuid);
            }
        }

        public async Task<List<FileInformationDto>> GetAllFilesAsync()
        {
            return await _context.Files.AsNoTracking()
                .Select(f => MapToDto(f))
                .ToListAsync();
        }

        public async Task<List<FileInformationDto>> GetFilesByTypeAsync(FileType fileType)
        {
            return await _context.Files.AsNoTracking()
                .Where(f => f.Type == fileType)
                .Select(f => MapToDto(f))
                .ToListAsync();
        }

        public async Task<List<FileInformationDto>> GetFilesByIDsAsync(List<int> fileIds)
        {
            return await _context.Files.AsNoTracking()
                .Where(f => fileIds.Contains(f.Id))
                .Select(f => MapToDto(f))
                .ToListAsync();
        }

        private static FileInformationDto MapToDto(FileStorage f) => new FileInformationDto
        {
            FileId = f.Id,
            FileName = f.FileName,
            FileType = f.Type,
            UploadDate = DateTime.SpecifyKind(f.UploadDate, DateTimeKind.Utc),
            ChangeDate = f.ChangeDate.HasValue ? DateTime.SpecifyKind(f.ChangeDate.Value, DateTimeKind.Utc) : null,
            IsEncrypted = f.IsEncrypted
        };

        public async Task<List<string>> CheckFilesComparability()
        {
            var basePath = GetBaseStoragePath();
            if (!Directory.Exists(basePath))
                return new List<string> { "Корневая папка хранилища не существует." };

            var databaseFiles = (await _context.Files.AsNoTracking()
                .Select(fs => fs.StorageFileGuid.ToString())
                .ToListAsync())
                .ToHashSet();

            var directoryFiles = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories)
                .Select(df => Path.GetFileNameWithoutExtension(df))
                .ToHashSet();

            var incomparabilityFiles = new List<string>();

            foreach (var databaseFile in databaseFiles)
            {
                if (!directoryFiles.Contains(databaseFile))
                    incomparabilityFiles.Add($"Файл \"{databaseFile}\" отсутствует на диске, но есть запись в БД!");
            }

            foreach (var directoryFile in directoryFiles)
            {
                if (directoryFile.StartsWith(".")) continue;

                if (!databaseFiles.Contains(directoryFile))
                    incomparabilityFiles.Add($"Файл \"{directoryFile}\" отсутствует в БД, но есть на диске!");
            }

            return incomparabilityFiles;
        }

        private async Task SavePhysicalFileAsync(Stream contentStream, Guid fileGUID, FileType fileType, bool encrypt = false)
        {
            string typeDirectory = GetPhysicalDirectoryPath(fileType);
            Directory.CreateDirectory(typeDirectory);

            string targetPath = Path.Combine(typeDirectory, fileGUID.ToString());

            using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);

            if (encrypt)
                await FileEncryptionHelper.EncryptStreamAsync(contentStream, fileStream);
            else
                await contentStream.CopyToAsync(fileStream);
        }

        private void DeletePhysicalFile(FileType fileType, Guid fileGUID)
        {
            var filePath = GetPhysicalFilePath(fileType, fileGUID);
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); } 
                catch(Exception ex) { Log.Error(ex.Message); }
            }
        }

        private string GetBaseStoragePath()
        {
            string storagePath = _settingsService.StoragePath;
            if (string.IsNullOrWhiteSpace(storagePath))
                storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FileStorage");

            return storagePath;
        }

        private string GetPhysicalDirectoryPath(FileType fileType)
        {
            return Path.Combine(GetBaseStoragePath(), fileType.ToString());
        }

        private string GetPhysicalFilePath(FileType fileType, Guid fileGUID)
        {
            return Path.Combine(GetPhysicalDirectoryPath(fileType), fileGUID.ToString());
        }
    }
}
