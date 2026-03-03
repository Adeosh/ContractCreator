using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ContractWaybillServiceTests : IntegrationTestBase
    {
        private readonly ContractWaybillService _waybillService;

        public ContractWaybillServiceTests() : base()
        {
            _waybillService = new ContractWaybillService(UowFactory);
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

        private async Task<int> SeedWaybillViaServiceAsync(int contractId, int invoiceId, string number = "WB-TEST")
        {
            var model = TestDataFactory.CreateWaybill(0, contractId, invoiceId);
            var dto = model.Adapt<ContractWaybillDto>();
            dto.Id = 0;
            dto.WaybillNumber = number;

            return await _waybillService.CreateAsync(dto);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddWaybillToDatabase()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var model = TestDataFactory.CreateWaybill(0, deps.ContractId, deps.InvoiceId);
            var dto = model.Adapt<ContractWaybillDto>();
            dto.Id = 0;
            dto.WaybillNumber = "WB-NEW";

            // Act
            var createdId = await _waybillService.CreateAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var waybillInDb = await db.ContractWaybills.FirstOrDefaultAsync(w => w.Id == createdId);

            waybillInDb.Should().NotBeNull();
            waybillInDb!.WaybillNumber.Should().Be("WB-NEW");
            waybillInDb.ContractId.Should().Be(deps.ContractId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectWaybill()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedWaybillViaServiceAsync(deps.ContractId, deps.InvoiceId, "WB-SEARCH");

            // Act
            var result = await _waybillService.GetByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.WaybillNumber.Should().Be("WB-SEARCH");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingWaybill()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedWaybillViaServiceAsync(deps.ContractId, deps.InvoiceId, "WB-OLD");
            var updateDto = await _waybillService.GetByIdAsync(targetId);

            updateDto!.WaybillNumber = "WB-UPDATED";
            updateDto.TotalAmount = 5555m;

            // Act
            await _waybillService.UpdateAsync(updateDto);

            // Assert
            var result = await _waybillService.GetByIdAsync(targetId);
            result!.WaybillNumber.Should().Be("WB-UPDATED");
            result.TotalAmount.Should().Be(5555m);
        }

        [Fact]
        public async Task DeleteAsync_ShouldHardDeleteWaybill()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedWaybillViaServiceAsync(deps.ContractId, deps.InvoiceId);

            // Act
            await _waybillService.DeleteAsync(targetId);

            // Assert
            using var db = CreateContext();
            var waybillInDb = await db.ContractWaybills.FirstOrDefaultAsync(w => w.Id == targetId);
            waybillInDb.Should().BeNull();
        }

        [Fact]
        public async Task GetByContractIdAsync_ShouldReturnOnlyForSpecificContract()
        {
            // Arrange
            var targetDeps = await SeedDependenciesAsync();
            var otherDeps = await SeedDependenciesAsync();

            var id1 = await SeedWaybillViaServiceAsync(targetDeps.ContractId, targetDeps.InvoiceId, "WB-A");
            var id2 = await SeedWaybillViaServiceAsync(targetDeps.ContractId, targetDeps.InvoiceId, "WB-B");
            var id3 = await SeedWaybillViaServiceAsync(otherDeps.ContractId, otherDeps.InvoiceId, "WB-OTHER");

            // Act
            var result = await _waybillService.GetByContractIdAsync(targetDeps.ContractId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().HaveCount(2);
            dtos.Should().Contain(w => w.Id == id1);
            dtos.Should().Contain(w => w.Id == id2);
            dtos.Should().NotContain(w => w.Id == id3);
        }
    }
}