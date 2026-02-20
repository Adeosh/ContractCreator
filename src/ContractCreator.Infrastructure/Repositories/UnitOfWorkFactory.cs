using ContractCreator.Domain.Interfaces;
using ContractCreator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContractCreator.Infrastructure.Repositories
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UnitOfWorkFactory(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(_contextFactory.CreateDbContext());
        }
    }
}
