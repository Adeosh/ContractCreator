using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractWaybillItemConfiguration : IEntityTypeConfiguration<ContractWaybillItem>
    {
        public void Configure(EntityTypeBuilder<ContractWaybillItem> builder)
        {
            builder.ToTable("ContractWaybillItems", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.NomenclatureName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.UnitOfMeasure)
                   .HasMaxLength(20);

            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.UnitPrice).HasPrecision(18, 2);

            builder.Property(e => e.TotalAmount)
                   .HasComputedColumnSql("\"Quantity\" * \"UnitPrice\"", stored: true)
                   .HasPrecision(18, 2);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Waybill)
                   .WithMany(w => w.Items)
                   .HasForeignKey(e => e.WaybillId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
