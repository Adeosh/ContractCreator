using ContractCreator.Domain.Models.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations.Templates
{
    public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
        {
            builder.ToTable("DocumentTemplates", schema: "ref");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.TemplateName)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(e => e.TemplateType)
                   .HasMaxLength(40)
                   .IsRequired();

            builder.Property(e => e.Content)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(255);

            builder.Property(b => b.IsActive)
                   .HasDefaultValue(true);
        }
    }
}
