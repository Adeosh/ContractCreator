using ContractCreator.Domain.Models;

namespace ContractCreator.Domain.Specifications.Data
{
    public class ProductWithCurrencySpec : BaseSpecification<GoodsAndService>
    {
        public ProductWithCurrencySpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Currency);
        }

        public ProductWithCurrencySpec() : base(p => !p.IsDeleted)
        {
            AddInclude(p => p.Currency);
        }
    }
}
