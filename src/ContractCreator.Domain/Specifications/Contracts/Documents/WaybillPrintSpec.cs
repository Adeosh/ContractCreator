using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts.Documents
{
    public class WaybillPrintSpec : BaseSpecification<ContractWaybill>
    {
        public WaybillPrintSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Items);
            AddInclude(x => x.Contract);
            AddInclude(x => x.Invoice);
            AddInclude($"{nameof(ContractWaybill.Contract)}.{nameof(Contract.Firm)}");
            AddInclude($"{nameof(ContractWaybill.Contract)}.{nameof(Contract.Firm)}.{nameof(Firm.Workers)}");
            AddInclude($"{nameof(ContractWaybill.Contract)}.{nameof(Contract.Firm)}.{nameof(Firm.BankAccounts)}");
            AddInclude($"{nameof(ContractWaybill.Contract)}.{nameof(Contract.Counterparty)}");
            AddInclude($"{nameof(ContractWaybill.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Contacts)}");
            AddInclude($"{nameof(ContractWaybill.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Director)}");
        }
    }
}
