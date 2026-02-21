using ContractCreator.Application.Mapping;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.Data;
using ContractCreator.Shared.Enums;
using FluentAssertions;
using Mapster;

namespace ContractCreator.Tests.Unit.Mapping
{
    public class MappingTests
    {
        public MappingTests()
        {
            MappingConfig.Configure();
        }

        [Fact]
        public void ShouldMapFirmToFirmDto_WithAllFields()
        {
            // Arrange
            var source = new Firm
            {
                Id = 10,
                LegalFormType = LegalFormType.IndividualPerson,
                FullName = "Полное наименование",
                ShortName = "Краткое",
                Phone = "+79991234567",
                Email = new EmailAddress("info@test.ru"),
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

            source.BankAccounts.Add(new BankAccount
            {
                Id = 1,
                BIC = "888633",
                BankName = "Sber",
                AccountNumber = "01928374655647382910",
                CorrespondentAccount = "01928374655647382910"
            });

            source.EconomicActivities.Add(new FirmEconomicActivity
            {
                EconomicActivityId = 55,
                FirmId = 10,
                IsMain = true
            });

            source.Files.Add(new FirmFile
            {
                FirmId = 10,
                FileId = 1,
                Description = "Contract 01"
            });

            source.Workers.Add(new Worker
            {
                Id = 1,
                FirstName = "Test1",
                LastName = "Test2",
                MiddleName = "Test3",
                Email = new EmailAddress("info@test.ru"),
                Phone = "+7 (922) 44-00-33",
                INN = "1234567890",
                Position = "Директор",
                IsDirector = true,
                IsAccountant = false,
                Note = "testchup",
                FirmId = 1,
                IsDeleted = false
            });

            // Act
            var dest = source.Adapt<FirmDto>();

            // Assert
            dest.Id.Should().Be(source.Id);
            dest.LegalFormType.Should().Be(source.LegalFormType);
            dest.FullName.Should().Be(source.FullName);
            dest.ShortName.Should().Be(source.ShortName);
            dest.Phone.Should().Be(source.Phone);
            dest.Email.Should().Be(source.Email.Value);
            dest.LegalAddress.FullAddress.Should().Be(source.LegalAddress.FullAddress);
            dest.LegalAddress.Building.Should().Be(source.LegalAddress.Building);
            dest.ActualAddress.ObjectId.Should().Be(source.ActualAddress.ObjectId);
            dest.INN.Should().Be(source.INN);
            dest.KPP.Should().Be(source.KPP);
            dest.OGRN.Should().Be(source.OGRN);
            dest.OKTMO.Should().Be(source.OKTMO);
            dest.OKPO.Should().Be(source.OKPO);
            dest.ERNS.Should().Be(source.ERNS);
            dest.ExtraInformation.Should().Be(source.ExtraInformation);
            dest.TaxationType.Should().Be(source.TaxationType);
            dest.IsVATPayment.Should().Be(source.IsVATPayment);
            dest.CreatedDate.Should().Be(source.CreatedDate);
            dest.UpdatedDate.Should().Be(source.UpdatedDate);
            dest.FacsimileSeal.Should().BeEquivalentTo(source.FacsimileSeal);
            dest.FacsimileName.Should().Be(source.FacsimileName);
            dest.OkopfId.Should().Be(source.OkopfId);

            dest.BankAccounts.Should().HaveCount(1);
            dest.BankAccounts[0].BankName.Should().Be("Sber");
            dest.Workers.Should().HaveCount(1);
            dest.Workers[0].FirstName.Should().Be("Test1");
            dest.EconomicActivities.Should().HaveCount(1);
            dest.EconomicActivities[0].IsMain.Should().BeTrue();
            dest.EconomicActivities[0].EconomicActivityId.Should().Be(55);
            dest.Files.Should().HaveCount(1);
            dest.Files[0].Description.Should().Be("Contract 01");
        }

        [Fact]
        public void ShouldMapFirmDtoToFirm_AndCreateValueObjects()
        {
            // Arrange
            var dto = new FirmDto
            {
                FullName = "Из DTO",
                Email = "dto@test.com",
                LegalAddress = new AddressDto
                {
                    FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                    ObjectId = 931166,
                    House = "44",
                    Flat = "100",
                    Building = "3",
                    PostalIndex = "630120",
                }
            };

            // Act
            var firm = dto.Adapt<Firm>();

            // Assert
            firm.FullName.Should().Be(dto.FullName);
            firm.Email.Should().BeOfType<EmailAddress>();
            firm.Email.Value.Should().Be(dto.Email);
            firm.LegalAddress.FullAddress.Should().Be(dto.LegalAddress.FullAddress);
        }
    }
}
