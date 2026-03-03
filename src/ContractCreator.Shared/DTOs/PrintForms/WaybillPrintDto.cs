namespace ContractCreator.Shared.DTOs.PrintForms
{
    public class WaybillPrintDto
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public DateTime DocumentDate { get; set; } = DateTime.Now;
        public string ApplicationName { get; set; } = "Актум";

        public string WaybillNumber { get; set; } = string.Empty;
        public string WaybillDate { get; set; } = string.Empty;

        public string ContractInfo { get; set; } = string.Empty;

        public string ConsignorFullData { get; set; } = string.Empty;
        public string SupplierFullData { get; set; } = string.Empty;
        public string ConsigneeFullData { get; set; } = string.Empty;
        public string PayerFullData { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public string FormattedTotalAmount { get; set; } = string.Empty;
        public decimal VATAmount { get; set; }
        public string FormattedVATAmount { get; set; } = string.Empty;
        public decimal AggregateAmount { get; set; }
        public string FormattedAggregateAmount { get; set; } = string.Empty;
        public string AmountInWords { get; set; } = string.Empty;

        public int TotalItemsCount { get; set; }
        public int TotalQuantity { get; set; }

        public string DirectorName { get; set; } = string.Empty;
        public string ChiefAccountantName { get; set; } = string.Empty;
        public string StorekeeperName { get; set; } = string.Empty;
        public string CustomerSignatoryName { get; set; } = string.Empty;

        public List<WaybillPrintItemDto> Items { get; set; } = new();
    }

    public class WaybillPrintItemDto
    {
        public int Number { get; set; }
        public string Nomenclature { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
