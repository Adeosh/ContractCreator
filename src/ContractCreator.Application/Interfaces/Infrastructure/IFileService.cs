using ContractCreator.Shared.DTOs.Data;
using ContractCreator.Shared.Enums;

namespace ContractCreator.Application.Interfaces.Infrastructure
{
    public interface IFileService
    {
        Task<int> UploadEncryptedFileAsync(Stream fileStream, FileType fileType, string fileName, DateTime uploadDate);
        Task<int> UploadFileAsync(Stream fileStream, FileType fileType, string fileName, DateTime uploadDate);
        Task<FileDataDto?> DownloadFileAsync(int fileId);
        Task UpdateEncryptedFileAsync(int fileId, Stream fileStream, string fileName, DateTime changeDate);
        Task UpdateFileAsync(int fileId, Stream fileStream, string fileName, DateTime changeDate);
        Task<List<FileInformationDto>> GetFilesByTypeAsync(FileType fileType);
        Task<List<FileInformationDto>> GetAllFilesAsync();
        Task<List<FileInformationDto>> GetFilesByIDsAsync(List<int> fileIds);
        Task<List<FileDataDto>> GetFileDataByIDsAsync(List<int> fileIds);
        Task<bool> DeleteFileAsync(int fileId);
        Task DeleteFilesByIDsAsync(List<int> fileIds);
        Task<List<string>> CheckFilesComparability();
    }
}
