using ContractCreator.Domain.Models;
using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContractCreator.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ClassifierBic> ClassifierBics { get; set; }
        public DbSet<ClassifierGar> ClassifierGars { get; set; }
        public DbSet<ClassifierOkopf> ClassifierOkopfs { get; set; }
        public DbSet<ClassifierOkv> ClassifierOkvs { get; set; }
        public DbSet<ClassifierOkved> ClassifierOkveds { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractAct> ContractActs { get; set; }
        public DbSet<ContractActItem> ContractActItems { get; set; }
        public DbSet<ContractInvoice> ContractInvoices { get; set; }
        public DbSet<ContractInvoiceItem> ContractInvoiceItems { get; set; }
        public DbSet<ContractSpecification> ContractSpecifications { get; set; }
        public DbSet<ContractStage> ContractStages { get; set; }
        public DbSet<ContractStageChangeHistory> ChangeHistories { get; set; }
        public DbSet<ContractStep> ContractSteps { get; set; }
        public DbSet<ContractStepItem> ContractStepItems { get; set; }
        public DbSet<Counterparty> Counterparties { get; set; }
        public DbSet<CounterpartyFile> CounterpartyFiles { get; set; }
        public DbSet<FileStorage> Storages { get; set; }
        public DbSet<Firm> Firms { get; set; }
        public DbSet<FirmEconomicActivity> FirmEconomicActivities { get; set; }
        public DbSet<FirmFile> FirmFiles { get; set; }
        public DbSet<GoodsAndService> GoodsAndServices { get; set; }
        public DbSet<Worker> Workers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<EmailAddress>()
                .HaveConversion<EmailAddressConverter>()
                .HaveMaxLength(150);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.GetColumnName(StoreObjectIdentifier.Table(
                        entity.GetTableName()!, 
                        entity.GetSchema())) == null)
                    {
                        property.SetColumnName(property.Name);
                    }
                }
            }
        }
    }
}
