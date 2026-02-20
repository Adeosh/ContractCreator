using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.Enums;
using FluentAssertions;
using Moq;

namespace ContractCreator.Tests.Unit.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<GoodsAndService>> _repoMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            MappingConfig.Configure();

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            _uowMock = new Mock<IUnitOfWork>();
            _repoMock = new Mock<IRepository<GoodsAndService>>();
            _uowMock.Setup(x => x.Repository<GoodsAndService>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new ProductService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnGoodsAndServices_WithCurrency()
        {
            // Arrange
            var list = new List<GoodsAndService>
            {
                new GoodsAndService
                {
                    Id = 1,
                    Type = ProductType.Good,
                    Name = "Laptop",
                    Price = 1000,
                    Currency = new ClassifierOkv { LetterCode = "USD", CurrencyName = "Dollar", CountriesCurrencyUsed = "USA" }
                },
                new GoodsAndService
                {
                    Id = 2,
                    Type = ProductType.Service,
                    Name = "Delivery",
                    Price = 50,
                    Currency = new ClassifierOkv { LetterCode = "RUB", CurrencyName = "Ruble", CountriesCurrencyUsed = "RU" }
                }
            };

            _repoMock.Setup(x => x.ListAsync(It.IsAny<ISpecification<GoodsAndService>>()))
                .ReturnsAsync(list);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);

            var good = result.First(x => x.Id == 1);
            good.TypeName.Should().Be("Товар");
            good.Type.Should().Be(1);

            var service = result.First(x => x.Id == 2);
            service.TypeName.Should().Be("Услуга");
            service.Type.Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
        {
            // Arrange
            var entity = new GoodsAndService
            {
                Id = 10,
                Type = ProductType.Good,
                Name = "Table",
                Currency = new ClassifierOkv { LetterCode = "EUR", CurrencyName = "Euro", CountriesCurrencyUsed = "EU" }
            };

            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<GoodsAndService>>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(10);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Table");
        }

        [Fact]
        public async Task CreateAsync_ShouldSaveAsGood_WhenTypeIs1()
        {
            // Arrange
            var dto = new GoodsAndServiceDto
            {
                Name = "New Laptop",
                Type = (byte)ProductType.Good,
                Price = 50000,
                CurrencyId = 95
            };

            // Act
            var resultId = await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.Is<GoodsAndService>(p =>
                p.Name == "New Laptop" &&
                p.Type == ProductType.Good && // <-- ВАЖНО: Проверяем, что сохранилось как Enum
                p.IsDeleted == false &&
                p.CreatedDate == DateOnly.FromDateTime(DateTime.Now)
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldSaveAsService_WhenTypeIs2()
        {
            // Arrange
            var dto = new GoodsAndServiceDto
            {
                Name = "Consulting",
                Type = (byte)ProductType.Service,
                Price = 1000,
                CurrencyId = 95
            };

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.Is<GoodsAndService>(p =>
                p.Name == "Consulting" &&
                p.Type == ProductType.Service
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFields_WhenFound()
        {
            // Arrange
            var id = 5;
            var entity = new GoodsAndService
            {
                Id = id,
                Name = "Old Name",
                Type = ProductType.Good
            };

            var updateDto = new GoodsAndServiceDto
            {
                Id = id,
                Name = "New Name",
                Type = (byte)ProductType.Service
            };

            _repoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert
            entity.Name.Should().Be("New Name");
            entity.Type.Should().Be(ProductType.Service);

            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDelete()
        {
            // Arrange
            var id = 7;
            var entity = new GoodsAndService { Id = id, IsDeleted = false, Name = "To Del" };

            _repoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            entity.IsDeleted.Should().BeTrue();

            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
