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
            var entity = TestDataFactory.CreateAct(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSave()
        {
            var dto = new ContractActDto { Id = 1, ContractId = 1, ActNumber = "A-1" };
            await _service.CreateAsync(dto);
            _repoMock.Verify(x => x.AddAsync(It.IsAny<ContractAct>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave()
        {
            var entity = TestDataFactory.CreateAct(1);
            var dto = new ContractActDto { Id = 1, ActNumber = "NEW-A" };
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await _service.UpdateAsync(dto);
            entity.ActNumber.Should().Be("NEW-A");
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            var entity = TestDataFactory.CreateAct(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            await _service.DeleteAsync(1);
            _repoMock.Verify(x => x.DeleteAsync(entity), Times.Once);
        }
    }
}
