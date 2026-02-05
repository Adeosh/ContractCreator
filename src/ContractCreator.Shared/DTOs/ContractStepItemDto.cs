namespace ContractCreator.Shared.DTOs
{
    public class ContractStepItemDto
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public string NomenclatureName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
    }
}
