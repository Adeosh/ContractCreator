namespace ContractCreator.Shared.DTOs
{
    public class FirmEconomicActivityDto
    {
        public int Id { get; set; }
        public int EconomicActivityId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public string DisplayName => $"{Code} {Name}";
    }
}
