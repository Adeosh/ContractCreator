using ContractCreator.Shared.Common.Extensions;
using ContractCreator.Shared.Enums;

namespace ContractCreator.Shared.DTOs
{
    public class GoodsAndServiceDto
    {
        public int Id { get; set; }
        public ProductType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateOnly CreatedDate { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal Price { get; set; }
        public int CurrencyId { get; set; }
        public int FirmId { get; set; }
        public bool IsDeleted { get; set; }

        public string TypeName => Type.GetDescription();
    }
}
