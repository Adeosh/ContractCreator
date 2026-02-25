using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts
{
    public class ContractStageHistoryByContractIdSpec : BaseSpecification<ContractStageChangeHistory>
    {
        public ContractStageHistoryByContractIdSpec(int contractId) : base(x => x.ContractId == contractId)
        {
            ApplyOrderByDescending(x => x.ChangeDate);
        }
    }
}
