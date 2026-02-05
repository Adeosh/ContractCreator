using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractSpecification
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public required string NomenclatureName { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VATRate { get; set; }
        public int CurrencyId { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
    }
}
