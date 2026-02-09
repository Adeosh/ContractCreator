using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Infrastructure.Services.Gar
{
    public interface IGarService
    {
        /// <summary>
        /// Поиск адресов по ГАР
        /// </summary>
        /// <param name="query">Строка поиска</param>
        Task<IEnumerable<AddressSearchResultDto>> SearchAddressAsync(string query, CancellationToken ct);
    }
}
