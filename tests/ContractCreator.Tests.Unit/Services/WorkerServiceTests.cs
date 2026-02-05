using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.DTOs;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ContractCreator.Tests.Unit.Services
{
    public class WorkerServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<Worker>> _workerRepoMock;
        private readonly WorkerService _service;

        public WorkerServiceTests()
        {
            MappingConfig.Configure();

            _uowMock = new Mock<IUnitOfWork>();
            _workerRepoMock = new Mock<IRepository<Worker>>();
            _uowMock.Setup(x => x.Repository<Worker>()).Returns(_workerRepoMock.Object);
            _service = new WorkerService(_uowMock.Object);
        }

        [Fact]
        public async Task GetWorkersByFirmIdAsync_ShouldReturnFilteredList()
        {
            // Arrange
            var firmId = 10;

            var allWorkers = new List<Worker>
            {
                new Worker { Id = 1, FirmId = firmId, FirstName = "Ivan", LastName = "Ivanov", Position = "Dev", INN = "1", Phone = "1", IsDeleted = false },
                new Worker { Id = 2, FirmId = 999, FirstName = "Petr", LastName = "Petrov", Position = "QA", INN = "2", Phone = "2", IsDeleted = false },
                new Worker { Id = 3, FirmId = firmId, FirstName = "Deleted", LastName = "User", Position = "Dev", INN = "3", Phone = "3", IsDeleted = true }
            };

            _workerRepoMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Worker, bool>>>())) // Настраиваем FindAsync. 
                .ReturnsAsync((Expression<Func<Worker, bool>> predicate) =>
                {
                    return allWorkers.Where(predicate.Compile()).ToList();
                });

            // Act
            var result = await _service.GetWorkersByFirmIdAsync(firmId);

            // Assert
            result.Should().HaveCount(1);
            result.First().FirstName.Should().Be("Ivan");
            result.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetWorkerByIdAsync_ShouldReturnDto_WhenExists()
        {
            // Arrange
            var worker = new Worker
            {
                Id = 5,
                FirstName = "Test",
                LastName = "User",
                Position = "CEO",
                INN = "123",
                Phone = "555",
                Email = new EmailAddress("test@mail.ru")
            };

            _workerRepoMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(worker);

            // Act
            var result = await _service.GetWorkerByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(5);
            result.Email.Should().Be("test@mail.ru");
        }

        [Fact]
        public async Task CreateWorkerAsync_ShouldAddEntityAndReturnId()
        {
            // Arrange
            var dto = new WorkerDto
            {
                FirstName = "New",
                LastName = "Worker",
                Position = "Manager",
                INN = "111",
                Phone = "222",
                Email = "new@mail.ru",
                FirmId = 10
            };

            // Act
            var resultId = await _service.CreateWorkerAsync(dto);

            // Assert
            _workerRepoMock.Verify(x => x.AddAsync(It.Is<Worker>(w =>
                w.FirstName == "New" &&
                w.Email!.Value == "new@mail.ru" &&
                w.IsDeleted == false
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkerAsync_ShouldUpdateFields_WhenWorkerExists()
        {
            // Arrange
            var workerId = 7;
            var existingWorker = new Worker
            {
                Id = workerId,
                FirstName = "OldName",
                LastName = "OldLast",
                Position = "OldPos",
                INN = "000",
                Phone = "000",
                Email = new EmailAddress("old@mail.ru")
            };

            var updateDto = new WorkerDto
            {
                Id = workerId,
                FirstName = "NewName",
                LastName = "NewLast",
                Position = "NewPos",
                INN = "111",
                Phone = "111",
                Email = "new@mail.ru"
            };

            _workerRepoMock.Setup(x => x.GetByIdAsync(workerId)).ReturnsAsync(existingWorker);

            // Act
            await _service.UpdateWorkerAsync(updateDto);

            // Assert
            existingWorker.FirstName.Should().Be("NewName");
            existingWorker.Email!.Value.Should().Be("new@mail.ru");

            _workerRepoMock.Verify(x => x.UpdateAsync(existingWorker), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkerAsync_ShouldThrow_WhenWorkerNotFound()
        {
            // Arrange
            _workerRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Worker?)null);
            var dto = new WorkerDto { Id = 999 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateWorkerAsync(dto));
            ex.Message.Should().Be("Сотрудник не найден");

            _workerRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Worker>()), Times.Never);
        }

        [Fact]
        public async Task DeleteWorkerAsync_ShouldSetIsDeletedFlag()
        {
            // Arrange
            var workerId = 10;
            var worker = new Worker
            {
                Id = workerId,
                IsDeleted = false,
                FirstName = "DeleteMe",
                LastName = "Now",
                Position = "None",
                INN = "0",
                Phone = "0"
            };

            _workerRepoMock.Setup(x => x.GetByIdAsync(workerId)).ReturnsAsync(worker);

            // Act
            await _service.DeleteWorkerAsync(workerId);

            // Assert
            worker.IsDeleted.Should().BeTrue();

            _workerRepoMock.Verify(x => x.UpdateAsync(worker), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
