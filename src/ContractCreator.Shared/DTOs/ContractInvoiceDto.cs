namespace ContractCreator.Shared.DTOs
{
    public class ContractInvoiceDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int BankAccountId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateOnly InvoiceDate { get; set; }
        public string PurchaserINN { get; set; } = string.Empty;
        public string PurchaserKPP { get; set; } = string.Empty;
        public decimal? TotalAmount { get; set; }
        public decimal? VATAmount { get; set; }
        public decimal VATRate { get; set; }
        public decimal AggregateAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
        public int? CountNomenclatureNames { get; set; }

        public List<ContractInvoiceItemDto> Items { get; set; } = new();
    }
}
