using ContractCreator.Application.Interfaces.Infrastructure;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Services.Files;
using ContractCreator.Infrastructure.Services.Files.Helpers;
using ContractCreator.Shared.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace ContractCreator.Tests.Integration.Services
{
    public class FileServiceTests : IntegrationTestBase, IDisposable
    {
        private readonly FileService _fileService;
        private readonly Mock<ISettingsService> _settingsServiceMock;
        private readonly string _tempStoragePath;
        private readonly string _expectedStoragePath;

        public FileServiceTests() : base()
        {
            _tempStoragePath = Path.Combine(Path.GetTempPath(), "ContractCreatorTests", Guid.NewGuid().ToString());
            _expectedStoragePath = Path.Combine(_tempStoragePath, "FileStorage");

            Directory.CreateDirectory(_tempStoragePath);

            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.StoragePath).Returns(_tempStoragePath);

            var contextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();
            contextFactoryMock.Setup(f => f.CreateDbContextAsync(default)).ReturnsAsync(() => CreateContext());

            _fileService = new FileService(contextFactoryMock.Object, _settingsServiceMock.Object);
        }

        public new void Dispose()
        {
            if (Directory.Exists(_tempStoragePath))
            {
                Directory.Delete(_tempStoragePath, true);
            }
            base.Dispose();
        }

        private Stream CreateTestStream(string content)
            => new MemoryStream(Encoding.UTF8.GetBytes(content));

        [Fact]
        public async Task UploadFileAsync_ShouldSaveToDbAndDisk()
        {
            // Arrange
            var content = "Простой текст";
            using var stream = CreateTestStream(content);
            var fileType = (FileType)1;

            // Act
            var fileId = await _fileService.UploadFileAsync(stream, fileType, "test.txt", DateTime.UtcNow);

            // Assert
            fileId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var fileInDb = await db.Files.FindAsync(fileId);
            fileInDb.Should().NotBeNull();
            fileInDb!.FileName.Should().Be("test.txt");
            fileInDb.IsEncrypted.Should().BeFalse();

            var expectedPath = Path.Combine(_expectedStoragePath, fileType.ToString(), fileInDb.StorageFileGuid.ToString());
            File.Exists(expectedPath).Should().BeTrue();
        }

        [Fact]
        public async Task UploadEncryptedFileAsync_ShouldSaveEncryptedToDisk()
        {
            // Arrange
            var content = "Секретный текст";
            using var stream = CreateTestStream(content);
            var fileType = (FileType)1;

            // Act
            var fileId = await _fileService.UploadEncryptedFileAsync(stream, fileType, "secret.txt", DateTime.UtcNow);

            // Assert
            using var db = CreateContext();
            var fileInDb = await db.Files.FindAsync(fileId);
            fileInDb!.IsEncrypted.Should().BeTrue();

            var downloadedFile = await _fileService.DownloadFileAsync(fileId);
            downloadedFile.Should().NotBeNull();
            downloadedFile!.FileName.Should().Be("secret.txt");
            downloadedFile.IsEncrypted.Should().BeTrue();

            var decryptedText = Encoding.UTF8.GetString(downloadedFile.Content);
            decryptedText.Should().Be(content);
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldRemoveFromDbAndDisk()
        {
            // Arrange
            using var stream = CreateTestStream("Удали меня");
            var fileType = (FileType)1;
            var fileId = await _fileService.UploadFileAsync(stream, fileType, "delete.txt", DateTime.UtcNow);

            using var db = CreateContext();
            var fileGuid = (await db.Files.FindAsync(fileId))!.StorageFileGuid;
            var filePath = Path.Combine(_expectedStoragePath, fileType.ToString(), fileGuid.ToString());

            File.Exists(filePath).Should().BeTrue();

            // Act
            var result = await _fileService.DeleteFileAsync(fileId);

            // Assert
            result.Should().BeTrue();
            File.Exists(filePath).Should().BeFalse();

            using var dbCheck = CreateContext();
            (await dbCheck.Files.FindAsync(fileId)).Should().BeNull();
        }

        [Fact]
        public async Task CheckFilesComparability_ShouldFindDiscrepancies()
        {
            // Arrange
            var fileType = (FileType)1;

            using var stream1 = CreateTestStream("Ок");
            await _fileService.UploadFileAsync(stream1, fileType, "ok.txt", DateTime.UtcNow);

            using var stream2 = CreateTestStream("Потерян");
            var missingFileId = await _fileService.UploadFileAsync(stream2, fileType, "missing.txt", DateTime.UtcNow);

            using var db = CreateContext();
            var missingGuid = (await db.Files.FindAsync(missingFileId))!.StorageFileGuid;
            File.Delete(Path.Combine(_expectedStoragePath, fileType.ToString(), missingGuid.ToString()));

            var orphanGuid = Guid.NewGuid();
            var orphanDirectory = Path.Combine(_expectedStoragePath, fileType.ToString());
            Directory.CreateDirectory(orphanDirectory);
            var orphanPath = Path.Combine(orphanDirectory, orphanGuid.ToString());
            await File.WriteAllTextAsync(orphanPath, "Призрак");

            // Act
            var discrepancies = await _fileService.CheckFilesComparability();

            // Assert
            discrepancies.Should().HaveCount(2);
            discrepancies.Should().Contain(msg =>
                msg.Contains(missingGuid.ToString(), StringComparison.OrdinalIgnoreCase) &&
                msg.Contains("отсутствует на диске"));

            discrepancies.Should().Contain(msg =>
                msg.Contains(orphanGuid.ToString(), StringComparison.OrdinalIgnoreCase) &&
                msg.Contains("отсутствует в БД"));
        }

        [Fact]
        public async Task EncryptAndDecrypt_ShouldReturnOriginalContent()
        {
            // Arrange
            var originalText = "Секретный договор №123";
            var originalBytes = Encoding.UTF8.GetBytes(originalText);
            using var inputStream = new MemoryStream(originalBytes);

            var tempFilePath = Path.GetTempFileName();

            try
            {
                // Act
                using (var outputStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await FileEncryptionHelper.EncryptStreamAsync(inputStream, outputStream);
                }

                // Act
                var decryptedBytes = await FileEncryptionHelper.DecryptFileToBytesAsync(tempFilePath);
                var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                // Assert
                decryptedText.Should().Be(originalText);
            }
            finally
            {
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
        }

        [Fact]
        public async Task DecryptFile_ShouldThrow_WhenFormatVersionIsWrong()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();

            await File.WriteAllBytesAsync(tempFilePath, new byte[] { 99, 1, 2, 3, 4 });

            try
            {
                // Act & Assert
                var action = async () => await FileEncryptionHelper.DecryptFileToBytesAsync(tempFilePath);

                await action.Should().ThrowAsync<CryptographicException>()
                    .WithMessage("Неверная версия формата шифрования файла.");
            }
            finally
            {
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
        }
    }
}
