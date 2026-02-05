using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class FirmEconomicActivity
    {
        public int FirmId { get; set; }
        public virtual Firm Firm { get; set; } = null!;
        public int EconomicActivityId { get; set; }
        public virtual ClassifierOkved EconomicActivity { get; set; } = null!;

        /// <summary> Главный ОКВЭД </summary>
        public bool IsMain { get; set; }
    }
}
