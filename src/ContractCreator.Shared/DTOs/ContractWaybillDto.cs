namespace ContractCreator.Shared.DTOs
{
    public class ContractWaybillDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string ActNumber { get; set; } = string.Empty;
        public DateOnly ActDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? VATAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal AggregateAmount { get; set; }
        public int? ContractorId { get; set; }
        public int? CustomerId { get; set; }
        public int InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;

        public List<ContractWaybillItemDto> Items { get; set; } = new();
    }
}
