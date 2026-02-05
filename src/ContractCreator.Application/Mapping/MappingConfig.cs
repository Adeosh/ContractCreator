using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.DTOs.Data;
using Mapster;
using System.Reflection;

namespace ContractCreator.Application.Mapping
{
    public static class MappingConfig
    {
        private static bool _isConfigured;
        private static readonly object _lock = new();

        public static void Configure()
        {
            lock (_lock)
            {
                if (_isConfigured) return;

                TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;

                config.NewConfig<AddressData, AddressDto>();
                config.NewConfig<AddressDto, AddressData>();

                config.NewConfig<EmailAddress, string>()
                      .MapWith(email => email.Value);
                config.NewConfig<string, EmailAddress>()
                      .MapWith(str => string.IsNullOrWhiteSpace(str) ? null! : new EmailAddress(str));
                config.NewConfig<EmailAddress, string>()
                      .MapWith(src => src == null ? string.Empty : src.Value);

                config.Scan(Assembly.GetExecutingAssembly());
                config.Default.PreserveReference(true);

                _isConfigured = true;
            }
        }
    }
}
