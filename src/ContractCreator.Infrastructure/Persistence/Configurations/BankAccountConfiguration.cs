using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.ToTable("BankAccounts", schema: "public", t =>
            {
                t.HasCheckConstraint("CK_BankAccount_Owner",
                    "(\"FirmId\" IS NOT NULL AND \"CounterpartyId\" IS NULL) OR (\"FirmId\" IS NULL AND \"CounterpartyId\" IS NOT NULL)");
            });

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BIC)
                   .HasMaxLength(15)
                   .IsRequired();

            builder.Property(b => b.BankName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(b => b.AccountNumber)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(b => b.CorrespondentAccount)
                   .HasMaxLength(20);

            builder.Property(b => b.BankAddress)
                   .HasMaxLength(500);

            builder.Property(b => b.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasOne(b => b.Firm)
                   .WithMany(f => f.BankAccounts)
                   .HasForeignKey(b => b.FirmId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Counterparty)
                   .WithMany(c => c.BankAccounts)
                   .HasForeignKey(b => b.CounterpartyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
