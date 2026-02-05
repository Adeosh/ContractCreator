namespace ContractCreator.Shared.DTOs
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public string BIC { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string? CorrespondentAccount { get; set; }
        public string? BankAddress { get; set; }
        public bool IsDeleted { get; set; }
        public int? FirmId { get; set; }
        public int? CounterpartyId { get; set; }
    }
}
