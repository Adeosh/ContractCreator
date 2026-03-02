using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Contracts.Documents
{
    public class InvoiceByIdWithDetailsSpec : BaseSpecification<ContractInvoice>
    {
        public InvoiceByIdWithDetailsSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Items);
            AddInclude(x => x.Currency);
            AddInclude(x => x.Contract);
        }
    }
}
