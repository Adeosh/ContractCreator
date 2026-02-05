using ContractCreator.Domain.Enums;

namespace ContractCreator.Domain.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public required string BIC { get; set; }
        public required string BankName { get; set; }
        public required string AccountNumber { get; set; }
        public string? CorrespondentAccount { get; set; }
        public string? BankAddress { get; set; }
        public bool IsDeleted { get; set; }

        public int? FirmId { get; set; }
        public virtual Firm? Firm { get; set; }

        public int? CounterpartyId { get; set; }
        public virtual Counterparty? Counterparty { get; set; }

        public OwnerType Type => FirmId.HasValue ? OwnerType.Firm : OwnerType.Counterparty;
    }
}
