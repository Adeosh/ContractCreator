using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.PrintForms;

namespace ContractCreator.Application.Interfaces
{
    public interface IContractWaybillService
    {
        Task<IEnumerable<ContractWaybillDto>> GetByContractIdAsync(int contractId);
        Task<ContractWaybillDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ContractWaybillDto dto);
        Task UpdateAsync(ContractWaybillDto dto);
        Task DeleteAsync(int id);
        Task<WaybillPrintDto> GetPrintDataAsync(int waybillId);
    }
}
