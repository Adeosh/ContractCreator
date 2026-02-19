using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Shared.DTOs
{
    public class FirmDto
    {
        public int Id { get; set; }
        public byte LegalFormType { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public AddressDto LegalAddress { get; set; } = new();
        public AddressDto ActualAddress { get; set; } = new();
        public string INN { get; set; } = string.Empty;
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? OKTMO { get; set; }
        public string? OKPO { get; set; }
        public string? ERNS { get; set; }
        public string? ExtraInformation { get; set; }
        public byte TaxationType { get; set; }
        public bool IsVATPayment { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public byte[]? FacsimileSeal { get; set; }
        public string? FacsimileName { get; set; }
        public bool IsDeleted { get; set; }
        public int OkopfId { get; set; }

        public List<BankAccountDto> BankAccounts { get; set; } = new();
        public List<WorkerDto> Workers { get; set; } = new();
        public List<ContractDto> Contracts { get; set; } = new();
        public List<EntityFileDto> Files { get; set; } = new();
        public List<FirmEconomicActivityDto> EconomicActivities { get; set; } = new();

        public string DisplayName => $"{ShortName} (ИНН {INN})";
    }
}
