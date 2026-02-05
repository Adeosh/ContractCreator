using ContractCreator.Domain.Enums;

namespace ContractCreator.Domain.Models
{
    public class FileStorage
    {
        public int Id { get; set; }
        public Guid StorageFileGuid { get; set; }
        public FileType Type { get; set; }
        public required string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ChangeDate { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
