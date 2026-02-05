using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractAct
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public required string ActNumber { get; set; }
        public DateOnly ActDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal? VATAmount { get; set; }
        public decimal AggregateAmount { get; set; }
        public int CurrencyId { get; set; }
        public int? ContractorId { get; set; }
        public int? CustomerId { get; set; }
        public int InvoiceId { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
        public virtual ICollection<ContractActItem> Items { get; set; } = new List<ContractActItem>();
    }
}
