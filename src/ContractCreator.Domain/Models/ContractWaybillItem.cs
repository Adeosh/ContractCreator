using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractWaybillItem
    {
        public int Id { get; set; }
        public int WaybillId { get; set; }
        public required string NomenclatureName { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }

        public virtual ContractWaybill Waybill { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
    }
}
