namespace ContractCreator.Shared.DTOs
{
    public class FileStorageDto
    {
        public int Id { get; set; }
        public Guid StorageFileGuid { get; set; }
        public byte Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public DateTime? ChangeDate { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
