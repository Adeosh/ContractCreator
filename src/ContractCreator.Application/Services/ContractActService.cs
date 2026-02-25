using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractActService : IContractActService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractActService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractActDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allActs = await factory.Repository<ContractAct>().ListAllAsync();
            var filtered = allActs.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractActDto>>();
        }

        public async Task<ContractActDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var act = await factory.Repository<ContractAct>().GetByIdAsync(id);
            return act?.Adapt<ContractActDto>();
        }

        public async Task<int> CreateAsync(ContractActDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = dto.Adapt<ContractAct>();

            await factory.Repository<ContractAct>().AddAsync(entity);
            await factory.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(ContractActDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractAct>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Счет не найден");

            dto.Adapt(entity);

            await factory.Repository<ContractAct>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractAct>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractAct>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
