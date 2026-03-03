namespace ContractCreator.Shared.DTOs.PrintForms
{
    public class InvoicePrintDto
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public DateTime DocumentDate { get; set; } = DateTime.Now;
        public string ApplicationName { get; set; } = "Актум";

        public string InvoiceNumber { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;

        public string BIC { get; set; } = string.Empty;
        public string INN { get; set; } = string.Empty;
        public string KPP { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public string BankInfo { get; set; } = string.Empty;
        public string CorrespondentAccount { get; set; } = string.Empty;
        public string PaidAccount { get; set; } = string.Empty;

        public string FirmFullData { get; set; } = string.Empty;
        public string CounterpartyFullData { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public string FormattedTotalAmount { get; set; } = string.Empty;
        public decimal VATAmount { get; set; }
        public string FormattedVATAmount { get; set; } = string.Empty;
        public decimal AggregateAmount { get; set; }
        public string FormattedAggregateAmount { get; set; } = string.Empty;
        public string AmountInWords { get; set; } = string.Empty;
        public int CountNomenclatureNames { get; set; }

        public string DirectorName { get; set; } = "Не указан";
        public string AccountantName { get; set; } = "Не указан";

        public List<InvoicePrintItemDto> Items { get; set; } = new();
    }

    public class InvoicePrintItemDto
    {
        public int Number { get; set; }
        public string NomenclatureName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
