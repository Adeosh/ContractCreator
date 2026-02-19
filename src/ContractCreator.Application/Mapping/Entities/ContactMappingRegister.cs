using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class ContactMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Contact, ContactDto>();
            config.NewConfig<ContactDto, Contact>()
                .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x);
        }
    }
}
