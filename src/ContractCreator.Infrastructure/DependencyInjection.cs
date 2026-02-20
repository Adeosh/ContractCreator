using ContractCreator.Application.Interfaces.Infrastructure;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Infrastructure.Repositories;
using ContractCreator.Infrastructure.Services.Bic;
using ContractCreator.Infrastructure.Services.Classifiers;
using ContractCreator.Infrastructure.Services.Files;
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

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddScoped<IGarService, GarService>();
            services.AddScoped<IBicService, BicService>();
            services.AddScoped<IClassifierService, ClassifierService>();
            services.AddScoped<IFileService, FileService>();

            return services;
        }
    }
}
