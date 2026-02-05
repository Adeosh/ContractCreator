using ContractCreator.Domain.Models;
using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class FirmConfiguration : IEntityTypeConfiguration<Firm>
    {
        public void Configure(EntityTypeBuilder<Firm> builder)
        {
            builder.ToTable("Firms", schema: "public");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.FullName).HasMaxLength(400).IsRequired();
            builder.Property(f => f.ShortName).HasMaxLength(200).IsRequired();
            builder.Property(f => f.Email).HasMaxLength(150).IsRequired(false);
            builder.Property(f => f.Phone).HasMaxLength(30);
            builder.Property(f => f.INN).HasMaxLength(12).IsRequired();
            builder.Property(f => f.KPP).HasMaxLength(9);
            builder.Property(f => f.OGRN).HasMaxLength(15);
            builder.Property(f => f.OKTMO).HasMaxLength(11);
            builder.Property(f => f.OKPO).HasMaxLength(10);
            builder.Property(f => f.ERNS).HasMaxLength(10);

            builder.Property(f => f.LegalFormType)
               .HasConversion<byte>()
               .IsRequired();

            builder.Property(f => f.TaxationType)
                   .HasConversion<byte>()
                   .IsRequired();

            builder.Property(f => f.LegalAddress)
                   .HasConversion<JsonValueConverter<AddressData>>()
                   .HasColumnType("jsonb")
                   .IsRequired();

            builder.Property(f => f.ActualAddress)
                   .HasConversion<JsonValueConverter<AddressData>>()
                   .HasColumnType("jsonb")
                   .IsRequired();

            builder.Property(f => f.IsVATPayment)
               .HasDefaultValue(false);

            builder.Property(f => f.FacsimileName).HasMaxLength(255);
            builder.Property(f => f.FacsimileSeal)
               .HasColumnType("bytea");

            builder.Property(f => f.CreatedDate)
               .HasColumnType("date")
               .IsRequired();

            builder.Property(f => f.UpdatedDate)
                   .HasColumnType("date");

            builder.HasOne(f => f.Okopf)
                   .WithMany()
                   .HasForeignKey(f => f.OkopfId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Files)
               .WithOne()
               .HasForeignKey(file => file.FirmId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.EconomicActivities)
               .WithOne(ea => ea.Firm)
               .HasForeignKey(ea => ea.FirmId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
