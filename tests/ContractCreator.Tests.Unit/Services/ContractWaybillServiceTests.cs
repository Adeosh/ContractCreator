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
    public class ContractWaybillServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IRepository<ContractWaybill>> _repoMock = new();
        private readonly ContractWaybillService _service;

        public ContractWaybillServiceTests()
        {
            MappingConfig.Configure();

            _uowMock.Setup(x => x.Repository<ContractWaybill>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new ContractWaybillService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto()
        {
            var entity = TestDataFactory.CreateWaybill(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);
            result.Should().NotBeNull();
            result!.WaybillNumber.Should().Be(entity.WaybillNumber);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSave()
        {
            var dto = new ContractWaybillDto { Id = 1, ContractId = 1, WaybillNumber = "WB-1" };
            await _service.CreateAsync(dto);
            _repoMock.Verify(x => x.AddAsync(It.IsAny<ContractWaybill>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave()
        {
            var entity = TestDataFactory.CreateWaybill(1);
            var dto = new ContractWaybillDto { Id = 1, WaybillNumber = "NEW-WB" };
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await _service.UpdateAsync(dto);
            entity.WaybillNumber.Should().Be("NEW-WB");
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            var entity = TestDataFactory.CreateWaybill(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            await _service.DeleteAsync(1);
            _repoMock.Verify(x => x.DeleteAsync(entity), Times.Once);
        }
    }
}
