using ContractCreator.Domain.Models.Dictionaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations.Dictionaries
{
    public class ClassifierOkopfConfiguration : IEntityTypeConfiguration<ClassifierOkopf>
    {
        public void Configure(EntityTypeBuilder<ClassifierOkopf> builder)
        {
            builder.ToTable("ClassifierOkopf", schema: "ref");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Code).HasMaxLength(10).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(255).IsRequired();

            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
