using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IBankAccountService
    {
        Task<IEnumerable<BankAccountDto>> GetByFirmIdAsync(int firmId);
        Task<IEnumerable<BankAccountDto>> GetByCounterpartyIdAsync(int counterpartyId);
        Task<BankAccountDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(BankAccountDto dto);
        Task UpdateAsync(BankAccountDto dto);
        Task DeleteAsync(int id);
    }
}
