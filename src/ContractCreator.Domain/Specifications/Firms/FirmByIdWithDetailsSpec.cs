using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Firms
{
    public class FirmByIdWithDetailsSpec : BaseSpecification<Firm>
    {
        public FirmByIdWithDetailsSpec(int firmId)
            : base(f => f.Id == firmId)
        {
            AddInclude(f => f.BankAccounts);
            AddInclude(f => f.Workers);
            AddInclude(f => f.Files);
            AddInclude(f => f.Okopf);
            AddInclude(f => f.EconomicActivities);
            AddInclude("EconomicActivities.EconomicActivity");
        }
    }
}
