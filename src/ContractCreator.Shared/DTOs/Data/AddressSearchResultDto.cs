namespace ContractCreator.Shared.DTOs.Data
{
    public class AddressSearchResultDto
    {
        public long ObjectId { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public string? PostalIndex { get; set; }
    }
}
