using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractWaybillService : IContractWaybillService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractWaybillService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractWaybillDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allWaybills = await factory.Repository<ContractWaybill>().ListAllAsync();
            var filtered = allWaybills.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractWaybillDto>>();
        }

        public async Task<ContractWaybillDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var waybill = await factory.Repository<ContractWaybill>().GetByIdAsync(id);
            return waybill?.Adapt<ContractWaybillDto>();
        }

        public async Task<int> CreateAsync(ContractWaybillDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = dto.Adapt<ContractWaybill>();

            await factory.Repository<ContractWaybill>().AddAsync(entity);
            await factory.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(ContractWaybillDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractWaybill>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Счет не найден");

            dto.Adapt(entity);

            await factory.Repository<ContractWaybill>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractWaybill>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractWaybill>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
