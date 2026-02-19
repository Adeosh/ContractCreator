namespace ContractCreator.Shared.DTOs.Data
{
    public class FileDataDto
    {
        public int FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public bool IsEncrypted { get; set; }
    }
}
