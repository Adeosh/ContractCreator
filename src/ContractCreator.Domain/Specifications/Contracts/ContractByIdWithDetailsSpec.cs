using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts
{
    public class ContractByIdWithDetailsSpec : BaseSpecification<Contract>
    {
        public ContractByIdWithDetailsSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Currency);
            AddInclude(x => x.StageType);
            AddInclude(x => x.Acts);
            AddInclude(x => x.Invoices);
            AddInclude(x => x.Steps);
            AddInclude(x => x.Waybills);
            AddInclude(x => x.Firm);
            AddInclude(x => x.Counterparty);
            AddInclude(x => x.FirmSigner!);
            AddInclude(x => x.CounterpartySigner!);
            AddInclude(x => x.Specifications);
            AddInclude("Specifications.Currency");
        }
    }
}
