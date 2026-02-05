using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class FileStorageConfiguration : IEntityTypeConfiguration<FileStorage>
    {
        public void Configure(EntityTypeBuilder<FileStorage> builder)
        {
            builder.ToTable("Storages", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.StorageFileGuid)
                   .IsRequired();

            builder.Property(e => e.Type)
                   .HasConversion<byte>()
                   .IsRequired();

            builder.Property(e => e.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.UploadDate)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(e => e.ChangeDate)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.IsEncrypted)
                   .HasDefaultValue(false);
        }
    }
}
