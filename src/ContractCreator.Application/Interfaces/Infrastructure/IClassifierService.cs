using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Application.Interfaces.Infrastructure
{
    public interface IClassifierService
    {
        Task<List<ClassifierDto>> GetOkopfsAsync();
        Task<List<ClassifierDto>> GetCurrenciesAsync();
        Task<List<ClassifierDto>> SearchOkvedsAsync(string query);
        Task<List<ClassifierDto>> GetAllOkvedsAsync();
    }
}
