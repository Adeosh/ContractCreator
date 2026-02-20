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
        private readonly IUnitOfWorkFactory _uowFactory;

        public ProductService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<GoodsAndServiceDto>> GetAllAsync()
        {
            using var factory = _uowFactory.Create();

            var spec = new ProductWithCurrencySpec();
            var products = await factory.Repository<GoodsAndService>().ListAsync(spec);

            return products.Adapt<IEnumerable<GoodsAndServiceDto>>();
        }

        public async Task<GoodsAndServiceDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new ProductWithCurrencySpec(id);
            var product = await factory.Repository<GoodsAndService>().FirstOrDefaultAsync(spec);

            return product?.Adapt<GoodsAndServiceDto>();
        }

        public async Task<int> CreateAsync(GoodsAndServiceDto dto)
        {
            using var factory = _uowFactory.Create();

            var entity = dto.Adapt<GoodsAndService>();

            entity.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            entity.IsDeleted = false;

            await factory.Repository<GoodsAndService>().AddAsync(entity);
            await factory.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(GoodsAndServiceDto dto)
        {
            using var factory = _uowFactory.Create();

            var entity = await factory.Repository<GoodsAndService>().GetByIdAsync(dto.Id);

            if (entity == null) throw new Exception("Товар/Услуга не найдены");

            dto.Adapt(entity);

            await factory.Repository<GoodsAndService>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var entity = await factory.Repository<GoodsAndService>().GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                await factory.Repository<GoodsAndService>().UpdateAsync(entity);
                await factory.SaveChangesAsync();
            }
        }
    }
}
