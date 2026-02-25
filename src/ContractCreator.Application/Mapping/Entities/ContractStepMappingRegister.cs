using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class ContractStepMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ContractStepItem, ContractStepItemDto>()
            .Map(dest => dest.CurrencyName, src => src.Currency != null
            ? src.Currency.CurrencyName
            : string.Empty);

            config.NewConfig<ContractStepItemDto, ContractStepItem>();


            config.NewConfig<ContractStep, ContractStepDto>()
                .Map(dest => dest.CurrencyName, src => src.Currency != null
                ? src.Currency.CurrencyName
                : string.Empty)
                .PreserveReference(true);

            config.NewConfig<ContractStepDto, ContractStep>()
                .Ignore(dest => dest.Contract)
                .Ignore(dest => dest.Currency)
                .Ignore(dest => dest.Items);
        }
    }
}
