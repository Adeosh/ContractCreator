using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContractCreator.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = "Host=localhost;Port=5432;Database=ContractCreatorDb;Username=postgres;Password=zaq1234";

            optionsBuilder.UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
