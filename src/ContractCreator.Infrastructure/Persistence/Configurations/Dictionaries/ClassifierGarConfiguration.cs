using ContractCreator.Domain.Models.Dictionaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations.Dictionaries
{
    public class ClassifierGarConfiguration : IEntityTypeConfiguration<ClassifierGar>
    {
        public void Configure(EntityTypeBuilder<ClassifierGar> builder)
        {
            builder.ToTable("ClassifierGar", schema: "ref");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.ObjectId)
                   .IsRequired();

            builder.Property(e => e.ObjectGuid);

            builder.Property(e => e.RegionCode)
                   .IsRequired();

            builder.Property(e => e.Name)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.TypeName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.Level)
                   .IsRequired();

            builder.Property(e => e.FullAddress)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(e => e.LiveInTown)
                   .IsRequired();

            builder.Property(e => e.PostalIndex)
                   .HasMaxLength(6);

            builder.HasIndex(e => e.ObjectId)
                   .HasDatabaseName("addresses_objectid_idx");

            builder.HasIndex(e => e.Name)
                   .HasDatabaseName("addresses_name_idx");

            builder.HasIndex(e => e.FullAddress)
                   .HasDatabaseName("addresses_fulladdress_idx");
        }
    }
}
