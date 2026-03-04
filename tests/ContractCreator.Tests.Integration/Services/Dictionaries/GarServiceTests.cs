using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Services.Gar;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ContractCreator.Tests.Integration.Services.Dictionaries
{
    public class GarServiceTests : IntegrationTestBase
    {
        private readonly GarService _garService;

        public GarServiceTests() : base()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();

            contextFactoryMock
                .Setup(f => f.CreateDbContextAsync(default))
                .ReturnsAsync(() => CreateContext());

            _garService = new GarService(contextFactoryMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchAddressAsync_ShouldReturnEmpty_WhenQueryIsNullOrWhitespace(string? query)
        {
            // Act
            var result = await _garService.SearchAddressAsync(query!, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("г")]
        [InlineData("а")]
        [InlineData("г а")]
        [InlineData(".-/")]
        public async Task SearchAddressAsync_ShouldReturnEmpty_WhenQueryHasOnlyShortWordsOrDelimiters(string query)
        {
            // Act
            var result = await _garService.SearchAddressAsync(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
