using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractStepService : IContractStepService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractStepService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractStepDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allSteps = await factory.Repository<ContractStep>().ListAllAsync();
            var filtered = allSteps.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractStepDto>>();
        }

        public async Task<ContractStepDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var step = await factory.Repository<ContractStep>().GetByIdAsync(id);
            return step?.Adapt<ContractStepDto>();
        }

        public async Task<int> CreateAsync(ContractStepDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = dto.Adapt<ContractStep>();

            await factory.Repository<ContractStep>().AddAsync(entity);
            await factory.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(ContractStepDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractStep>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Счет не найден");

            dto.Adapt(entity);

            await factory.Repository<ContractStep>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractStep>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractStep>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
