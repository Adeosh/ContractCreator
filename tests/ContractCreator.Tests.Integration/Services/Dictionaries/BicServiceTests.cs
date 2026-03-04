using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Services.Bic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ContractCreator.Tests.Integration.Services.Dictionaries
{
    public class BicServiceTests : IntegrationTestBase
    {
        private readonly BicService _bicService;

        public BicServiceTests() : base()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();

            contextFactoryMock
                .Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(() => CreateContext());

            _bicService = new BicService(contextFactoryMock.Object);
        }

        private async Task SeedBicsAsync()
        {
            using var db = CreateContext();

            db.ClassifierBics.AddRange(
                new ClassifierBic { BIC = "044525225", Name = "сбербанк россии", Account = "30101810400000000225", SettlementType = "г.", SettlementName = "Москва", Address = "ул. Вавилова, 19" },
                new ClassifierBic { BIC = "044525593", Name = "альфа-банк", Account = "30101810200000000593", SettlementType = "г.", SettlementName = "Москва", Address = "ул. Каланчевская, 27" },
                new ClassifierBic { BIC = "044030653", Name = "банк санкт-петербург", Account = "30101810900000000653", SettlementType = "г.", SettlementName = "Санкт-Петербург", Address = "Малоохтинский пр., 64" },
                new ClassifierBic { BIC = "040000001", Name = "пустой банк", Account = null, SettlementType = null, SettlementName = null, Address = null }
            );

            await db.SaveChangesAsync();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchAsync_ShouldReturnEmpty_WhenQueryIsNullOrWhitespace(string? query)
        {
            // Arrange
            await SeedBicsAsync();

            // Act
            var result = await _bicService.SearchAsync(query!);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchAsync_ShouldSearchByBicPartial()
        {
            // Arrange
            await SeedBicsAsync();

            // Act
            var result = await _bicService.SearchAsync("0445");

            // Assert
            var dtos = result.ToList();
            dtos.Should().HaveCount(2);
            dtos.Should().Contain(b => b.Bic == "044525225");
            dtos.Should().Contain(b => b.Bic == "044525593");
        }

        [Fact]
        public async Task SearchAsync_ShouldSearchByNameCaseInsensitive()
        {
            // Arrange
            await SeedBicsAsync();

            var result = await _bicService.SearchAsync("АЛЬФА");

            // Assert
            var dtos = result.ToList();
            dtos.Should().ContainSingle();
            dtos.First().Name.Should().Be("альфа-банк");
        }

        [Fact]
        public async Task SearchAsync_ShouldMapAddressAndAccountCorrectly()
        {
            // Arrange
            await SeedBicsAsync();

            // Act
            var result = await _bicService.SearchAsync("044525225");

            // Assert
            var bank = result.First();
            bank.CorrespondentAccount.Should().Be("30101810400000000225");
            bank.Address.Should().Be("г., Москва, ул. Вавилова, 19");
        }

        [Fact]
        public async Task SearchAsync_ShouldHandleNullFieldsGracefully()
        {
            // Arrange
            await SeedBicsAsync();

            // Act
            var result = await _bicService.SearchAsync("ПУСТОЙ");

            // Assert
            var bank = result.First();
            bank.CorrespondentAccount.Should().BeEmpty();
            bank.Address.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMax20Results()
        {
            // Arrange
            using var db = CreateContext();
            var bics = Enumerable.Range(100, 25).Select(i => new ClassifierBic
            {
                BIC = $"044000{i}",
                Name = $"тестовый банк {i}"
            });

            db.ClassifierBics.AddRange(bics);
            await db.SaveChangesAsync();

            // Act
            var result = await _bicService.SearchAsync("ТЕСТОВЫЙ");

            // Assert
            result.Should().HaveCount(20);
        }
    }
}
