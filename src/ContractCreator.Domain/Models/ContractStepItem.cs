using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractStepItem
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public required string NomenclatureName { get; set; }
        public int Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }

        public virtual ContractStep Step { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
    }
}
