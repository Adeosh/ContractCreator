using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IFirmService
    {
        Task<IEnumerable<FirmDto>> GetAllFirmsAsync();
        Task<FirmDto?> GetFirmByIdAsync(int id);
        Task<int> CreateFirmAsync(FirmDto dto);
        Task UpdateFirmAsync(FirmDto dto);
        Task DeleteFirmAsync(int id);
    }
}
