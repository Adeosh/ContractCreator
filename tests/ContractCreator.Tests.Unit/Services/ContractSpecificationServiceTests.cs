using ContractCreator.Application.Interfaces;
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
    public class ContractSpecificationServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<ContractSpecification>> _repoMock;
        private readonly ContractSpecificationService _service;

        public ContractSpecificationServiceTests()
        {
            MappingConfig.Configure();

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            _uowMock = new Mock<IUnitOfWork>();
            _repoMock = new Mock<IRepository<ContractSpecification>>();
            _uowMock.Setup(x => x.Repository<ContractSpecification>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new ContractSpecificationService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetByContractIdAsync_ShouldReturnFilteredList()
        {
            var contractId = 5;
            var list = new List<ContractSpecification>
            {
                TestDataFactory.CreateSpecification(1, contractId),
                TestDataFactory.CreateSpecification(2, 999) // Другой контракт
            };
            _repoMock.Setup(x => x.ListAllAsync()).ReturnsAsync(list);

            var result = await _service.GetByContractIdAsync(contractId);

            result.Should().HaveCount(1);
            result.First().ContractId.Should().Be(contractId);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndSave()
        {
            var dto = new ContractSpecificationDto { Id = 1, ContractId = 1, NomenclatureName = "Test" };

            await _service.CreateAsync(dto);

            _repoMock.Verify(x => x.AddAsync(It.IsAny<ContractSpecification>()), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndSave()
        {
            var entity = TestDataFactory.CreateSpecification(1);
            var dto = new ContractSpecificationDto { Id = 1, NomenclatureName = "NewName" };
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await _service.UpdateAsync(dto);

            entity.NomenclatureName.Should().Be("NewName");
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete_WhenExists()
        {
            var entity = TestDataFactory.CreateSpecification(1);
            _repoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await _service.DeleteAsync(1);

            _repoMock.Verify(x => x.DeleteAsync(entity), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
