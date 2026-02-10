using ContractCreator.Domain.Interfaces;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Repositories;
using ContractCreator.Infrastructure.Services.Bic;
using ContractCreator.Infrastructure.Services.Classifiers;
using ContractCreator.Infrastructure.Services.Gar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContractCreator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IGarService, GarService>();
            services.AddScoped<IBicService, BicService>();
            services.AddScoped<IClassifierService, ClassifierService>();

            return services;
        }
    }
}
