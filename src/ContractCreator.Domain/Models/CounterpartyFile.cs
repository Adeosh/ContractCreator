namespace ContractCreator.Domain.Models
{
    public class CounterpartyFile
    {
        public int CounterpartyId { get; set; }
        public virtual Counterparty Counterparty { get; set; } = null!;
        public int FileId { get; set; }
        public virtual FileStorage File { get; set; } = null!;

        public string? Description { get; set; }
    }
}
