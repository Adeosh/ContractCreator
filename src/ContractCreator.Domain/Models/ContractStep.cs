using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class ContractStep
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public required string StepName { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public DateOnly StartStepDate { get; set; }
        public DateOnly EndStepDate { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual ClassifierOkv Currency { get; set; } = null!;
        public virtual ICollection<ContractStepItem> Items { get; set; } = new List<ContractStepItem>();
    }
}
