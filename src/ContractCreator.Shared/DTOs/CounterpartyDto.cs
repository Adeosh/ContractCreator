using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Shared.DTOs
{
    public class CounterpartyDto
    {
        public int Id { get; set; }
        public byte LegalForm { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public AddressDto LegalAddress { get; set; } = new();
        public AddressDto ActualAddress { get; set; } = new();
        public string INN { get; set; } = string.Empty;
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? OKTMO { get; set; }
        public string? OKPO { get; set; }
        public string? ExtraInformation { get; set; }
        public int? DirectorId { get; set; }
        public int? AccountantId { get; set; }
        public int FirmId { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public List<BankAccountDto> BankAccounts { get; set; } = new();
        public List<ContractDto> Contracts { get; set; } = new();
        public List<CounterpartyFileDto> Files { get; set; } = new();
        public List<ContactDto> Contacts { get; set; } = new();
    }
}
