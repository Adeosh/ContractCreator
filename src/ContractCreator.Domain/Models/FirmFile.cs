namespace ContractCreator.Domain.Models
{
    public class FirmFile
    {
        public int FirmId { get; set; }
        public virtual Firm Firm { get; set; } = null!;
        public int FileId { get; set; }
        public virtual FileStorage File { get; set; } = null!;

        public string? Description { get; set; }
    }
}
