namespace ContractCreator.Shared.DTOs
{
    public class ContractSpecificationDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string NomenclatureName { get; set; } = string.Empty;
        public string? UnitOfMeasure { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VATRate { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
    }
}
