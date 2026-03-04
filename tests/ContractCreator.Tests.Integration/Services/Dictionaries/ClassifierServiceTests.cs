using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Services.Classifiers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ContractCreator.Tests.Integration.Services.Dictionaries
{
    public class ClassifierServiceTests : IntegrationTestBase
    {
        private readonly ClassifierService _classifierService;

        public ClassifierServiceTests() : base()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();

            contextFactoryMock
                .Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(() => CreateContext());

            _classifierService = new ClassifierService(contextFactoryMock.Object);
        }

        private async Task SeedDataAsync()
        {
            using var db = CreateContext();

            db.ClassifierOkopfs.RemoveRange(db.ClassifierOkopfs);
            db.ClassifierOkvs.RemoveRange(db.ClassifierOkvs);
            db.ClassifierOkveds.RemoveRange(db.ClassifierOkveds);
            await db.SaveChangesAsync();

            db.ClassifierOkopfs.AddRange(
                new ClassifierOkopf { Id = 9, Code = "12300", Name = "Общества с ограниченной ответственностью" },
                new ClassifierOkopf { Id = 6, Code = "12200", Name = "Акционерные общества" },
                new ClassifierOkopf { Id = 79, Code = "50102", Name = "Индивидуальные предприниматели" }
            );

            db.ClassifierOkvs.AddRange(
                new ClassifierOkv { Id = 119, NumericCode = 840, LetterCode = "USD", CurrencyName = "Доллар США", CountriesCurrencyUsed = "США" },
                new ClassifierOkv { Id = 157, NumericCode = 978, LetterCode = "EUR", CurrencyName = "Евро", CountriesCurrencyUsed = "Европа" },
                new ClassifierOkv { Id = 95, NumericCode = 643, LetterCode = "RUB", CurrencyName = "Российский рубль", CountriesCurrencyUsed = "Россия" }
            );

            db.ClassifierOkveds.AddRange(
                new ClassifierOkved { Id = 4, Code = "01.11.1", Name = "выращивание зерновых культур" },
                new ClassifierOkved { Id = 5, Code = "01.11.11", Name = "выращивание пшеницы" },
                new ClassifierOkved { Id = 988, Code = "26.20.1", Name = "производство компьютеров" }
            );

            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task GetOkopfsAsync_ShouldReturnAllMappedAndOrderedByCode()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _classifierService.GetOkopfsAsync();

            // Assert
            result.Should().HaveCount(3);
            result.First().Code.Should().Be("12200");
            result.First().Name.Should().Be("Акционерные общества");
        }

        [Fact]
        public async Task GetCurrenciesAsync_ShouldReturnAllMappedAndOrderedByName()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _classifierService.GetCurrenciesAsync();

            // Assert
            result.Should().HaveCount(3);

            var usd = result.Single(c => c.Code == "USD");
            usd.Name.Should().Be("Доллар США");

            result.First().Name.Should().Be("Доллар США");
            result.Last().Name.Should().Be("Российский рубль");
        }

        [Fact]
        public async Task GetAllOkvedsAsync_ShouldReturnAllItems()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _classifierService.GetAllOkvedsAsync();

            // Assert
            result.Should().Contain(x => x.Code == "01.11.1");
            result.Should().Contain(x => x.Code == "26.20.1");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchOkvedsAsync_ShouldReturnEmpty_WhenQueryIsNullOrWhitespace(string? query)
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _classifierService.SearchOkvedsAsync(query!);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchOkvedsAsync_ShouldSearchByCodeStartsWith()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _classifierService.SearchOkvedsAsync("01.11");

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Code == "01.11.1");
            result.Should().Contain(x => x.Code == "01.11.11");
        }

        [Fact]
        public async Task SearchOkvedsAsync_ShouldSearchByNameCaseInsensitive()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var result = await _classifierService.SearchOkvedsAsync("ПШЕНИЦЫ");

            // Assert
            result.Should().ContainSingle();
            result.First().Code.Should().Be("01.11.11");
            result.First().Name.Should().Be("выращивание пшеницы");
        }

        [Fact]
        public async Task SearchOkvedsAsync_ShouldLimitTo20Results()
        {
            // Arrange
            using var db = CreateContext();
            var okveds = Enumerable.Range(10000, 25).Select(i => new ClassifierOkved
            {
                Id = i,
                Code = $"99.{i}",
                Name = $"тестовый оквэд {i}"
            });
            db.ClassifierOkveds.AddRange(okveds);
            await db.SaveChangesAsync();

            // Act
            var result = await _classifierService.SearchOkvedsAsync("ТЕСТОВЫЙ");

            // Assert
            result.Should().HaveCount(20);
        }
    }
}
