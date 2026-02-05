using ContractCreator.Domain.Models.Dictionaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations.Dictionaries
{
    public class ClassifierOkvedConfiguration : IEntityTypeConfiguration<ClassifierOkved>
    {
        public void Configure(EntityTypeBuilder<ClassifierOkved> builder)
        {
            builder.ToTable("ClassifierOkved", schema: "ref");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Code).HasMaxLength(20).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(500).IsRequired();

            builder.HasIndex(e => e.Code);
        }
    }
}
