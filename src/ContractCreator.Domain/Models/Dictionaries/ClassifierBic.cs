namespace ContractCreator.Domain.Models.Dictionaries
{
    /// <summary> Справочник БИК(Банковский идентификационный код) </summary>
    /// <a href="https://www.cbr.ru/PSystem/payment_system/#a_44305"></a>
    public class ClassifierBic
    {
        public required string BIC { get; set; }
        public required string Name { get; set; }
        public string? EnglishName { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? CountryCode { get; set; }
        public byte? RegionCode { get; set; }
        public string? PostalIndex { get; set; }
        public string? SettlementType { get; set; }
        public string? SettlementName { get; set; }
        public string? Address { get; set; }
        public string? ParentBIC { get; set; }
        public DateTime DateIn { get; set; }
        public byte? ParticipantType { get; set; }
        public string? Services { get; set; }
        public string? ExchangeType { get; set; }
        public string? UID { get; set; }
        public string? ParticipantStatus { get; set; }

        /// <summary> Номер корреспондентского счета </summary>
        public string? Account { get; set; }
        /// <summary> БИК банка в котором открыт номер корреспондентского счета </summary>
        public string? AccountCBRBIC { get; set; }
    }
}
