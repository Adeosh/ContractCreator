using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts.Documents
{
    public class InvoicePrintSpec : BaseSpecification<ContractInvoice>
    {
        public InvoicePrintSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Items);
            AddInclude(x => x.Currency);
            AddInclude(x => x.Contract);
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Firm)}");
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Firm)}.{nameof(Firm.Workers)}");
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Firm)}.{nameof(Firm.BankAccounts)}");
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Counterparty)}");
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Contacts)}");
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Director)}");
            AddInclude($"{nameof(ContractInvoice.Contract)}.{nameof(Contract.Counterparty)}.{nameof(Counterparty.Accountant)}");
        }
    }
}
