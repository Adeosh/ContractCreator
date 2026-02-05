using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class CounterpartyFileConfiguration : IEntityTypeConfiguration<CounterpartyFile>
    {
        public void Configure(EntityTypeBuilder<CounterpartyFile> builder)
        {
            builder.ToTable("CounterpartyFiles", schema: "public");

            builder.HasKey(x => new { x.CounterpartyId, x.FileId });

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Counterparty)
                   .WithMany(c => c.Files)
                   .HasForeignKey(x => x.CounterpartyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.File)
                   .WithMany()
                   .HasForeignKey(x => x.FileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
