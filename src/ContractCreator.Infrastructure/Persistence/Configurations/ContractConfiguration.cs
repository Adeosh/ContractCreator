using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts", schema: "public");
            builder.HasKey(e => e.Id);
            builder.Property(t => t.Type).HasConversion<byte>();
            builder.Property(er => er.EnterpriseRole).HasConversion<byte>();
            builder.Property(i => i.Initiator).HasConversion<byte>();

            builder.Property(c => c.ContractNumber).HasMaxLength(50).IsRequired();

            builder.Property(c => c.ContractPrice)
                   .HasPrecision(18, 2);

            builder.Property(c => c.ContractSubject).HasMaxLength(1000);
            builder.Property(c => c.SubmissionCode).HasMaxLength(50);
            builder.Property(c => c.SubmissionLink).HasMaxLength(500);
            builder.Property(c => c.TerminationReason).HasMaxLength(1000);

            builder.Property(c => c.SubmissionDate).HasColumnType("date");
            builder.Property(c => c.TenderDate).HasColumnType("date");
            builder.Property(c => c.IssueDate).HasColumnType("date");
            builder.Property(c => c.StartDate).HasColumnType("date");
            builder.Property(c => c.EndDate).HasColumnType("date");
            builder.Property(c => c.ExecutionDate).HasColumnType("date");

            builder.HasOne(c => c.Firm)
                   .WithMany(f => f.Contracts)
                   .HasForeignKey(c => c.FirmId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Counterparty)
                   .WithMany(cp => cp.Contracts)
                   .HasForeignKey(c => c.CounterpartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.FirmSigner)
                   .WithMany()
                   .HasForeignKey(c => c.FirmSignerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CounterpartySigner)
                   .WithMany()
                   .HasForeignKey(c => c.CounterpartySignerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.StageType)
                   .WithMany()
                   .HasForeignKey(c => c.StageTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Currency)
                   .WithMany()
                   .HasForeignKey(c => c.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Specifications)
                   .WithOne(s => s.Contract)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Invoices)
                   .WithOne(i => i.Contract)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Acts)
                   .WithOne(c => c.Contract)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Waybills)
                   .WithOne(c => c.Contract)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Steps)
                   .WithOne(c => c.Contract)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
