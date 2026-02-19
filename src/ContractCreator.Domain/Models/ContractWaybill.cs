using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractWaybill
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int InvoiceId { get; set; }
        public required string WaybillNumber { get; set; }
        public DateOnly WaybillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal AggregateAmount { get; set; }
        public int CurrencyId { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual ContractInvoice Invoice { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
        public virtual ICollection<ContractWaybillItem> Items { get; set; } = new List<ContractWaybillItem>();
    }
}
