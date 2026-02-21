using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.Data;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ContractCreator.Tests.Unit.Services
{
    public class CounterpartyServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<Counterparty>> _repoMock;
        private readonly CounterpartyService _service;

        public CounterpartyServiceTests()
        {
            MappingConfig.Configure();

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            _uowMock = new Mock<IUnitOfWork>();
            _repoMock = new Mock<IRepository<Counterparty>>();
            _uowMock.Setup(x => x.Repository<Counterparty>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new CounterpartyService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllCounterpartiesAsync_ShouldReturnActiveOnly()
        {
            // Arrange
            var list = new List<Counterparty>
            {
                new Counterparty
                {
                    Id = 1, IsDeleted = false, FullName = "Active 1", ShortName = "A1", INN = "1", Phone = "1",
                    Email = new EmailAddress("a@a.ru"), LegalAddress = new AddressData(), ActualAddress = new AddressData()
                },
                new Counterparty
                {
                    Id = 2, IsDeleted = true, FullName = "Deleted", ShortName = "D", INN = "2", Phone = "2",
                    Email = new EmailAddress("d@d.ru"), LegalAddress = new AddressData(), ActualAddress = new AddressData()
                },
                new Counterparty
                {
                    Id = 3, IsDeleted = false, FullName = "Active 2", ShortName = "A2", INN = "3", Phone = "3",
                    Email = new EmailAddress("b@b.ru"), LegalAddress = new AddressData(), ActualAddress = new AddressData()
                }
            };

            _repoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Counterparty, bool>>>()))
                .ReturnsAsync((Expression<Func<Counterparty, bool>> predicate) =>
                {
                    return list.Where(predicate.Compile()).ToList();
                });

            // Act
            var result = await _service.GetAllCounterpartiesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.FullName == "Active 1");
            result.Should().Contain(x => x.FullName == "Active 2");
            result.Should().NotContain(x => x.FullName == "Deleted");
        }

        [Fact]
        public async Task GetCounterpartyByIdAsync_ShouldReturnDto_WhenFound()
        {
            // Arrange
            var id = 10;
            var entity = new Counterparty
            {
                Id = id,
                FullName = "Test Company",
                ShortName = "TC",
                INN = "123",
                Phone = "000",
                Email = new EmailAddress("test@test.ru"),
                LegalAddress = new AddressData(),
                ActualAddress = new AddressData()
            };

            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Counterparty>>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.GetCounterpartyByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.FullName.Should().Be("Test Company");
        }

        [Fact]
        public async Task GetCounterpartyByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Counterparty>>()))
                .ReturnsAsync((Counterparty?)null);

            // Act
            var result = await _service.GetCounterpartyByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateCounterpartyAsync_ShouldSetDateAndDefaults()
        {
            // Arrange
            var dto = new CounterpartyDto
            {
                FullName = "New Company",
                ShortName = "New Co",
                INN = "7777777777",
                Phone = "+7000",
                Email = "new@co.ru",
                LegalAddress = new AddressDto { FullAddress = "Street 1" }
            };

            // Act
            var resultId = await _service.CreateCounterpartyAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.Is<Counterparty>(c =>
                c.FullName == "New Company" &&
                c.IsDeleted == false &&
                c.CreatedDate == DateOnly.FromDateTime(DateTime.Now)
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateCounterpartyAsync_ShouldUpdate_WhenFound()
        {
            // Arrange
            var id = 5;
            var existingEntity = new Counterparty
            {
                Id = id,
                FullName = "Old Name",
                ShortName = "Old",
                INN = "111",
                Phone = "111",
                Email = new EmailAddress("old@co.ru"),
                LegalAddress = new AddressData(),
                ActualAddress = new AddressData()
            };

            var updateDto = new CounterpartyDto
            {
                Id = id,
                FullName = "New Name",
                ShortName = "New",
                INN = "222",
                Phone = "222",
                Email = "new@co.ru",
                LegalAddress = new AddressDto(),
                ActualAddress = new AddressDto()
            };

            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Counterparty>>()))
                .ReturnsAsync(existingEntity);

            // Act
            await _service.UpdateCounterpartyAsync(updateDto);

            // Assert
            existingEntity.FullName.Should().Be("New Name");
            existingEntity.INN.Should().Be("222");

            _repoMock.Verify(x => x.UpdateAsync(existingEntity), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateCounterpartyAsync_ShouldThrow_WhenNotFound()
        {
            // Arrange
            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Counterparty>>()))
                .ReturnsAsync((Counterparty?)null);

            var dto = new CounterpartyDto { Id = 999 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateCounterpartyAsync(dto));
            ex.Message.Should().Be("Контрагент не найден");

            _repoMock.Verify(x => x.UpdateAsync(It.IsAny<Counterparty>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCounterpartyAsync_ShouldSoftDelete_WhenFound()
        {
            // Arrange
            var id = 10;
            var entity = new Counterparty
            {
                Id = id,
                IsDeleted = false,
                FullName = "To Del",
                ShortName = "D",
                INN = "1",
                Phone = "1",
                Email = new EmailAddress("a@a.ru"),
                LegalAddress = new AddressData(),
                ActualAddress = new AddressData()
            };

            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Counterparty>>()))
                .ReturnsAsync(entity);

            // Act
            await _service.DeleteCounterpartyAsync(id);

            // Assert
            entity.IsDeleted.Should().BeTrue(); // Флаг изменился

            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteCounterpartyAsync_ShouldDoNothing_WhenNotFound()
        {
            // Arrange
            _repoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Counterparty?)null);

            // Act
            await _service.DeleteCounterpartyAsync(999);

            // Assert
            _repoMock.Verify(x => x.UpdateAsync(It.IsAny<Counterparty>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }
    }
}
