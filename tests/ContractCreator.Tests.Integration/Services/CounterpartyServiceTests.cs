using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class CounterpartyServiceTests : IntegrationTestBase
    {
        private readonly CounterpartyService _counterpartyService;

        public CounterpartyServiceTests() : base()
        {
            _counterpartyService = new CounterpartyService(UowFactory);
        }

        private async Task<int> CreateTestCounterpartyViaServiceAsync(int firmId = 1, string fullName = "ООО 'Новый Контрагент'")
        {
            var counterpartyModel = TestDataFactory.CreateCounterparty(0, firmId);
            var dto = counterpartyModel.Adapt<CounterpartyDto>();

            dto.Id = 0;
            dto.FirmId = firmId;
            dto.FullName = fullName;
            dto.Email = "counterparty@test.ru";

            return await _counterpartyService.CreateCounterpartyAsync(dto);
        }

        [Fact]
        public async Task CreateCounterpartyAsync_ShouldAddCounterpartyToDatabase()
        {
            // Arrange
            var counterpartyModel = TestDataFactory.CreateCounterparty(0, 1);
            var dto = counterpartyModel.Adapt<CounterpartyDto>();
            dto.Email = "test_cp@test.ru";
            dto.Id = 0;

            // Act
            var createdId = await _counterpartyService.CreateCounterpartyAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var cpInDb = await db.Counterparties.FirstOrDefaultAsync(c => c.Id == createdId);

            cpInDb.Should().NotBeNull();
            cpInDb!.FullName.Should().Be(dto.FullName);
            cpInDb.IsDeleted.Should().BeFalse();
            cpInDb.CreatedDate.Should().Be(DateOnly.FromDateTime(DateTime.Now));
        }

        [Fact]
        public async Task GetCounterpartyByIdAsync_ShouldReturnCorrectCounterparty()
        {
            // Arrange
            var targetId = await CreateTestCounterpartyViaServiceAsync(1, "ЗАО 'Искомый Контрагент'");

            // Act
            var result = await _counterpartyService.GetCounterpartyByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.FullName.Should().Be("ЗАО 'Искомый Контрагент'");
        }

        [Fact]
        public async Task UpdateCounterpartyAsync_ShouldModifyExistingCounterparty()
        {
            // Arrange
            var targetId = await CreateTestCounterpartyViaServiceAsync(1, "ООО 'Старое Название'");

            var updateDto = await _counterpartyService.GetCounterpartyByIdAsync(targetId);
            updateDto.Should().NotBeNull();

            updateDto!.FullName = "ООО 'Обновленное Название'";
            updateDto.INN = "9998887776";

            // Act
            await _counterpartyService.UpdateCounterpartyAsync(updateDto);

            // Assert
            var result = await _counterpartyService.GetCounterpartyByIdAsync(targetId);
            result!.FullName.Should().Be("ООО 'Обновленное Название'");
            result.INN.Should().Be("9998887776");
        }

        [Fact]
        public async Task DeleteCounterpartyAsync_ShouldSoftDeleteCounterparty()
        {
            // Arrange
            var targetId = await CreateTestCounterpartyViaServiceAsync();

            // Act
            await _counterpartyService.DeleteCounterpartyAsync(targetId);

            // Assert
            using var db = CreateContext();
            var cpInDb = await db.Counterparties.FirstOrDefaultAsync(c => c.Id == targetId);

            cpInDb.Should().NotBeNull("Потому что используется мягкое удаление (Soft Delete)");
            cpInDb!.IsDeleted.Should().BeTrue();

            var allCounterparties = await _counterpartyService.GetAllCounterpartiesAsync();
            allCounterparties.Should().NotContain(c => c.Id == targetId);
        }

        [Fact]
        public async Task GetAllCounterpartiesAsync_ShouldReturnOnlyNonDeleted()
        {
            // Arrange
            var id1 = await CreateTestCounterpartyViaServiceAsync(1, "Активный 1");
            var id2 = await CreateTestCounterpartyViaServiceAsync(1, "Удаленный 1");
            var id3 = await CreateTestCounterpartyViaServiceAsync(2, "Активный 2");

            await _counterpartyService.DeleteCounterpartyAsync(id2);

            // Act
            var result = await _counterpartyService.GetAllCounterpartiesAsync();

            // Assert
            result.Should().NotBeNull();
            var dtos = result.ToList();

            dtos.Should().Contain(c => c.Id == id1);
            dtos.Should().Contain(c => c.Id == id3);
            dtos.Should().NotContain(c => c.Id == id2, "Удаленные контрагенты не должны возвращаться");
        }

        [Fact]
        public async Task GetCounterpartiesByFirmIdAsync_ShouldReturnCorrectCounterparties()
        {
            // Arrange
            int firmA_Id = 10;
            int firmB_Id = 20;

            var id1 = await CreateTestCounterpartyViaServiceAsync(firmA_Id, "Контрагент Фирмы А - 1");
            var id2 = await CreateTestCounterpartyViaServiceAsync(firmA_Id, "Контрагент Фирмы А - 2");
            var id3 = await CreateTestCounterpartyViaServiceAsync(firmB_Id, "Контрагент Фирмы Б");

            // Act
            var result = await _counterpartyService.GetCounterpartiesByFirmIdAsync(firmA_Id);

            // Assert
            result.Should().NotBeNull();
            var dtos = result.ToList();

            dtos.Should().HaveCount(2);
            dtos.Should().Contain(c => c.Id == id1);
            dtos.Should().Contain(c => c.Id == id2);
            dtos.Should().NotContain(c => c.Id == id3, "Контрагент Фирмы Б не должен попасть в выборку Фирмы А");
        }
    }
}