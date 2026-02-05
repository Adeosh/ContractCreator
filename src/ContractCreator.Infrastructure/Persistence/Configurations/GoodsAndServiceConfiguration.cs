using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class GoodsAndServiceConfiguration : IEntityTypeConfiguration<GoodsAndService>
    {
        public void Configure(EntityTypeBuilder<GoodsAndService> builder)
        {
            builder.ToTable("GoodsAndServices", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(x => x.Type)
                   .HasConversion<byte>();

            builder.Property(e => e.Name)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(1000);

            builder.Property(e => e.CreatedDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(e => e.UnitOfMeasure)
                   .HasMaxLength(20);

            builder.Property(e => e.Price)
                   .HasPrecision(18, 2);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasIndex(x => x.Type);
        }
    }
}
