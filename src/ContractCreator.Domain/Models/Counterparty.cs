using ContractCreator.Domain.Enums;
using ContractCreator.Domain.ValueObjects;

namespace ContractCreator.Domain.Models
{
    public class Counterparty
    {
        public int Id { get; set; }
        public LegalFormType LegalForm { get; set; }
        public required string FullName { get; set; }
        public required string ShortName { get; set; }
        public string? Phone { get; set; }
        public EmailAddress? Email { get; set; }
        public required AddressData LegalAddress { get; set; }
        public required AddressData ActualAddress { get; set; }
        public required string INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? OKTMO { get; set; }
        public string? OKPO { get; set; }
        public string? ExtraInformation { get; set; }
        public int? DirectorId { get; set; }
        public virtual Contact? Director { get; set; }
        public int? AccountantId { get; set; }
        public virtual Contact? Accountant { get; set; }
        public int FirmId { get; set; }
        public virtual Firm Firm { get; set; } = null!;
        public DateOnly CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public virtual ICollection<CounterpartyFile> Files { get; set; } = new List<CounterpartyFile>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
