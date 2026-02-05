using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractSpecificationConfiguration : IEntityTypeConfiguration<ContractSpecification>
    {
        public void Configure(EntityTypeBuilder<ContractSpecification> builder)
        {
            builder.ToTable("ContractSpecifications", schema: "public");
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

            builder.Property(e => e.VATRate)
                   .HasPrecision(5, 2);

            builder.Property(e => e.TotalAmount)
                   .HasComputedColumnSql("\"Quantity\" * \"UnitPrice\"", stored: true)
                   .HasPrecision(18, 2);

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Contract)
                   .WithMany(c => c.Specifications)
                   .HasForeignKey(e => e.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
