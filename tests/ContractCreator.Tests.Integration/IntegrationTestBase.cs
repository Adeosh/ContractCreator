using ContractCreator.Application.Mapping;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Repositories;
using ContractCreator.Tests.Integration.Data;
using Mapster;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Data.Common;

namespace ContractCreator.Tests.Integration
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly DbContextOptions<AppDbContext> DbOptions;
        protected readonly IUnitOfWorkFactory UowFactory;

        private readonly DbConnection _connection;

        protected IntegrationTestBase()
        {
            MappingConfig.Configure();

            EmailMapping();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = OFF;"; // Игнорируем foreign keys
                command.ExecuteNonQuery();
            }

            DbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using (var context = new AppDbContext(DbOptions))
            {
                context.Database.EnsureCreated();
                TestDataFactory.SeedDictionaries(context);
            }

            var dbContextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();
            dbContextFactoryMock.Setup(f => f.CreateDbContext())
                .Returns(() => {
                    var context = new AppDbContext(DbOptions);
                    return context;
                });

            UowFactory = new UnitOfWorkFactory(dbContextFactoryMock.Object);
        }

        private static void EmailMapping()
        {
            TypeAdapterConfig<string, EmailAddress>.NewConfig()
                .MapWith(s => string.IsNullOrWhiteSpace(s) ? null! : new EmailAddress(s));
            TypeAdapterConfig<EmailAddress, string>.NewConfig()
                .MapWith(e => e == null ? string.Empty : e.Value);
        }

        protected AppDbContext CreateContext() => new AppDbContext(DbOptions);

        public void Dispose()
        {
            using var context = CreateContext();
            context.Database.EnsureDeleted();
            _connection.Close();
            _connection.Dispose();
        }
    }
}