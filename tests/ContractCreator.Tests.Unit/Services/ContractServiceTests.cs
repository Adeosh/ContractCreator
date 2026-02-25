using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Contracts;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.Enums;
using ContractCreator.Tests.Unit.Data;
using FluentAssertions;
using Moq;
namespace ContractCreator.Tests.Unit.Services
{
    public class ContractServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<Contract>> _contractRepoMock;
        private readonly ContractService _service;
        private readonly Mock<IRepository<ContractSpecification>> _specRepoMock;
        private readonly Mock<IRepository<ContractStep>> _stepRepoMock;
        private readonly Mock<IRepository<ContractStageChangeHistory>> _historyRepoMock;

        public ContractServiceTests()
        {
            MappingConfig.Configure();

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            _uowMock = new Mock<IUnitOfWork>();
            _contractRepoMock = new Mock<IRepository<Contract>>();
            _specRepoMock = new Mock<IRepository<ContractSpecification>>();
            _stepRepoMock = new Mock<IRepository<ContractStep>>();
            _historyRepoMock = new Mock<IRepository<ContractStageChangeHistory>>();

            _uowMock.Setup(x => x.Repository<Contract>()).Returns(_contractRepoMock.Object);
            _uowMock.Setup(x => x.Repository<ContractSpecification>()).Returns(_specRepoMock.Object);
            _uowMock.Setup(x => x.Repository<ContractStep>()).Returns(_stepRepoMock.Object);
            _uowMock.Setup(x => x.Repository<ContractStageChangeHistory>()).Returns(_historyRepoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);

            _service = new ContractService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetContractByIdAsync_ShouldReturnDto_WhenContractExists()
        {
            // Arrange
            var contractId = 1;
            var existingContract = new Contract
            {
                Id = contractId,
                FirmId = 10,
                CounterpartyId = 20,
                Type = ContractType.Contract,
                EnterpriseRole = ContractEnterpriseRole.Contractor,
                ContractNumber = "№ 123-А",
                ContractPrice = 15000.00m,
                ContractSubject = "Оказание услуг",
                IssueDate = new DateOnly(2024, 1, 1),
                CurrencyId = 1,
                StageTypeId = 1,
                Currency = TestDataFactory.CreateCurrencyRUB(),
                Counterparty = TestDataFactory.CreateCounterparty(),
                Firm = TestDataFactory.CreateFirm()
            };

            _contractRepoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ContractByIdWithDetailsSpec>()))
                .ReturnsAsync(existingContract);

            // Act
            var result = await _service.GetContractByIdAsync(contractId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(contractId);
            result.ContractNumber.Should().Be("№ 123-А");
            result.ContractPrice.Should().Be(15000.00m);
            result.Type.Should().Be(ContractType.Contract);
            result.EnterpriseRole.Should().Be(ContractEnterpriseRole.Contractor);
        }

