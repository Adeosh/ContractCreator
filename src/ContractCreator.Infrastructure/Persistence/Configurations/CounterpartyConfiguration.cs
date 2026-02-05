using ContractCreator.Domain.Models;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class CounterpartyConfiguration : IEntityTypeConfiguration<Counterparty>
    {
        public void Configure(EntityTypeBuilder<Counterparty> builder)
        {
            builder.ToTable("Counterparties", schema: "public");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FullName).HasMaxLength(400).IsRequired();
            builder.Property(c => c.ShortName).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(150).IsRequired(false);
            builder.Property(c => c.Phone).HasMaxLength(30);
            builder.Property(c => c.INN).HasMaxLength(12).IsRequired();
            builder.Property(c => c.KPP).HasMaxLength(9);
            builder.Property(c => c.OGRN).HasMaxLength(15);
            builder.Property(c => c.OKTMO).HasMaxLength(11);
            builder.Property(c => c.OKPO).HasMaxLength(10);

            builder.Property(c => c.LegalAddress)
               .HasConversion<JsonValueConverter<AddressData>>()
               .HasColumnType("jsonb")
               .IsRequired();

            builder.Property(c => c.ActualAddress)
                   .HasConversion<JsonValueConverter<AddressData>>()
                   .HasColumnType("jsonb")
                   .IsRequired();

            builder.Property(c => c.LegalForm)
                   .HasConversion<byte>()
                   .IsRequired();

            builder.HasOne(c => c.Firm)
                   .WithMany()
                   .HasForeignKey(c => c.FirmId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Director)
               .WithMany()
               .HasForeignKey(c => c.DirectorId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Accountant)
                   .WithMany()
                   .HasForeignKey(c => c.AccountantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CreatedDate)
               .HasColumnType("date")
               .IsRequired();

            builder.Property(c => c.UpdatedDate)
                   .HasColumnType("date");

            builder.HasMany(c => c.Files)
               .WithOne()
               .HasForeignKey(file => file.CounterpartyId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
