using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ContractActServiceTests : IntegrationTestBase
    {
        private readonly ContractActService _actService;

        public ContractActServiceTests() : base()
        {
            _actService = new ContractActService(UowFactory);
        }

        private async Task<(int ContractId, int InvoiceId)> SeedDependenciesAsync()
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

            var invoice = TestDataFactory.CreateInvoice(0, contract.Id);
            db.ContractInvoices.Add(invoice);
            await db.SaveChangesAsync();

            return (contract.Id, invoice.Id);
        }

        private async Task<int> SeedActViaServiceAsync(int contractId, int invoiceId, string number = "ACT-TEST")
        {
            var model = TestDataFactory.CreateAct(0, contractId, invoiceId);
            var dto = model.Adapt<ContractActDto>();
            dto.Id = 0;
            dto.ActNumber = number;

            return await _actService.CreateAsync(dto);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddActToDatabase()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var model = TestDataFactory.CreateAct(0, deps.ContractId, deps.InvoiceId);
            var dto = model.Adapt<ContractActDto>();
            dto.Id = 0;
            dto.ActNumber = "ACT-NEW";

            // Act
            var createdId = await _actService.CreateAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var actInDb = await db.ContractActs.FirstOrDefaultAsync(a => a.Id == createdId);

            actInDb.Should().NotBeNull();
            actInDb!.ActNumber.Should().Be("ACT-NEW");
            actInDb.ContractId.Should().Be(deps.ContractId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectAct()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedActViaServiceAsync(deps.ContractId, deps.InvoiceId, "ACT-SEARCH");

            // Act
            var result = await _actService.GetByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.ActNumber.Should().Be("ACT-SEARCH");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingAct()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedActViaServiceAsync(deps.ContractId, deps.InvoiceId, "ACT-OLD");
            var updateDto = await _actService.GetByIdAsync(targetId);

            updateDto!.ActNumber = "ACT-UPDATED";
            updateDto.TotalAmount = 8888m;

            // Act
            await _actService.UpdateAsync(updateDto);

            // Assert
            var result = await _actService.GetByIdAsync(targetId);
            result!.ActNumber.Should().Be("ACT-UPDATED");
            result.TotalAmount.Should().Be(8888m);
        }

        [Fact]
        public async Task DeleteAsync_ShouldHardDeleteAct()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedActViaServiceAsync(deps.ContractId, deps.InvoiceId);

            // Act
            await _actService.DeleteAsync(targetId);

            // Assert
            using var db = CreateContext();
            var actInDb = await db.ContractActs.FirstOrDefaultAsync(a => a.Id == targetId);
            actInDb.Should().BeNull();
        }

        [Fact]
        public async Task GetByContractIdAsync_ShouldReturnOnlyForSpecificContract()
        {
            // Arrange
            var targetDeps = await SeedDependenciesAsync();
            var otherDeps = await SeedDependenciesAsync();

            var id1 = await SeedActViaServiceAsync(targetDeps.ContractId, targetDeps.InvoiceId, "ACT-A");
            var id2 = await SeedActViaServiceAsync(targetDeps.ContractId, targetDeps.InvoiceId, "ACT-B");
            var id3 = await SeedActViaServiceAsync(otherDeps.ContractId, otherDeps.InvoiceId, "ACT-OTHER");

            // Act
            var result = await _actService.GetByContractIdAsync(targetDeps.ContractId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().HaveCount(2);
            dtos.Should().Contain(a => a.Id == id1);
            dtos.Should().Contain(a => a.Id == id2);
            dtos.Should().NotContain(a => a.Id == id3);
        }
    }
}