        [Fact]
        public async Task GetContractByIdAsync_ShouldReturnNull_WhenContractDoesNotExist()
        {
            // Arrange
            _contractRepoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ContractByIdWithDetailsSpec>()))
                .ReturnsAsync((Contract?)null);

            // Act
            var result = await _service.GetContractByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateContractAsync_ShouldCallAddAndSave_AndReturnNewId()
        {
            // Arrange
            var dto = new ContractDto
            {
                Id = 1,
                FirmId = 10,
                CounterpartyId = 20,
                Type = ContractType.Agreement,
                EnterpriseRole = ContractEnterpriseRole.Customer,
                ContractNumber = "AGR-001",
                ContractPrice = 50000m,
                ContractSubject = "Поставка оборудования",
                CurrencyId = 1,
                StageTypeId = 1
            };

            // Act
            var resultId = await _service.CreateContractAsync(dto);

            // Assert
            _contractRepoMock.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateContractAsync_ShouldUpdateEntity_WhenContractExists()
        {
            // Arrange
            var contractId = 5;

            var existingContract = new Contract
            {
                Id = contractId,
                FirmId = 10,
                ContractNumber = "OLD-123",
                ContractPrice = 100m,
                CurrencyId = 1,
                StageTypeId = 1,
                Type = ContractType.Contract
            };

            var updateDto = new ContractDto
            {
                Id = contractId,
                FirmId = 10,
                ContractNumber = "NEW-999",
                ContractPrice = 200m,
                CurrencyId = 1,
                StageTypeId = 2,
                Type = ContractType.Contract
            };

            _contractRepoMock.Setup(x => x.GetByIdAsync(contractId)).ReturnsAsync(existingContract);

            // Act
            await _service.UpdateContractAsync(updateDto);

            // Assert
            existingContract.ContractNumber.Should().Be("NEW-999");
            existingContract.ContractPrice.Should().Be(200m);
            existingContract.StageTypeId.Should().Be(2);

            _contractRepoMock.Verify(x => x.UpdateAsync(existingContract), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateContractAsync_ShouldThrowException_WhenContractDoesNotExist()
        {
            // Arrange
            var dto = new ContractDto { Id = 999 };

            _contractRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Contract?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.UpdateContractAsync(dto));
            exception.Message.Should().Be("Контракт не найден");

            _contractRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Contract>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteContractAsync_ShouldCallDelete_WhenContractExists()
        {
            // Arrange
            var contractId = 5;
            var contract = new Contract { Id = contractId, FirmId = 1, ContractNumber = "123", CurrencyId = 1, StageTypeId = 1 };

            _contractRepoMock.Setup(x => x.GetByIdAsync(contractId)).ReturnsAsync(contract);

            // Act
            await _service.DeleteContractAsync(contractId);

            // Assert
            _contractRepoMock.Verify(x => x.DeleteAsync(contract), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteContractAsync_ShouldDoNothing_WhenContractDoesNotExist()
        {
            // Arrange
            _contractRepoMock.Setup(x => x.GetByIdAsync(777)).ReturnsAsync((Contract?)null);

            // Act
            await _service.DeleteContractAsync(777);

            // Assert
            _contractRepoMock.Verify(x => x.DeleteAsync(It.IsAny<Contract>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetContractsByFirmIdAsync_ShouldReturnMappedList_FilteredByFirmId()
        {
            // Arrange
            var firmIdToSearch = 10;
            var contractsList = new List<Contract>
            {
                new Contract
                {
                    Id = 1,
                    FirmId = firmIdToSearch,
                    ContractNumber = "DOC-1",
                    ContractPrice = 1000m,
                    Counterparty = TestDataFactory.CreateCounterparty(),
                    Currency = TestDataFactory.CreateCurrencyRUB()
                },
                new Contract
                {
                    Id = 2,
                    FirmId = firmIdToSearch,
                    ContractNumber = "DOC-2",
                    ContractPrice = 2000m,
                    Counterparty = TestDataFactory.CreateCounterparty(),
                    Currency = TestDataFactory.CreateCurrencyRUB()
                }
            };

            _contractRepoMock
                .Setup(x => x.ListAsync(It.IsAny<ContractsByFirmIdSpec>()))
                .ReturnsAsync(contractsList);

            // Act
            var result = await _service.GetContractsByFirmIdAsync(firmIdToSearch);

            // Assert
            result.Should().HaveCount(2);
            result.First().ContractNumber.Should().Be("DOC-1");
            result.Last().ContractPrice.Should().Be(2000m);
        }

        [Fact]
        public async Task SaveContractWithDetailsAsync_ShouldCreateNewContract_AndDetails_WhenIdIsZero()
        {
            // Arrange
            var workerId = 99;
            var dto = new ContractDto { Id = 0, ContractNumber = "NEW-123", StageTypeId = 2 };

            var specs = new List<ContractSpecificationDto> { new ContractSpecificationDto { NomenclatureName = "Товар 1" } };
            var steps = new List<ContractStepDto> { new ContractStepDto { StepName = "Этап 1" } };

            _contractRepoMock.Setup(x => x.AddAsync(It.IsAny<Contract>()))
                .Callback<Contract>(c => c.Id = 10)
                .Returns(Task.CompletedTask);

            // Act
            var resultId = await _service.SaveContractWithDetailsAsync(dto, specs, steps, workerId);

            // Assert
            resultId.Should().Be(10);

            _contractRepoMock.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Once);
            _specRepoMock.Verify(x => x.AddAsync(It.Is<ContractSpecification>(s => s.ContractId == 10)), Times.Once);
            _stepRepoMock.Verify(x => x.AddAsync(It.Is<ContractStep>(s => s.ContractId == 10)), Times.Once);
            _historyRepoMock.Verify(x => x.AddAsync(It.Is<ContractStageChangeHistory>(h => h.StageTypeId == 2 && h.WorkerId == workerId)), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Exactly(2));
        }

        [Fact]
        public async Task SaveContractWithDetailsAsync_ShouldUpdateContract_AndReplaceDetails_WhenIdExists()
        {
            // Arrange
            var contractId = 5;
            var workerId = 99;

            var dto = new ContractDto { Id = contractId, ContractNumber = "UPDATED-123", StageTypeId = 3 };
            var newSpecs = new List<ContractSpecificationDto> { new ContractSpecificationDto { NomenclatureName = "Новый Товар" } };
            var newSteps = new List<ContractStepDto> { new ContractStepDto { StepName = "Новый Этап" } };

            var existingContract = new Contract
            {
                Id = contractId,
                ContractNumber = "OLD-123",
                StageTypeId = 1,
                Specifications = new List<ContractSpecification> { new ContractSpecification { Id = 1, NomenclatureName = "Старый Товар" } },
                Steps = new List<ContractStep> { new ContractStep { Id = 1, StepName = "Старый Этап" } }
            };

            var lastHistory = new ContractStageChangeHistory { ContractId = contractId, StageTypeId = 1 };

            _contractRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ContractByIdWithDetailsSpec>())).ReturnsAsync(existingContract);
            _historyRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ContractStageHistoryByContractIdSpec>())).ReturnsAsync(lastHistory);

            // Act
            var resultId = await _service.SaveContractWithDetailsAsync(dto, newSpecs, newSteps, workerId);

            // Assert
            resultId.Should().Be(contractId);
            existingContract.ContractNumber.Should().Be("UPDATED-123");

            _contractRepoMock.Verify(x => x.UpdateAsync(existingContract), Times.Once);
            _specRepoMock.Verify(x => x.DeleteAsync(It.Is<ContractSpecification>(s => s.NomenclatureName == "Старый Товар")), Times.Once);
            _stepRepoMock.Verify(x => x.DeleteAsync(It.Is<ContractStep>(s => s.StepName == "Старый Этап")), Times.Once);
            _specRepoMock.Verify(x => x.AddAsync(It.Is<ContractSpecification>(s => s.NomenclatureName == "Новый Товар")), Times.Once);
            _stepRepoMock.Verify(x => x.AddAsync(It.Is<ContractStep>(s => s.StepName == "Новый Этап")), Times.Once);
            _historyRepoMock.Verify(x => x.AddAsync(It.Is<ContractStageChangeHistory>(h => h.StageTypeId == 3)), Times.Once);
        }
    }
}
