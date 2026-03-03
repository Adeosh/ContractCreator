using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ProductServiceTests : IntegrationTestBase
    {
        private readonly ProductService _productService;

        public ProductServiceTests() : base()
        {
            _productService = new ProductService(UowFactory);
        }

        private async Task<int> CreateTestProductViaServiceAsync(string name = "Услуга по умолчанию")
        {
            var productModel = TestDataFactory.CreateProduct(0);
            var dto = productModel.Adapt<GoodsAndServiceDto>();

            dto.Id = 0;
            dto.Name = name;

            return await _productService.CreateAsync(dto);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var productModel = TestDataFactory.CreateProduct(0);
            var dto = productModel.Adapt<GoodsAndServiceDto>();
            dto.Id = 0;
            dto.Name = "Новый Товар";

            // Act
            var createdId = await _productService.CreateAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var productInDb = await db.GoodsAndServices.FirstOrDefaultAsync(p => p.Id == createdId);

            productInDb.Should().NotBeNull();
            productInDb!.Name.Should().Be(dto.Name);
            productInDb.IsDeleted.Should().BeFalse();
            productInDb.CreatedDate.Should().Be(DateOnly.FromDateTime(DateTime.Now));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProduct()
        {
            // Arrange
            var targetId = await CreateTestProductViaServiceAsync("Лицензия на ПО");

            // Act
            var result = await _productService.GetByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.Name.Should().Be("Лицензия на ПО");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingProduct()
        {
            // Arrange
            var targetId = await CreateTestProductViaServiceAsync("Старое Название Услуги");

            var updateDto = await _productService.GetByIdAsync(targetId);
            updateDto.Should().NotBeNull();

            updateDto!.Name = "Обновленное Название Услуги";
            updateDto.Price = 9999m;

            // Act
            await _productService.UpdateAsync(updateDto);

            // Assert
            var result = await _productService.GetByIdAsync(targetId);
            result!.Name.Should().Be("Обновленное Название Услуги");
            result.Price.Should().Be(9999m);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteProduct()
        {
            // Arrange
            var targetId = await CreateTestProductViaServiceAsync("Удаляемый товар");

            // Act
            await _productService.DeleteAsync(targetId);

            // Assert
            using var db = CreateContext();
            var productInDb = await db.GoodsAndServices.FirstOrDefaultAsync(p => p.Id == targetId);

            productInDb.Should().NotBeNull();
            productInDb!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            // Arrange
            var id1 = await CreateTestProductViaServiceAsync("Товар 1");
            var id2 = await CreateTestProductViaServiceAsync("Товар 2");

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            var dtos = result.ToList();

            dtos.Should().Contain(p => p.Id == id1);
            dtos.Should().Contain(p => p.Id == id2);
        }
    }
}