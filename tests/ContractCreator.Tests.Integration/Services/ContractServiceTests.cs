using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ContractServiceTests : IntegrationTestBase
    {
        private readonly ContractService _contractService;

        public ContractServiceTests() : base()
        {
            _contractService = new ContractService(UowFactory);
        }

        private async Task<(int FirmId, int CounterpartyId)> CreateDependenciesAsync()
        {
            using var db = CreateContext();

            var firm = TestDataFactory.CreateFirm(0);
            db.Firms.Add(firm);
            await db.SaveChangesAsync();

            var cp = TestDataFactory.CreateCounterparty(0, firm.Id);
            db.Counterparties.Add(cp);
            await db.SaveChangesAsync();

            return (firm.Id, cp.Id);
        }

        private async Task<int> CreateTestContractViaServiceAsync(int firmId, int counterpartyId, string contractNumber = "DOC-TEST")
        {
            var model = TestDataFactory.CreateContract(0, firmId, counterpartyId);
            var dto = model.Adapt<ContractDto>();
            dto.Id = 0;
            dto.ContractNumber = contractNumber;

            return await _contractService.CreateContractAsync(dto);
        }

        [Fact]
        public async Task CreateContractAsync_ShouldAddContractToDatabase()
        {
            // Arrange
            var deps = await CreateDependenciesAsync();

            var model = TestDataFactory.CreateContract(0, deps.FirmId, deps.CounterpartyId);
            var dto = model.Adapt<ContractDto>();
            dto.Id = 0;
            dto.ContractNumber = "DOC-NEW-1";

            // Act
            var createdId = await _contractService.CreateContractAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var checkDb = CreateContext();
            var contractInDb = await checkDb.Contracts.FirstOrDefaultAsync(c => c.Id == createdId);

            contractInDb.Should().NotBeNull();
            contractInDb!.ContractNumber.Should().Be("DOC-NEW-1");
            contractInDb.FirmId.Should().Be(deps.FirmId);
        }

        [Fact]
        public async Task GetContractByIdAsync_ShouldReturnCorrectContract()
        {
            // Arrange
            var deps = await CreateDependenciesAsync();
            var targetId = await CreateTestContractViaServiceAsync(deps.FirmId, deps.CounterpartyId, "DOC-SEARCH");

            // Act
            var result = await _contractService.GetContractByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.ContractNumber.Should().Be("DOC-SEARCH");
        }

        [Fact]
        public async Task UpdateContractAsync_ShouldModifyExistingContract()
        {
            // Arrange
            var deps = await CreateDependenciesAsync();
            var targetId = await CreateTestContractViaServiceAsync(deps.FirmId, deps.CounterpartyId, "DOC-OLD");

            var updateDto = await _contractService.GetContractByIdAsync(targetId);

            updateDto!.ContractNumber = "DOC-UPDATED";
            updateDto.ContractPrice = 999999m;

            // Act
            await _contractService.UpdateContractAsync(updateDto);

            // Assert
            var result = await _contractService.GetContractByIdAsync(targetId);
            result!.ContractNumber.Should().Be("DOC-UPDATED");
            result.ContractPrice.Should().Be(999999m);
        }

        [Fact]
        public async Task DeleteContractAsync_ShouldHardDeleteContract()
        {
            // Arrange
            var deps = await CreateDependenciesAsync();
            var targetId = await CreateTestContractViaServiceAsync(deps.FirmId, deps.CounterpartyId);

            // Act
            await _contractService.DeleteContractAsync(targetId);

            // Assert
            using var db = CreateContext();
            var contractInDb = await db.Contracts.FirstOrDefaultAsync(c => c.Id == targetId);
            contractInDb.Should().BeNull();
        }

        [Fact]
        public async Task GetContractsByFirmIdAsync_ShouldReturnOnlyForSpecificFirm()
        {
            // Arrange
            var targetFirmDeps = await CreateDependenciesAsync();
            var otherFirmDeps = await CreateDependenciesAsync();

            var id1 = await CreateTestContractViaServiceAsync(targetFirmDeps.FirmId, targetFirmDeps.CounterpartyId, "DOC-15-A");
            var id2 = await CreateTestContractViaServiceAsync(targetFirmDeps.FirmId, targetFirmDeps.CounterpartyId, "DOC-15-B");
            var id3 = await CreateTestContractViaServiceAsync(otherFirmDeps.FirmId, otherFirmDeps.CounterpartyId, "DOC-25");

            // Act
            var result = await _contractService.GetContractsByFirmIdAsync(targetFirmDeps.FirmId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().HaveCount(2);
            dtos.Should().Contain(c => c.Id == id1);
            dtos.Should().Contain(c => c.Id == id2);
            dtos.Should().NotContain(c => c.Id == id3);
        }

        [Fact]
        public async Task GetAllStagesAsync_ShouldReturnStagesFromDictionary()
        {
            // Arrange

            // Act
            var result = await _contractService.GetAllStagesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task SaveContractWithDetailsAsync_ShouldCreateNewContractWithDetails()
        {
            // Arrange
            var deps = await CreateDependenciesAsync();

            var contractDto = TestDataFactory.CreateContract(0, deps.FirmId, deps.CounterpartyId).Adapt<ContractDto>();
            contractDto.Id = 0;
            contractDto.ContractNumber = "DOC-COMPLEX";
            contractDto.StageTypeId = 1;

            var specDto = TestDataFactory.CreateSpecification(0, 0).Adapt<ContractSpecificationDto>();
            specDto.Id = 0;

            var stepDto = TestDataFactory.CreateStep(0, 0).Adapt<ContractStepDto>();
            stepDto.Id = 0;

            // Act
            var createdId = await _contractService.SaveContractWithDetailsAsync(
                contractDto,
                new List<ContractSpecificationDto> { specDto },
                new List<ContractStepDto> { stepDto },
                currentWorkerId: 1);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var checkDb = CreateContext();
            var contractInDb = await checkDb.Contracts
                .Include(c => c.Specifications)
                .Include(c => c.Steps)
                .FirstOrDefaultAsync(c => c.Id == createdId);

            contractInDb.Should().NotBeNull();
            contractInDb!.ContractNumber.Should().Be("DOC-COMPLEX");
            contractInDb.Specifications.Should().HaveCount(1);
            contractInDb.Steps.Should().HaveCount(1);
        }
    }
}