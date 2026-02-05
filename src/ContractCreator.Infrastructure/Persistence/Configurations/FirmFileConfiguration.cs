using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class FirmFileConfiguration : IEntityTypeConfiguration<FirmFile>
    {
        public void Configure(EntityTypeBuilder<FirmFile> builder)
        {
            builder.ToTable("FirmFiles", schema: "public");

            builder.HasKey(x => new { x.FirmId, x.FileId });

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Firm)
               .WithMany(f => f.Files)
               .HasForeignKey(x => x.FirmId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.File)
                   .WithMany()
                   .HasForeignKey(x => x.FileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
