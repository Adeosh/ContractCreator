using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.ToTable("Workers", schema: "public");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(w => w.LastName).HasMaxLength(100).IsRequired();
            builder.Property(w => w.MiddleName).HasMaxLength(100);
            builder.Property(w => w.Position).HasMaxLength(150).IsRequired();
            builder.Property(w => w.INN).HasMaxLength(12).IsRequired();
            builder.Property(w => w.Phone).HasMaxLength(30).IsRequired();
            builder.Property(w => w.Email).HasMaxLength(150).IsRequired(false);

            builder.Property(w => w.IsDirector).HasDefaultValue(false);
            builder.Property(w => w.IsAccountant).HasDefaultValue(false);
            builder.Property(w => w.IsDeleted).HasDefaultValue(false);

            builder.HasOne(f => f.Firm)
                   .WithMany(w => w.Workers)
                   .HasForeignKey(f => f.FirmId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
