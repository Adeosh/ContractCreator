using ContractCreator.Domain.Enums;
using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class GoodsAndService
    {
        public int Id { get; set; }
        public ProductType Type { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly CreatedDate { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal Price { get; set; }
        public int CurrencyId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ClassifierOkv Currency { get; set; } = null!;
    }
}
