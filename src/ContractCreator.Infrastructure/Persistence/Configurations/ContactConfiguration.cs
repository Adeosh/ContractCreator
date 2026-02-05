using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts", schema: "public");
            builder.HasKey(e => e.Id);

            builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
            builder.Property(c => c.MiddleName).HasMaxLength(100);
            builder.Property(c => c.Position).HasMaxLength(150).IsRequired();
            builder.Property(c => c.Phone).HasMaxLength(30).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(150).IsRequired(false);

            builder.Property(c => c.IsDirector).HasDefaultValue(false);
            builder.Property(c => c.IsAccountant).HasDefaultValue(false);
            builder.Property(c => c.IsDeleted).HasDefaultValue(false);

            builder.HasOne(c => c.Counterparty)
                   .WithMany(cp => cp.Contacts)
                   .HasForeignKey(c => c.CounterpartyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
