using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface ICounterpartyService
    {
        Task<IEnumerable<CounterpartyDto>> GetAllCounterpartiesAsync();
        Task<CounterpartyDto?> GetCounterpartyByIdAsync(int id);
        Task<int> CreateCounterpartyAsync(CounterpartyDto dto);
        Task UpdateCounterpartyAsync(CounterpartyDto dto);
        Task DeleteCounterpartyAsync(int id);
    }
}
