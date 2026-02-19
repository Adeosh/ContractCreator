using ContractCreator.Shared.Enums;

namespace ContractCreator.Shared.DTOs.Data
{
    public class FileInformationDto
    {
        public int FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public FileType FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ChangeDate { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
