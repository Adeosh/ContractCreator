using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public BankAccountService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<BankAccountDto>> GetByFirmIdAsync(int firmId)
        {
            using var factory = _uowFactory.Create();

            var accounts = await factory.Repository<BankAccount>()
                .FindAsync(x => x.FirmId == firmId && !x.IsDeleted);
            return accounts.Adapt<IEnumerable<BankAccountDto>>();
        }

        public async Task<IEnumerable<BankAccountDto>> GetByCounterpartyIdAsync(int counterpartyId)
        {
            using var factory = _uowFactory.Create();

            var accounts = await factory.Repository<BankAccount>()
                .FindAsync(x => x.CounterpartyId == counterpartyId && !x.IsDeleted);
            return accounts.Adapt<IEnumerable<BankAccountDto>>();
        }

        public async Task<BankAccountDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var account = await factory.Repository<BankAccount>().GetByIdAsync(id);
            return account?.Adapt<BankAccountDto>();
        }

        public async Task<int> CreateAsync(BankAccountDto dto)
        {
            if (dto.FirmId == null && dto.CounterpartyId == null)
                throw new Exception("Счет должен быть привязан к Фирме или Контрагенту");

            using var factory = _uowFactory.Create();

            var entity = dto.Adapt<BankAccount>();
            entity.IsDeleted = false;

            await factory.Repository<BankAccount>().AddAsync(entity);
            await factory.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(BankAccountDto dto)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<BankAccount>().GetByIdAsync(dto.Id);
            if (entity == null) 
                throw new Exception("Банковский счет не найден");

            dto.Adapt(entity);

            await factory.Repository<BankAccount>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var entity = await factory.Repository<BankAccount>().GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                await factory.Repository<BankAccount>().UpdateAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
