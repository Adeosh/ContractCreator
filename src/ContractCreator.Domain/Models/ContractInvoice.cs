using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractInvoice
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int BankAccountId { get; set; }
        public required string InvoiceNumber { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public required string PurchaserINN { get; set; }
        public required string PurchaserKPP { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? VATAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal AggregateAmount { get; set; }
        public int CurrencyId { get; set; }
        public int? CountNomenclatureNames { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
        public virtual ICollection<ContractInvoiceItem> Items { get; set; } = new List<ContractInvoiceItem>();
    }
}
