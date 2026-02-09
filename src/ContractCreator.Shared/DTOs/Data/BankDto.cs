namespace ContractCreator.Shared.DTOs.Data
{
    public class BankDto
    {
        public string Bic { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CorrespondentAccount { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DisplayName => $"{Bic} ({Name})";
    }
}
