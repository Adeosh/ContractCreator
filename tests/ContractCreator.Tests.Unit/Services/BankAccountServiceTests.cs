using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ContractCreator.Tests.Unit.Services
{
    public class BankAccountServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<BankAccount>> _repoMock;
        private readonly BankAccountService _service;

        public BankAccountServiceTests()
        {
            MappingConfig.Configure();

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            _uowMock = new Mock<IUnitOfWork>();
            _repoMock = new Mock<IRepository<BankAccount>>();
            _uowMock.Setup(x => x.Repository<BankAccount>()).Returns(_repoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new BankAccountService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateForFirm_WhenFirmIdIsProvided()
        {
            // Arrange
            var dto = new BankAccountDto
            {
                FirmId = 10,
                CounterpartyId = null,
                BIC = "044525225",
                BankName = "Sberbank",
                AccountNumber = "40702810..."
            };

            // Act
            var resultId = await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.Is<BankAccount>(b =>
                b.FirmId == 10 &&
                b.CounterpartyId == null &&
                b.BIC == "044525225" &&
                b.IsDeleted == false
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateForCounterparty_WhenCounterpartyIdIsProvided()
        {
            // Arrange
            var dto = new BankAccountDto
            {
                FirmId = null,
                CounterpartyId = 55,
                BIC = "044525974",
                BankName = "Tinkoff",
                AccountNumber = "40802810..."
            };

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(x => x.AddAsync(It.Is<BankAccount>(b =>
                b.FirmId == null &&
                b.CounterpartyId == 55 &&
                b.IsDeleted == false
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenNoOwnerIsProvided()
        {
            // Arrange
            var dto = new BankAccountDto
            {
                FirmId = null,
                CounterpartyId = null,
                BIC = "000000000",
                BankName = "Ghost Bank",
                AccountNumber = "000"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));

            ex.Message.Should().Be("Счет должен быть привязан к Фирме или Контрагенту");

            _repoMock.Verify(x => x.AddAsync(It.IsAny<BankAccount>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetByFirmIdAsync_ShouldReturnOnlyActiveFirmAccounts()
        {
            // Arrange
            var firmId = 10;
            var accounts = new List<BankAccount>
            {
                new BankAccount { Id = 1, FirmId = firmId, CounterpartyId = null, BIC = "1", BankName = "FirmBank", AccountNumber="1", IsDeleted = false },
                new BankAccount { Id = 2, FirmId = 999, CounterpartyId = null, BIC = "2", BankName = "Other", AccountNumber="2", IsDeleted = false },
                new BankAccount { Id = 3, FirmId = firmId, CounterpartyId = null, BIC = "3", BankName = "Deleted", AccountNumber="3", IsDeleted = true },
                new BankAccount { Id = 4, FirmId = null, CounterpartyId = 5, BIC = "4", BankName = "Cptr", AccountNumber="4", IsDeleted = false }
            };

            _repoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
                .ReturnsAsync((Expression<Func<BankAccount, bool>> predicate) =>
                {
                    return accounts.Where(predicate.Compile()).ToList();
                });

            // Act
            var result = await _service.GetByFirmIdAsync(firmId);

            // Assert
            result.Should().HaveCount(1);
            result.First().BankName.Should().Be("FirmBank");
        }

        [Fact]
        public async Task GetByCounterpartyIdAsync_ShouldReturnOnlyActiveCounterpartyAccounts()
        {
            // Arrange
            var cId = 55;
            var accounts = new List<BankAccount>
            {
                new BankAccount { Id = 1, FirmId = null, CounterpartyId = cId, BIC = "1", BankName = "Target", AccountNumber="1", IsDeleted = false },
                new BankAccount { Id = 2, FirmId = 10, CounterpartyId = null, BIC = "2", BankName = "Firm", AccountNumber="2", IsDeleted = false }
            };

            _repoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
                .ReturnsAsync((Expression<Func<BankAccount, bool>> predicate) =>
                {
                    return accounts.Where(predicate.Compile()).ToList();
                });

            // Act
            var result = await _service.GetByCounterpartyIdAsync(cId);

            // Assert
            result.Should().HaveCount(1);
            result.First().BankName.Should().Be("Target");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFields_WhenFound()
        {
            // Arrange
            var id = 7;
            var entity = new BankAccount { Id = id, BIC = "Old", BankName = "Old", AccountNumber = "Old" };
            var updateDto = new BankAccountDto { Id = id, BIC = "New", BankName = "New", AccountNumber = "New" };

            _repoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert
            entity.BankName.Should().Be("New");
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDelete()
        {
            // Arrange
            var id = 8;
            var entity = new BankAccount { Id = id, IsDeleted = false, BIC = "1", BankName = "1", AccountNumber = "1" };
            _repoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            entity.IsDeleted.Should().BeTrue();
            _repoMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        }
    }
}
