using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Application.Interfaces.Infrastructure
{
    public interface IBicService
    {
        Task<IEnumerable<BankDto>> SearchAsync(string query);
    }
}
