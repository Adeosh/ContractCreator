namespace ContractCreator.Shared.DTOs
{
    public class ContractStepDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string StepName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
        public DateOnly StartStepDate { get; set; }
        public DateOnly EndStepDate { get; set; }

        public List<ContractStepItemDto> Items { get; set; } = new();
    }
}
