using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts.Documents
{
    public class ActByIdWithDetailsSpec : BaseSpecification<ContractAct>
    {
        public ActByIdWithDetailsSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Items);
            AddInclude(x => x.Currency);
            AddInclude(x => x.Contract);
        }
    }
}
