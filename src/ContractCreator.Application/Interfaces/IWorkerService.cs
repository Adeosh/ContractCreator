using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IWorkerService
    {
        Task<IEnumerable<WorkerDto>> GetWorkersByFirmIdAsync(int firmId);
        Task<WorkerDto?> GetWorkerByIdAsync(int id);
        Task<int> CreateWorkerAsync(WorkerDto dto);
        Task UpdateWorkerAsync(WorkerDto dto);
        Task DeleteWorkerAsync(int id);
    }
}
