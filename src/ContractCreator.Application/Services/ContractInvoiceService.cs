using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Contracts.Documents;
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

            var spec = new InvoiceByIdWithDetailsSpec(id);
            var invoice = await factory.Repository<ContractInvoice>().FirstOrDefaultAsync(spec);

            return invoice?.Adapt<ContractInvoiceDto>();
        }

        public async Task<int> CreateAsync(ContractInvoiceDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync(); // Открываем транзакцию

            try
            {
                var entity = dto.Adapt<ContractInvoice>();

                await factory.Repository<ContractInvoice>().AddAsync(entity);
                await factory.SaveChangesAsync();

                if (dto.Items != null && dto.Items.Any())
                {
                    var itemRepo = factory.Repository<ContractInvoiceItem>();
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractInvoiceItem>();
                        itemEntity.Id = 0;
                        itemEntity.InvoiceId = entity.Id;
                        await itemRepo.AddAsync(itemEntity);
                    }
                    await factory.SaveChangesAsync();
                }

                await factory.CommitTransactionAsync();
                return entity.Id;
            }
            catch
            {
                await factory.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateAsync(ContractInvoiceDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var invoiceRepo = factory.Repository<ContractInvoice>();
                var itemRepo = factory.Repository<ContractInvoiceItem>();

                var spec = new InvoiceByIdWithDetailsSpec(dto.Id);
                var entity = await invoiceRepo.FirstOrDefaultAsync(spec);

                if (entity == null) throw new Exception("Счет не найден");

                dto.Adapt(entity);
                await invoiceRepo.UpdateAsync(entity);

                foreach (var oldItem in entity.Items.ToList())
                    await itemRepo.DeleteAsync(oldItem);

                await factory.SaveChangesAsync();

                if (dto.Items != null)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractInvoiceItem>();
                        itemEntity.Id = 0;
                        itemEntity.InvoiceId = entity.Id;
                        await itemRepo.AddAsync(itemEntity);
                    }
                }

                await factory.SaveChangesAsync();
                await factory.CommitTransactionAsync();
            }
            catch
            {
                await factory.RollbackTransactionAsync();
                throw;
            }
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
