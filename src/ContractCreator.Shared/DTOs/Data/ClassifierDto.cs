namespace ContractCreator.Shared.DTOs.Data
{
    /// <summary>
    /// ОКВЭД, ОКОПФ, Валюта
    /// </summary>
    public class ClassifierDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName => $"{Code} — {Name}";
    }
}
