namespace ContractCreator.Shared.DTOs.PrintForms
{
    public class ActPrintDto
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public DateTime DocumentDate { get; set; } = DateTime.Now;
        public string ApplicationName { get; set; } = "Актум";

        public string ActNumber { get; set; } = string.Empty;
        public string ActDate { get; set; } = string.Empty;

        public string ContractNumber { get; set; } = string.Empty;
        public string ContractDate { get; set; } = string.Empty;

        public string ContractorFullData { get; set; } = string.Empty;
        public string CustomerFullData { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public string FormattedTotalAmount { get; set; } = string.Empty;
        public decimal VATAmount { get; set; }
        public string FormattedVATAmount { get; set; } = string.Empty;
        public decimal AggregateAmount { get; set; }
        public string FormattedAggregateAmount { get; set; } = string.Empty;
        public string AmountInWords { get; set; } = string.Empty;

        public string FirmName { get; set; } = string.Empty;
        public string ContractorDirectorName { get; set; } = string.Empty;
        public string CounterpartyName { get; set; } = string.Empty;
        public string CustomerSignatoryName { get; set; } = string.Empty;

        public List<ActPrintItemDto> Items { get; set; } = new();
    }

    public class ActPrintItemDto
    {
        public int Number { get; set; }
        public string NomenclatureName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
