using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Unit.Data;
using FluentAssertions;
using Moq;

namespace ContractCreator.Tests.Unit.Services
{
    public class ContractActServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IRepository<ContractAct>> _repoMock = new();
        private readonly ContractActService _service;

        public ContractActServiceTests()
        {
            MappingConfig.Configure();

            _uowMock.Setup(x => x.Repository<ContractAct>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new ContractActService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto()
        {
            // Arrange
            var entity = TestDataFactory.CreateAct(1);
            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<ContractAct>>()))
                     .ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.ActNumber.Should().Be(entity.ActNumber);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSave()
        {
            // Arrange
            var dto = new ContractActDto { Id = 1, ContractId = 1, ActNumber = "A-1" };

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.IsAny<ContractAct>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave()
        {
            // Arrange
            var entity = TestDataFactory.CreateAct(1);
            var dto = new ContractActDto { Id = 1, ActNumber = "NEW-A" };
            _repoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<ContractAct>>()))
                     .ReturnsAsync(entity);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            entity.ActNumber.Should().Be("NEW-A");
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            // Arrange
            var entity = TestDataFactory.CreateAct(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _repoMock.Verify(x => x.DeleteAsync(entity), Times.Once);
        }
    }
}
