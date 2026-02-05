using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractStageChangeHistoryConfiguration : IEntityTypeConfiguration<ContractStageChangeHistory>
    {
        public void Configure(EntityTypeBuilder<ContractStageChangeHistory> builder)
        {
            builder.ToTable("ChangeHistories", schema: "public");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ContractId);
            builder.Property(e => e.ChangeDate)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();
        }
    }
}
