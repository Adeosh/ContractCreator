namespace ContractCreator.Domain.Models.Dictionaries
{
    /// <summary> ОКВ — Общероссийский классификатор валют </summary>
    /// <a href="https://classifikators.ru/okv#download"></a>
    public class ClassifierOkv
    {
        public int Id { get; set; }
        public int NumericCode { get; set; }
        public required string LetterCode { get; set; }
        public required string CurrencyName { get; set; }
        public required string CountriesCurrencyUsed { get; set; }
        public string? Note { get; set; }

        /// <summary> Тенге </summary>
        public const int KZT = 57;
        /// <summary> Российский рубль </summary>
        public const int RUB = 95;
        /// <summary> Доллар США </summary>
        public const int USD = 119;
        /// <summary> Белорусский рубль </summary>
        public const int BYN = 132;
        /// <summary> Евро </summary>
        public const int EUR = 157;
    }
}
