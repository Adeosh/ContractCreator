using ContractCreator.Domain.Models;
using ContractCreator.Domain.Models.Dictionaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class FirmEconomicActivityConfiguration : IEntityTypeConfiguration<FirmEconomicActivity>
    {
        public void Configure(EntityTypeBuilder<FirmEconomicActivity> builder)
        {
            builder.ToTable("FirmEconomicActivities", schema: "public");

            builder.HasKey(x => new { x.FirmId, x.EconomicActivityId });

            builder
                .HasOne(x => x.Firm)
                .WithMany(f => f.EconomicActivities)
                .HasForeignKey(x => x.FirmId);

            builder
                .HasOne(x => x.EconomicActivity)
                .WithMany()
                .HasForeignKey(x => x.EconomicActivityId);

            builder.Property(x => x.IsMain)
                .HasDefaultValue(false);
        }
    }
}
