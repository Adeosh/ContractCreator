using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts
{
    public class ContractsByFirmIdSpec : BaseSpecification<Contract>
    {
        public ContractsByFirmIdSpec(int firmId) : base(x => x.FirmId == firmId)
        {
            AddInclude(x => x.Counterparty);
            AddInclude(x => x.Currency);
            AddInclude(x => x.StageType);
        }
    }
}
