using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class WorkerServiceTests : IntegrationTestBase
    {
        private readonly WorkerService _workerService;

        public WorkerServiceTests() : base()
        {
            _workerService = new WorkerService(UowFactory);
        }

        private async Task<int> CreateTestWorkerViaServiceAsync(int firmId = 1, string lastName = "Тестов")
        {
            var workerModel = TestDataFactory.CreateWorker(0, firmId);
            var dto = workerModel.Adapt<WorkerDto>();

            dto.Id = 0;
            dto.FirmId = firmId;
            dto.LastName = lastName;
            dto.Email = "worker@test.ru";

            return await _workerService.CreateWorkerAsync(dto);
        }

        [Fact]
        public async Task CreateWorkerAsync_ShouldAddWorkerToDatabase()
        {
            // Arrange
            var workerModel = TestDataFactory.CreateWorker(0, 5);
            var dto = workerModel.Adapt<WorkerDto>();
            dto.Id = 0;
            dto.Email = "new_worker@test.ru";

            // Act
            var createdId = await _workerService.CreateWorkerAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var workerInDb = await db.Workers.FirstOrDefaultAsync(w => w.Id == createdId);

            workerInDb.Should().NotBeNull();
            workerInDb!.LastName.Should().Be(dto.LastName);
            workerInDb.FirmId.Should().Be(5);
            workerInDb.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetWorkerByIdAsync_ShouldReturnCorrectWorker()
        {
            // Arrange
            var targetId = await CreateTestWorkerViaServiceAsync(1, "Смирнов");

            // Act
            var result = await _workerService.GetWorkerByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.LastName.Should().Be("Смирнов");
        }

        [Fact]
        public async Task UpdateWorkerAsync_ShouldModifyExistingWorker()
        {
            // Arrange
            var targetId = await CreateTestWorkerViaServiceAsync(1, "СтараяФамилия");
            var updateDto = await _workerService.GetWorkerByIdAsync(targetId);

            updateDto!.LastName = "НоваяФамилия";
            updateDto.Position = "Главный Инженер";

            // Act
            await _workerService.UpdateWorkerAsync(updateDto);

            // Assert
            var result = await _workerService.GetWorkerByIdAsync(targetId);
            result!.LastName.Should().Be("НоваяФамилия");
            result.Position.Should().Be("Главный Инженер");
        }

        [Fact]
        public async Task DeleteWorkerAsync_ShouldSoftDeleteWorker()
        {
            // Arrange
            var targetId = await CreateTestWorkerViaServiceAsync(1, "НаУдаление");

            // Act
            await _workerService.DeleteWorkerAsync(targetId);

            // Assert
            using var db = CreateContext();
            var workerInDb = await db.Workers.FirstOrDefaultAsync(w => w.Id == targetId);
            workerInDb.Should().NotBeNull();
            workerInDb!.IsDeleted.Should().BeTrue();

            var firmWorkers = await _workerService.GetWorkersByFirmIdAsync(1);
            firmWorkers.Should().NotContain(w => w.Id == targetId);
        }

        [Fact]
        public async Task GetWorkersByFirmIdAsync_ShouldReturnOnlyActiveWorkersForSpecificFirm()
        {
            // Arrange
            int targetFirmId = 10;
            int otherFirmId = 20;

            var id1 = await CreateTestWorkerViaServiceAsync(targetFirmId, "Сотрудник Фирмы 10 - А");
            var id2 = await CreateTestWorkerViaServiceAsync(targetFirmId, "Сотрудник Фирмы 10 - Б");
            var id3 = await CreateTestWorkerViaServiceAsync(otherFirmId, "Сотрудник Фирмы 20");

            await _workerService.DeleteWorkerAsync(id2);

            // Act
            var result = await _workerService.GetWorkersByFirmIdAsync(targetFirmId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().ContainSingle();
            dtos.Should().Contain(w => w.Id == id1);
            dtos.Should().NotContain(w => w.Id == id2);
            dtos.Should().NotContain(w => w.Id == id3);
        }
    }
}