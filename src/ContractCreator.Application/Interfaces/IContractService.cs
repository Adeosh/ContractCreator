using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetContractsByFirmIdAsync(int firmId);
        Task<ContractDto?> GetContractByIdAsync(int id);
        Task<int> CreateContractAsync(ContractDto dto);
        Task UpdateContractAsync(ContractDto dto);
        Task DeleteContractAsync(int id);
        Task<List<ContractStageDto>> GetAllStagesAsync();
        Task<int> SaveContractWithDetailsAsync(
            ContractDto contract,
            IEnumerable<ContractSpecificationDto> specifications,
            IEnumerable<ContractStepDto> steps,
            int currentWorkerId);
    }
}
