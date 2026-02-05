using ContractCreator.Domain.Models.Dictionaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations.Dictionaries
{
    public class ClassifierBicConfiguration : IEntityTypeConfiguration<ClassifierBic>
    {
        public void Configure(EntityTypeBuilder<ClassifierBic> builder)
        {
            builder.ToTable("ClassifierBic", schema: "ref");

            builder.HasKey(e => e.BIC);
            builder.Property(e => e.BIC).HasMaxLength(9).IsFixedLength();

            builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
            builder.Property(e => e.EnglishName).HasMaxLength(255);
            builder.Property(e => e.Address).HasMaxLength(500);
            builder.Property(e => e.DateIn).HasColumnType("date");

            builder.HasIndex(e => e.Name);
        }
    }
}
