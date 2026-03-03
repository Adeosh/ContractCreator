using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Domain.Models;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class BankAccountServiceTests : IntegrationTestBase
    {
        private readonly BankAccountService _bankAccountService;

        public BankAccountServiceTests() : base()
        {
            _bankAccountService = new BankAccountService(UowFactory);
        }

        private async Task<(int FirmId, int CounterpartyId)> SeedDependenciesAsync()
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

        private async Task<int> SeedBankAccountInDbAsync(int? firmId, int? counterpartyId, string bankName = "Тест Банк")
        {
            using var db = CreateContext();

            var account = new BankAccount
            {
                FirmId = firmId,
                CounterpartyId = counterpartyId,
                BankName = bankName,
                BIC = "044525225",
                AccountNumber = "40702810000000000001",
                IsDeleted = false
            };

            db.BankAccounts.Add(account);
            await db.SaveChangesAsync();

            return account.Id;
        }

        [Fact]
        public async Task CreateAsync_ShouldAddBankAccountToDatabase()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var dto = new BankAccountDto
            {
                FirmId = deps.FirmId,
                BankName = "Новый Банк",
                BIC = "123456789",
                AccountNumber = "11122233344455566677"
            };

            // Act
            var createdId = await _bankAccountService.CreateAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var accountInDb = await db.BankAccounts.FirstOrDefaultAsync(a => a.Id == createdId);

            accountInDb.Should().NotBeNull();
            accountInDb!.BankName.Should().Be("Новый Банк");
            accountInDb.FirmId.Should().Be(deps.FirmId);
            accountInDb.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectBankAccount()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedBankAccountInDbAsync(deps.FirmId, null, "Искомый Банк");

            // Act
            var result = await _bankAccountService.GetByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.BankName.Should().Be("Искомый Банк");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingBankAccount()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedBankAccountInDbAsync(deps.FirmId, null, "Старый Банк");
            var updateDto = await _bankAccountService.GetByIdAsync(targetId);

            updateDto!.BankName = "Обновленный Банк";
            updateDto.BIC = "999999999";

            // Act
            await _bankAccountService.UpdateAsync(updateDto);

            // Assert
            var result = await _bankAccountService.GetByIdAsync(targetId);
            result!.BankName.Should().Be("Обновленный Банк");
            result.BIC.Should().Be("999999999");
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteBankAccount()
        {
            // Arrange
            var deps = await SeedDependenciesAsync();
            var targetId = await SeedBankAccountInDbAsync(null, deps.CounterpartyId);

            // Act
            await _bankAccountService.DeleteAsync(targetId);

            // Assert
            using var db = CreateContext();
            var accountInDb = await db.BankAccounts.FirstOrDefaultAsync(a => a.Id == targetId);
            accountInDb.Should().NotBeNull();
            accountInDb!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetByFirmIdAsync_ShouldReturnOnlyActiveForSpecificFirm()
        {
            // Arrange
            var targetDeps = await SeedDependenciesAsync();
            var otherDeps = await SeedDependenciesAsync();

            var id1 = await SeedBankAccountInDbAsync(targetDeps.FirmId, null);
            var id2 = await SeedBankAccountInDbAsync(targetDeps.FirmId, null);
            var id3 = await SeedBankAccountInDbAsync(otherDeps.FirmId, null);

            await _bankAccountService.DeleteAsync(id2);

            // Act
            var result = await _bankAccountService.GetByFirmIdAsync(targetDeps.FirmId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().ContainSingle();
            dtos.Should().Contain(a => a.Id == id1);
            dtos.Should().NotContain(a => a.Id == id2);
            dtos.Should().NotContain(a => a.Id == id3);
        }

        [Fact]
        public async Task GetByCounterpartyIdAsync_ShouldReturnOnlyActiveForSpecificCounterparty()
        {
            // Arrange
            var targetDeps = await SeedDependenciesAsync();
            var otherDeps = await SeedDependenciesAsync();

            var id1 = await SeedBankAccountInDbAsync(null, targetDeps.CounterpartyId);
            var id2 = await SeedBankAccountInDbAsync(null, targetDeps.CounterpartyId);
            var id3 = await SeedBankAccountInDbAsync(null, otherDeps.CounterpartyId);

            await _bankAccountService.DeleteAsync(id2);

            // Act
            var result = await _bankAccountService.GetByCounterpartyIdAsync(targetDeps.CounterpartyId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().ContainSingle();
            dtos.Should().Contain(a => a.Id == id1);
            dtos.Should().NotContain(a => a.Id == id2);
            dtos.Should().NotContain(a => a.Id == id3);
        }
    }
}