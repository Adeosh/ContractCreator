using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Contracts.Documents;
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

            var spec = new WaybillByIdWithDetailsSpec(id);
            var waybill = await factory.Repository<ContractWaybill>().FirstOrDefaultAsync(spec);

            return waybill?.Adapt<ContractWaybillDto>();
        }

        public async Task<int> CreateAsync(ContractWaybillDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync(); // Открываем транзакцию

            try
            {
                var entity = dto.Adapt<ContractWaybill>();

                await factory.Repository<ContractWaybill>().AddAsync(entity);
                await factory.SaveChangesAsync(); // Сохраняем, чтобы получить Id шапки

                if (dto.Items != null && dto.Items.Any())
                {
                    var itemRepo = factory.Repository<ContractWaybillItem>();
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractWaybillItem>();
                        itemEntity.Id = 0;
                        itemEntity.WaybillId = entity.Id;
                        itemEntity.CurrencyId = entity.CurrencyId;
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

        public async Task UpdateAsync(ContractWaybillDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var waybillRepo = factory.Repository<ContractWaybill>();
                var itemRepo = factory.Repository<ContractWaybillItem>();

                var spec = new WaybillByIdWithDetailsSpec(dto.Id);
                var entity = await waybillRepo.FirstOrDefaultAsync(spec);

                if (entity == null) throw new Exception("Накладная не найдена");

                dto.Adapt(entity);
                await waybillRepo.UpdateAsync(entity);

                foreach (var oldItem in entity.Items.ToList())
                    await itemRepo.DeleteAsync(oldItem);

                await factory.SaveChangesAsync();

                if (dto.Items != null)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractWaybillItem>();
                        itemEntity.Id = 0;
                        itemEntity.WaybillId = entity.Id;
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
            var entity = await factory.Repository<ContractWaybill>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractWaybill>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
