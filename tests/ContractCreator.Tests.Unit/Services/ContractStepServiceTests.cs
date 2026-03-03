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
    public class ContractStepServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IRepository<ContractStep>> _repoMock = new();
        private readonly ContractStepService _service;

        public ContractStepServiceTests()
        {
            MappingConfig.Configure();

            _uowMock.Setup(x => x.Repository<ContractStep>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new ContractStepService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetByContractIdAsync_ShouldReturnFilteredList()
        {
            // Arrange
            var contractId = 5;
            var list = new List<ContractStep>
            {
                TestDataFactory.CreateStep(1, contractId),
                TestDataFactory.CreateStep(2, 999)
            };
            _repoMock.Setup(x => x.ListAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _service.GetByContractIdAsync(contractId);

            // Assert
            result.Should().HaveCount(1);
            result.First().StepName.Should().Be("Этап 1");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSave()
        {
            // Arrange
            var dto = new ContractStepDto { Id = 1, ContractId = 1, StepName = "Этап 1" };

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.IsAny<ContractStep>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave()
        {
            // Arrange
            var entity = TestDataFactory.CreateStep(1);
            var dto = new ContractStepDto { Id = 1, StepName = "Новый этап" };
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            entity.StepName.Should().Be("Новый этап");
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            // Arrange
            var entity = TestDataFactory.CreateStep(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            await _service.DeleteAsync(1);

            // Assert
            _repoMock.Verify(x => x.DeleteAsync(entity), Times.Once);
        }
    }
}
