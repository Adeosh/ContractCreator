using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractInvoiceService : IContractInvoiceService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractInvoiceService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractInvoiceDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allInvoices = await factory.Repository<ContractInvoice>().ListAllAsync();
            var filtered = allInvoices.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractInvoiceDto>>();
        }

        public async Task<ContractInvoiceDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var invoice = await factory.Repository<ContractInvoice>().GetByIdAsync(id);
            return invoice?.Adapt<ContractInvoiceDto>();
        }

        public async Task<int> CreateAsync(ContractInvoiceDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = dto.Adapt<ContractInvoice>();

            await factory.Repository<ContractInvoice>().AddAsync(entity);
            await factory.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(ContractInvoiceDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractInvoice>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Счет не найден");

            dto.Adapt(entity);

            await factory.Repository<ContractInvoice>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractInvoice>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractInvoice>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
