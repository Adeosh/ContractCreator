using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractStepConfiguration : IEntityTypeConfiguration<ContractStep>
    {
        public void Configure(EntityTypeBuilder<ContractStep> builder)
        {
            builder.ToTable("ContractSteps", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.StepName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.TotalAmount)
                   .HasPrecision(18, 2);

            builder.Property(e => e.StartStepDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(e => e.EndStepDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.HasOne(e => e.Currency)
                   .WithMany()
                   .HasForeignKey(e => e.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Contract)
                   .WithMany(c => c.Steps)
                   .HasForeignKey(e => e.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Items)
                   .WithOne(i => i.Step)
                   .HasForeignKey(i => i.StepId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
