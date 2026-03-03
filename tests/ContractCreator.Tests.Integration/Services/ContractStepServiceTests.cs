using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ContractStepServiceTests : IntegrationTestBase
    {
        private readonly ContractStepService _stepService;

        public ContractStepServiceTests() : base()
        {
            _stepService = new ContractStepService(UowFactory);
        }

        private async Task<int> SeedDependenciesAsync()
        {
            using var db = CreateContext();

            var firm = TestDataFactory.CreateFirm(0);
            db.Firms.Add(firm);
            await db.SaveChangesAsync();

            var cp = TestDataFactory.CreateCounterparty(0, firm.Id);
            db.Counterparties.Add(cp);
            await db.SaveChangesAsync();

            var contract = TestDataFactory.CreateContract(0, firm.Id, cp.Id);
            db.Contracts.Add(contract);
            await db.SaveChangesAsync();

            return contract.Id;
        }

        private async Task<int> SeedStepInDbAsync(int contractId, string stepName = "Этап 1")
        {
            using var db = CreateContext();

            var step = TestDataFactory.CreateStep(0, contractId);
            step.StepName = stepName;

            db.ContractSteps.Add(step);
            await db.SaveChangesAsync();

            return step.Id;
        }

        [Fact]
        public async Task CreateAsync_ShouldAddStepToDatabase()
        {
            // Arrange
            var contractId = await SeedDependenciesAsync();
            var model = TestDataFactory.CreateStep(0, contractId);
            var dto = model.Adapt<ContractStepDto>();
            dto.Id = 0;
            dto.StepName = "Новый Этап";

            // Act
            var createdId = await _stepService.CreateAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var stepInDb = await db.ContractSteps.FirstOrDefaultAsync(s => s.Id == createdId);

            stepInDb.Should().NotBeNull();
            stepInDb!.StepName.Should().Be("Новый Этап");
            stepInDb.ContractId.Should().Be(contractId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectStep()
        {
            // Arrange
            var contractId = await SeedDependenciesAsync();
            var targetId = await SeedStepInDbAsync(contractId, "Искомый Этап");

            // Act
            var result = await _stepService.GetByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.StepName.Should().Be("Искомый Этап");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingStep()
        {
            // Arrange
            var contractId = await SeedDependenciesAsync();
            var targetId = await SeedStepInDbAsync(contractId, "Старое Название");
            var updateDto = await _stepService.GetByIdAsync(targetId);

            updateDto!.StepName = "Обновленное Название";
            updateDto.TotalAmount = 12345m;

            // Act
            await _stepService.UpdateAsync(updateDto);

            // Assert
            var result = await _stepService.GetByIdAsync(targetId);
            result!.StepName.Should().Be("Обновленное Название");
            result.TotalAmount.Should().Be(12345m);
        }

        [Fact]
        public async Task DeleteAsync_ShouldHardDeleteStep()
        {
            // Arrange
            var contractId = await SeedDependenciesAsync();
            var targetId = await SeedStepInDbAsync(contractId);

            // Act
            await _stepService.DeleteAsync(targetId);

            // Assert
            using var db = CreateContext();
            var stepInDb = await db.ContractSteps.FirstOrDefaultAsync(s => s.Id == targetId);
            stepInDb.Should().BeNull();
        }

        [Fact]
        public async Task GetByContractIdAsync_ShouldReturnOnlyForSpecificContract()
        {
            // Arrange
            var targetContractId = await SeedDependenciesAsync();
            var otherContractId = await SeedDependenciesAsync();

            var id1 = await SeedStepInDbAsync(targetContractId, "Этап 100 - А");
            var id2 = await SeedStepInDbAsync(targetContractId, "Этап 100 - Б");
            var id3 = await SeedStepInDbAsync(otherContractId, "Этап 200");

            // Act
            var result = await _stepService.GetByContractIdAsync(targetContractId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().HaveCount(2);
            dtos.Should().Contain(s => s.Id == id1);
            dtos.Should().Contain(s => s.Id == id2);
            dtos.Should().NotContain(s => s.Id == id3);
        }
    }
}