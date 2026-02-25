using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class ActMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ContractActItem, ContractActItemDto>()
            .Map(dest => dest.CurrencyName, src => src.Currency != null
            ? src.Currency.CurrencyName
            : string.Empty);

            config.NewConfig<ContractActItemDto, ContractActItem>();


            config.NewConfig<ContractAct, ContractActDto>()
                .Map(dest => dest.CurrencyName, src => src.Currency != null
                ? src.Currency.CurrencyName
                : string.Empty)
                .PreserveReference(true);

            config.NewConfig<ContractActDto, ContractAct>()
                .Ignore(dest => dest.Contract)
                .Ignore(dest => dest.Invoice)
                .Ignore(dest => dest.Currency)
                .Ignore(dest => dest.Items);
        }
    }
}
