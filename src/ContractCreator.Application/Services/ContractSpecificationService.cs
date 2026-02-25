using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractSpecificationService : IContractSpecificationService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractSpecificationService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractSpecificationDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allSpecifications = await factory.Repository<ContractSpecification>().ListAllAsync();
            var filtered = allSpecifications.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractSpecificationDto>>();
        }

        public async Task<ContractSpecificationDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var specification = await factory.Repository<ContractInvoice>().GetByIdAsync(id);
            return specification?.Adapt<ContractSpecificationDto>();
        }

        public async Task<int> CreateAsync(ContractSpecificationDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = dto.Adapt<ContractSpecification>();

            await factory.Repository<ContractSpecification>().AddAsync(entity);
            await factory.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(ContractSpecificationDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractSpecification>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Счет не найден");

            dto.Adapt(entity);

            await factory.Repository<ContractSpecification>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractSpecification>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractSpecification>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
