using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Counterparties
{
    public class CounterpartyByIdWithDetailsSpec : BaseSpecification<Counterparty>
    {
        public CounterpartyByIdWithDetailsSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.BankAccounts);
            AddInclude(x => x.Contacts);
            AddInclude(x => x.Files);
            AddInclude("Files.File");
        }
    }
}
