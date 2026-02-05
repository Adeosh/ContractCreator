using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractStepItemConfiguration : IEntityTypeConfiguration<ContractStepItem>
    {
        public void Configure(EntityTypeBuilder<ContractStepItem> builder)
        {
            builder.ToTable("ContractStepItems", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.NomenclatureName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.UnitOfMeasure)
                   .HasMaxLength(20);

            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.UnitPrice)
                   .HasPrecision(18, 2);

            builder.Property(e => e.TotalAmount)
                   .HasComputedColumnSql("\"Quantity\" * \"UnitPrice\"", stored: true)
                   .HasPrecision(18, 2);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Step)
                   .WithMany(s => s.Items)
                   .HasForeignKey(e => e.StepId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
