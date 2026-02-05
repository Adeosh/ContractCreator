using ContractCreator.Application.Interfaces;
using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContractCreator.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            MappingConfig.Configure();

            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ICounterpartyService, CounterpartyService>();
            services.AddScoped<IFirmService, FirmService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IWorkerService, WorkerService>();

            return services;
        }
    }
}
