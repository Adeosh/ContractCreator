using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractInvoiceConfiguration : IEntityTypeConfiguration<ContractInvoice>
    {
        public void Configure(EntityTypeBuilder<ContractInvoice> builder)
        {
            builder.ToTable("ContractInvoices", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.InvoiceNumber).HasMaxLength(50).IsRequired();
            builder.Property(e => e.PurchaserINN).HasMaxLength(12).IsRequired();
            builder.Property(e => e.PurchaserKPP).HasMaxLength(9).IsRequired();

            builder.Property(e => e.InvoiceDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(e => e.TotalAmount).HasPrecision(18, 2);
            builder.Property(e => e.VATAmount).HasPrecision(18, 2);
            builder.Property(e => e.VATRate).HasPrecision(5, 2);
            builder.Property(e => e.AggregateAmount).HasPrecision(18, 2);

            builder.HasOne(e => e.Contract)
                   .WithMany(c => c.Invoices)
                   .HasForeignKey(e => e.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<BankAccount>()
                   .WithMany()
                   .HasForeignKey(e => e.BankAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Items)
                   .WithOne(i => i.Invoice)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
