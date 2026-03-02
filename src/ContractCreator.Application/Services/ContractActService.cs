using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Contracts.Documents;
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

            var spec = new ActByIdWithDetailsSpec(id);
            var act = await factory.Repository<ContractAct>().FirstOrDefaultAsync(spec);

            return act?.Adapt<ContractActDto>();
        }

        public async Task<int> CreateAsync(ContractActDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var entity = dto.Adapt<ContractAct>();

                await factory.Repository<ContractAct>().AddAsync(entity);
                await factory.SaveChangesAsync();

                if (dto.Items != null && dto.Items.Any())
                {
                    var itemRepo = factory.Repository<ContractActItem>();
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractActItem>();
                        itemEntity.Id = 0;
                        itemEntity.ActId = entity.Id;
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

        public async Task UpdateAsync(ContractActDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var actRepo = factory.Repository<ContractAct>();
                var itemRepo = factory.Repository<ContractActItem>();

                var spec = new ActByIdWithDetailsSpec(dto.Id);
                var entity = await actRepo.FirstOrDefaultAsync(spec);

                if (entity == null) throw new Exception("Акт не найден");

                dto.Adapt(entity);
                await actRepo.UpdateAsync(entity);

                foreach (var oldItem in entity.Items.ToList())
                    await itemRepo.DeleteAsync(oldItem);

                await factory.SaveChangesAsync();

                if (dto.Items != null)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractActItem>();
                        itemEntity.Id = 0;
                        itemEntity.ActId = entity.Id;
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
            var entity = await factory.Repository<ContractAct>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractAct>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
