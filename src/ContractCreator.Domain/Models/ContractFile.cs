namespace ContractCreator.Domain.Models
{
    public class ContractFile
    {
        public int ContractId { get; set; }
        public virtual Contract Contract { get; set; } = null!;
        public int FileId { get; set; }
        public virtual FileStorage File { get; set; } = null!;

        public string? Description { get; set; }
    }
}
