using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IContractSpecificationService
    {
        Task<IEnumerable<ContractSpecificationDto>> GetByContractIdAsync(int contractId);
        Task<int> CreateAsync(ContractSpecificationDto dto);
        Task UpdateAsync(ContractSpecificationDto dto);
        Task DeleteAsync(int id);
    }
}
