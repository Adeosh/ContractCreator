using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts.Documents
{
    public class ActPrintSpec : BaseSpecification<ContractAct>
    {
        public ActPrintSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Items);
            AddInclude(x => x.Contract);
            AddInclude($"{nameof(ContractAct.Contract)}.{nameof(Contract.Firm)}");
            AddInclude($"{nameof(ContractAct.Contract)}.{nameof(Contract.Firm)}.{nameof(Firm.Workers)}");
            AddInclude($"{nameof(ContractAct.Contract)}.{nameof(Contract.Counterparty)}");
            AddInclude($"{nameof(ContractAct.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Contacts)}");
            AddInclude($"{nameof(ContractAct.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Director)}");
        }
    }
}
