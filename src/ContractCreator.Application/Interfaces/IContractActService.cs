using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IContractActService
    {
        Task<IEnumerable<ContractActDto>> GetByContractIdAsync(int contractId);
        Task<ContractActDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ContractActDto dto);
        Task UpdateAsync(ContractActDto dto);
        Task DeleteAsync(int id);
    }
}
