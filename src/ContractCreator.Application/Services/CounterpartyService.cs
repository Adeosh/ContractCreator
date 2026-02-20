using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Counterparties;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class CounterpartyService : ICounterpartyService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public CounterpartyService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<CounterpartyDto>> GetAllCounterpartiesAsync()
        {
            using var factory = _uowFactory.Create();

            var list = await factory.Repository<Counterparty>()
                .FindAsync(c => !c.IsDeleted);
            return list.Adapt<IEnumerable<CounterpartyDto>>();
        }

        public async Task<CounterpartyDto?> GetCounterpartyByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new CounterpartyByIdWithDetailsSpec(id);
            var counterparty = await factory.Repository<Counterparty>().FirstOrDefaultAsync(spec);

            return counterparty?.Adapt<CounterpartyDto>();
        }

        public async Task<int> CreateCounterpartyAsync(CounterpartyDto dto)
        {
            using var factory = _uowFactory.Create();

            var entity = dto.Adapt<Counterparty>();
            entity.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            entity.IsDeleted = false;

            await factory.Repository<Counterparty>().AddAsync(entity);
            await factory.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateCounterpartyAsync(CounterpartyDto dto)
        {
            using var factory = _uowFactory.Create();

            var spec = new CounterpartyByIdWithDetailsSpec(dto.Id);
            var entity = await factory.Repository<Counterparty>().FirstOrDefaultAsync(spec);

            if (entity == null) throw new Exception("Контрагент не найден");

            dto.Adapt(entity);
            entity.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await factory.Repository<Counterparty>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteCounterpartyAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var entity = await factory.Repository<Counterparty>().GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                await factory.Repository<Counterparty>().UpdateAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
