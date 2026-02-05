using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GoodsAndServiceDto>> GetAllAsync();
        Task<GoodsAndServiceDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(GoodsAndServiceDto dto);
        Task UpdateAsync(GoodsAndServiceDto dto);
        Task DeleteAsync(int id);
    }
}
