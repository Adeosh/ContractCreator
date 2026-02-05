using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Enums;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Firms;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.Data;
using FluentAssertions;
using Moq;

namespace ContractCreator.Tests.Unit.Services
{
    public class FirmServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<Firm>> _firmRepoMock;
        private readonly FirmService _service;

        public FirmServiceTests()
        {
            MappingConfig.Configure();

            _uowMock = new Mock<IUnitOfWork>();
            _firmRepoMock = new Mock<IRepository<Firm>>();
            _uowMock.Setup(x => x.Repository<Firm>()).Returns(_firmRepoMock.Object);
            _service = new FirmService(_uowMock.Object);
        }

        [Fact]
        public async Task GetFirmByIdAsync_ShouldReturnDto_WhenFirmExists()
        {
            // Arrange
            var firmId = 1;
            var existingFirm = new Firm
            {
                Id = firmId,
                FullName = "Тестовая фирма",
                ShortName = "ТФ",
                Phone = "+7 922 344 99 00",
                Email = new EmailAddress("test@firm.ru"),
                LegalAddress = new AddressData
                {
                    FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                    ObjectId = 753471,
                    House = "10",
                    Flat = "1",
                    Building = "A",
                    PostalIndex = "188540",
                },
                ActualAddress = new AddressData
                {
                    FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                    ObjectId = 931166,
                    House = "44",
                    Flat = "100",
                    Building = "3",
                    PostalIndex = "630120",
                },
                INN = "1234567890",
                KPP = "123456789",
                OGRN = "1027739460737",
                OKTMO = "45300000",
                OKPO = "12345678",
                ERNS = "999",
                ExtraInformation = "Заметки",
                TaxationType = TaxationSystemType.AUSN,
                IsVATPayment = true,
                CreatedDate = new DateOnly(2024, 1, 1),
                UpdatedDate = new DateOnly(2024, 2, 2),
                FacsimileSeal = new byte[] { 0x01, 0x02 },
                FacsimileName = "Sign.png",
                IsDeleted = false,
                OkopfId = 123
            };

            _firmRepoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<FirmByIdWithDetailsSpec>()))
                .ReturnsAsync(existingFirm);

            // Act
            var result = await _service.GetFirmByIdAsync(firmId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(firmId);
            result.FullName.Should().Be("Тестовая фирма");
            result.Email.Should().Be("test@firm.ru");
        }

        [Fact]
        public async Task CreateFirmAsync_ShouldCallSave_AndReturnNewId()
        {
            // Arrange
            var dto = new FirmDto
            {
                Id = 1,
                FullName = "Тестовая фирма",
                ShortName = "ТФ",
                Phone = "+7 922 344 99 00",
                Email = new EmailAddress("test@firm.ru"),
                LegalAddress = new AddressDto
                {
                    FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                    ObjectId = 753471,
                    House = "10",
                    Flat = "1",
                    Building = "A",
                    PostalIndex = "188540",
                },
                ActualAddress = new AddressDto
                {
                    FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                    ObjectId = 931166,
                    House = "44",
                    Flat = "100",
                    Building = "3",
                    PostalIndex = "630120",
                },
                INN = "1234567890",
                KPP = "123456789",
                OGRN = "1027739460737",
                OKTMO = "45300000",
                OKPO = "12345678",
                ERNS = "999",
                ExtraInformation = "Заметки",
                TaxationType = (byte)TaxationSystemType.AUSN,
                IsVATPayment = true,
                CreatedDate = new DateOnly(2024, 1, 1),
                UpdatedDate = new DateOnly(2024, 2, 2),
                FacsimileSeal = new byte[] { 0x01, 0x02 },
                FacsimileName = "Sign.png",
                IsDeleted = false,
                OkopfId = 123
            };

            // Act
            var resultId = await _service.CreateFirmAsync(dto);

            // Assert
            _firmRepoMock.Verify(x => x.AddAsync(It.IsAny<Firm>()), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetFirmByIdAsync_ShouldReturnNull_WhenFirmDoesNotExist()
        {
            // Arrange
            _firmRepoMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<FirmByIdWithDetailsSpec>()))
                .ReturnsAsync((Firm?)null);

            // Act
            var result = await _service.GetFirmByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateFirmAsync_ShouldUpdateEntity_WhenFirmExists()
        {
            // Arrange
            var firmId = 10;

            // Существующая в "базе" фирма
            var existingFirm = new Firm
            {
                Id = firmId,
                FullName = "Тестовая фирма",
                ShortName = "ТФ",
                Phone = "+7 922 344 99 00",
                Email = new EmailAddress("test@firm.ru"),
                LegalAddress = new AddressData
                {
                    FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                    ObjectId = 753471,
                    House = "10",
                    Flat = "1",
                    Building = "A",
                    PostalIndex = "188540",
                },
                ActualAddress = new AddressData
                {
                    FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                    ObjectId = 931166,
                    House = "44",
                    Flat = "100",
                    Building = "3",
                    PostalIndex = "630120",
                },
                INN = "1234567890",
                KPP = "123456789",
                OGRN = "1027739460737",
                OKTMO = "45300000",
                OKPO = "12345678",
                ERNS = "999",
                ExtraInformation = "Заметки",
                TaxationType = TaxationSystemType.AUSN,
                IsVATPayment = true,
                CreatedDate = new DateOnly(2024, 1, 1),
                UpdatedDate = new DateOnly(2024, 2, 2),
                FacsimileSeal = new byte[] { 0x01, 0x02 },
                FacsimileName = "Sign.png",
                IsDeleted = false,
                OkopfId = 123
            };

            // DTO с новыми данными
            var updateDto = new FirmDto
            {
                Id = firmId,
                FullName = "Обновленная фирма",
                ShortName = "ОФ",
                Phone = "+7 922 344 55 00",
                Email = new EmailAddress("update@firm.ru"),
                LegalAddress = new AddressDto
                {
                    FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                    ObjectId = 931166,
                    House = "44",
                    Flat = "100",
                    Building = "3",
                    PostalIndex = "630120",
                },
                ActualAddress = new AddressDto
                {                    
                    FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                    ObjectId = 753471,
                    House = "10",
                    Flat = "1",
                    Building = "A",
                    PostalIndex = "188540",
                },
                INN = "0987654321",
                KPP = "123456789",
                OGRN = "1027739460737",
                OKTMO = "45300000",
                OKPO = "12345678",
                ERNS = "999",
                ExtraInformation = "Заметки для обновления",
                TaxationType = (byte)TaxationSystemType.NPD,
                IsVATPayment = true,
                CreatedDate = new DateOnly(2024, 1, 1),
                UpdatedDate = new DateOnly(2024, 2, 2),
                FacsimileSeal = new byte[] { 0x01, 0x02 },
                FacsimileName = "Sign.png",
                IsDeleted = false,
                OkopfId = 321
            };

            // Настраиваем мок: GetByIdAsync должен вернуть нашу фирму
            _firmRepoMock.Setup(x => x.GetByIdAsync(firmId))
                .ReturnsAsync(existingFirm);

            // Act
            await _service.UpdateFirmAsync(updateDto);

            // Assert
            existingFirm.FullName.Should().Be("Обновленная фирма");
            existingFirm.Email.Value.Should().Be("update@firm.ru");

            _firmRepoMock.Verify(x => x.UpdateAsync(existingFirm), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateFirmAsync_ShouldThrowException_WhenFirmDoesNotExist()
        {
            // Arrange
            var dto = new FirmDto { Id = 999 };

            // Настраиваем мок: возвращаем null (не найдено)
            _firmRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Firm?)null);

            // Act & Assert
            // Ожидаем исключение Exception с текстом "Фирма не найдена"
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.UpdateFirmAsync(dto));
            exception.Message.Should().Be("Фирма не найдена");

            // Убеждаемся, что Update и SaveChanges НЕ вызывались
            _firmRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Firm>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteFirmAsync_ShouldCallDelete_WhenFirmExists()
        {
            // Arrange
            var firmId = 5;
            var firm = new Firm { Id = firmId, FullName = "Удалить", ShortName = "Уд", Phone = "1", INN = "1", Email = new EmailAddress("a@a.ru"), LegalAddress = new AddressData(), ActualAddress = new AddressData() };

            _firmRepoMock.Setup(x => x.GetByIdAsync(firmId)).ReturnsAsync(firm);

            // Act
            await _service.DeleteFirmAsync(firmId);

            // Assert
            _firmRepoMock.Verify(x => x.DeleteAsync(firm), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteFirmAsync_ShouldDoNothing_WhenFirmDoesNotExist()
        {
            // Arrange
            _firmRepoMock.Setup(x => x.GetByIdAsync(777)).ReturnsAsync((Firm?)null);

            // Act
            await _service.DeleteFirmAsync(777);

            // Assert
            // Убеждаемся, что Delete и SaveChanges НЕ вызывались
            _firmRepoMock.Verify(x => x.DeleteAsync(It.IsAny<Firm>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetAllFirmsAsync_ShouldReturnMappedList()
        {
            // Arrange
            var firmsList = new List<Firm>
            {
                new Firm
                {
                    Id = 1,
                    FullName = "Тестовая фирма",
                    ShortName = "ТФ",
                    Phone = "+7 922 344 99 00",
                    Email = new EmailAddress("test@firm.ru"),
                    LegalAddress = new AddressData
                    {
                        FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                        ObjectId = 753471,
                        House = "10",
                        Flat = "1",
                        Building = "A",
                        PostalIndex = "188540",
                    },
                    ActualAddress = new AddressData
                    {
                        FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                        ObjectId = 931166,
                        House = "44",
                        Flat = "100",
                        Building = "3",
                        PostalIndex = "630120",
                    },
                    INN = "1234567890",
                    KPP = "123456789",
                    OGRN = "1027739460737",
                    OKTMO = "45300000",
                    OKPO = "12345678",
                    ERNS = "999",
                    ExtraInformation = "Заметки",
                    TaxationType = TaxationSystemType.AUSN,
                    IsVATPayment = true,
                    CreatedDate = new DateOnly(2024, 1, 1),
                    UpdatedDate = new DateOnly(2024, 2, 2),
                    FacsimileSeal = new byte[] { 0x01, 0x02 },
                    FacsimileName = "Sign.png",
                    IsDeleted = false,
                    OkopfId = 123
                },
                new Firm
                {
                    Id = 2,
                    FullName = "Тестовая фирма 2",
                    ShortName = "ТФ 2",
                    Phone = "+7 922 344 99 00",
                    Email = new EmailAddress("test2@firm.ru"),
                    LegalAddress = new AddressData
                    {
                        FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                        ObjectId = 753471,
                        House = "10",
                        Flat = "1",
                        Building = "A",
                        PostalIndex = "188540",
                    },
                    ActualAddress = new AddressData
                    {
                        FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                        ObjectId = 931166,
                        House = "44",
                        Flat = "100",
                        Building = "3",
                        PostalIndex = "630120",
                    },
                    INN = "1234567890",
                    KPP = "987654321",
                    OGRN = "1027739460737",
                    OKTMO = "45300000",
                    OKPO = "12345678",
                    ERNS = "999",
                    ExtraInformation = "Заметки2",
                    TaxationType = TaxationSystemType.ESHN,
                    IsVATPayment = true,
                    CreatedDate = new DateOnly(2024, 1, 1),
                    UpdatedDate = new DateOnly(2024, 2, 2),
                    FacsimileSeal = new byte[] { 0x01, 0x02 },
                    FacsimileName = "Sign2.png",
                    IsDeleted = false,
                    OkopfId = 321
                }
            };

            _firmRepoMock.Setup(x => x.ListAllAsync()).ReturnsAsync(firmsList);

            // Act
            var result = await _service.GetAllFirmsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().FullName.Should().Be("Тестовая фирма");
            result.Last().OkopfId.Should().Be(321);
        }
    }
}
