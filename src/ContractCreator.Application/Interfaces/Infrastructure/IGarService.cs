using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.Application.Interfaces.Infrastructure
{
    public interface IGarService
    {
        Task<IEnumerable<AddressSearchResultDto>> SearchAddressAsync(string query, CancellationToken ct);
    }
}
