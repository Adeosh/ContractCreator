using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractWaybillConfiguration : IEntityTypeConfiguration<ContractWaybill>
    {
        public void Configure(EntityTypeBuilder<ContractWaybill> builder)
        {
            builder.ToTable("ContractWaybills", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.WaybillNumber).HasMaxLength(50).IsRequired();
            builder.Property(e => e.WaybillDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(e => e.TotalAmount).HasPrecision(18, 2);
            builder.Property(e => e.VATAmount).HasPrecision(18, 2);
            builder.Property(e => e.VATRate).HasPrecision(5, 2);
            builder.Property(e => e.AggregateAmount).HasPrecision(18, 2);

            builder.HasOne(e => e.Invoice)
                   .WithMany()
                   .HasForeignKey(e => e.InvoiceId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Contract)
                   .WithMany(c => c.Waybills)
                   .HasForeignKey(e => e.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
