namespace ContractCreator.Shared.DTOs.Data
{
    public class AddressDto
    {
        public long ObjectId { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public string House { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Flat { get; set; } = string.Empty;
        public string PostalIndex { get; set; } = string.Empty;
    }
}
