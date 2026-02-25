using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IContractStepService
    {
        Task<IEnumerable<ContractStepDto>> GetByContractIdAsync(int contractId);
        Task<int> CreateAsync(ContractStepDto dto);
        Task UpdateAsync(ContractStepDto dto);
        Task DeleteAsync(int id);
    }
}
