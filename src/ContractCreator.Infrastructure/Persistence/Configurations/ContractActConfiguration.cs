using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractActConfiguration : IEntityTypeConfiguration<ContractAct>
    {
        public void Configure(EntityTypeBuilder<ContractAct> builder)
        {
            builder.ToTable("ContractActs", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ActNumber).HasMaxLength(50).IsRequired();

            builder.Property(e => e.ActDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(e => e.TotalAmount).HasPrecision(18, 2);
            builder.Property(e => e.VATAmount).HasPrecision(18, 2);
            builder.Property(e => e.VATRate).HasPrecision(5, 2);
            builder.Property(e => e.AggregateAmount).HasPrecision(18, 2);

            builder.HasOne(e => e.Contract)
                   .WithMany(c => c.Acts)
                   .HasForeignKey(e => e.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ContractInvoice>()
                   .WithMany()
                   .HasForeignKey(e => e.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Items)
                   .WithOne(i => i.Act)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
