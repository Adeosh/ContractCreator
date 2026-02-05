using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BankAccountService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<BankAccountDto>> GetByFirmIdAsync(int firmId)
        {
            var accounts = await _unitOfWork.Repository<BankAccount>()
                .FindAsync(x => x.FirmId == firmId && !x.IsDeleted);
            return accounts.Adapt<IEnumerable<BankAccountDto>>();
        }

        public async Task<IEnumerable<BankAccountDto>> GetByCounterpartyIdAsync(int counterpartyId)
        {
            var accounts = await _unitOfWork.Repository<BankAccount>()
                .FindAsync(x => x.CounterpartyId == counterpartyId && !x.IsDeleted);
            return accounts.Adapt<IEnumerable<BankAccountDto>>();
        }

        public async Task<BankAccountDto?> GetByIdAsync(int id)
        {
            var account = await _unitOfWork.Repository<BankAccount>().GetByIdAsync(id);
            return account?.Adapt<BankAccountDto>();
        }

        public async Task<int> CreateAsync(BankAccountDto dto)
        {
            if (dto.FirmId == null && dto.CounterpartyId == null)
                throw new Exception("Счет должен быть привязан к Фирме или Контрагенту");

            var entity = dto.Adapt<BankAccount>();
            entity.IsDeleted = false;

            await _unitOfWork.Repository<BankAccount>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(BankAccountDto dto)
        {
            var entity = await _unitOfWork.Repository<BankAccount>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Банковский счет не найден");

            dto.Adapt(entity);

            await _unitOfWork.Repository<BankAccount>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<BankAccount>().GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                await _unitOfWork.Repository<BankAccount>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
