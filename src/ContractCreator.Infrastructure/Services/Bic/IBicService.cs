using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Infrastructure.Services.Bic
{
    public interface IBicService
    {
        Task<IEnumerable<BankDto>> SearchAsync(string query);
    }
}
