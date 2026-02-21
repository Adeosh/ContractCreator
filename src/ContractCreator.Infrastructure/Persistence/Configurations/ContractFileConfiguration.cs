using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    internal class ContractFileConfiguration : IEntityTypeConfiguration<ContractFile>
    {
        public void Configure(EntityTypeBuilder<ContractFile> builder)
        {
            builder.ToTable("ContractFiles", schema: "public");

            builder.HasKey(x => new { x.ContractId, x.FileId });

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Contract)
                   .WithMany(f => f.Files)
                   .HasForeignKey(x => x.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.File)
                   .WithMany()
                   .HasForeignKey(x => x.FileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
