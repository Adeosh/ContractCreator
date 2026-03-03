using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class FirmServiceTests : IntegrationTestBase
    {
        private readonly FirmService _firmService;

        public FirmServiceTests() : base()
        {
            _firmService = new FirmService(UowFactory);
        }

        private async Task<int> CreateTestFirmViaServiceAsync(string fullName = "ООО 'Уникальная Фирма'")
        {
            var firmModel = TestDataFactory.CreateFirm(0);
            var dto = firmModel.Adapt<FirmDto>();
            dto.FullName = fullName;

            return await _firmService.CreateFirmAsync(dto);
        }

        [Fact]
        public async Task CreateFirmAsync_ShouldAddFirmToDatabase()
        {
            // Arrange
            var firmModel = TestDataFactory.CreateFirm(0);
            var dto = firmModel.Adapt<FirmDto>();
            dto.Email = "firm@test.ru";

            // Act
            var createdId = await _firmService.CreateFirmAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var firmInDb = await db.Firms.FirstOrDefaultAsync(f => f.Id == createdId);

            firmInDb.Should().NotBeNull();
            firmInDb!.FullName.Should().Be(dto.FullName);
            firmInDb.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetFirmByIdAsync_ShouldReturnCorrectFirm()
        {
            // Arrange
            var targetId = await CreateTestFirmViaServiceAsync("ООО 'Искомая Фирма'");

            // Act
            var result = await _firmService.GetFirmByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.FullName.Should().Be("ООО 'Искомая Фирма'");
        }

        [Fact]
        public async Task UpdateFirmAsync_ShouldModifyExistingFirm()
        {
            // Arrange
            var targetId = await CreateTestFirmViaServiceAsync("ООО 'Старое Название'");
            var updateDto = await _firmService.GetFirmByIdAsync(targetId);
            updateDto.Should().NotBeNull();
            updateDto!.FullName = "ООО 'Новое Название'";

            // Act
            await _firmService.UpdateFirmAsync(updateDto);

            // Assert
            var result = await _firmService.GetFirmByIdAsync(targetId);
            result!.FullName.Should().Be("ООО 'Новое Название'");
        }

        [Fact]
        public async Task DeleteFirmAsync_ShouldRemoveFirm()
        {
            // Arrange
            var targetId = await CreateTestFirmViaServiceAsync();
            var checkExists = await _firmService.GetFirmByIdAsync(targetId);
            checkExists.Should().NotBeNull();

            // Act
            await _firmService.DeleteFirmAsync(targetId);

            // Assert
            var resultAfterDelete = await _firmService.GetFirmByIdAsync(targetId);
            resultAfterDelete.Should().BeNull();
        }

        [Fact]
        public async Task GetAllFirmsAsync_ShouldReturnAllFirms()
        {
            // Arrange
            var id1 = await CreateTestFirmViaServiceAsync("Фирма 1");
            var id2 = await CreateTestFirmViaServiceAsync("Фирма 2");

            // Act
            var result = await _firmService.GetAllFirmsAsync();

            // Assert
            result.Should().NotBeNull();
            var dtos = result.ToList();

            dtos.Should().Contain(f => f.Id == id1);
            dtos.Should().Contain(f => f.Id == id2);
        }
    }
}