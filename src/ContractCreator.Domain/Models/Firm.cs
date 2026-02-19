using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.Enums;

namespace ContractCreator.Domain.Models
{
    public class Firm
    {
        public int Id { get; set; }
        public LegalFormType LegalFormType { get; set; }
        public required string FullName { get; set; }
        public required string ShortName { get; set; }
        public required string Phone { get; set; }
        public required EmailAddress Email { get; set; }
        public required AddressData LegalAddress { get; set; }
        public required AddressData ActualAddress { get; set; }
        public required string INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? OKTMO { get; set; }
        public string? OKPO { get; set; }
        public string? ERNS { get; set; }
        public string? ExtraInformation { get; set; }
        public TaxationSystemType TaxationType { get; set; }
        public bool IsVATPayment { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public byte[]? FacsimileSeal { get; set; }
        public string? FacsimileName { get; set; }
        public bool IsDeleted { get; set; }

        public int OkopfId { get; set; }
        public virtual ClassifierOkopf Okopf { get; set; } = null!;

        public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public virtual ICollection<FirmFile> Files { get; set; } = new List<FirmFile>();
        public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
        public virtual ICollection<FirmEconomicActivity> EconomicActivities { get; set; } = new List<FirmEconomicActivity>();
    }
}
