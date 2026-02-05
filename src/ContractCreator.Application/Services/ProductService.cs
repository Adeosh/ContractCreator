using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Data;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<GoodsAndServiceDto>> GetAllAsync()
        {
            var spec = new ProductWithCurrencySpec();
            var products = await _unitOfWork.Repository<GoodsAndService>().ListAsync(spec);

            return products.Adapt<IEnumerable<GoodsAndServiceDto>>();
        }

        public async Task<GoodsAndServiceDto?> GetByIdAsync(int id)
        {
            var spec = new ProductWithCurrencySpec(id);
            var product = await _unitOfWork.Repository<GoodsAndService>().FirstOrDefaultAsync(spec);

            return product?.Adapt<GoodsAndServiceDto>();
        }

        public async Task<int> CreateAsync(GoodsAndServiceDto dto)
        {
            var entity = dto.Adapt<GoodsAndService>();

            entity.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            entity.IsDeleted = false;

            await _unitOfWork.Repository<GoodsAndService>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(GoodsAndServiceDto dto)
        {
            var entity = await _unitOfWork.Repository<GoodsAndService>().GetByIdAsync(dto.Id);

            if (entity == null) throw new Exception("Товар/Услуга не найдены");

            dto.Adapt(entity);

            await _unitOfWork.Repository<GoodsAndService>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<GoodsAndService>().GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                await _unitOfWork.Repository<GoodsAndService>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
