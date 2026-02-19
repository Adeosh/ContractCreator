using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class WaybillMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ContractWaybillItem, ContractWaybillItemDto>()
            .Map(dest => dest.CurrencyName, src => src.Currency != null
            ? src.Currency.CurrencyName
            : string.Empty);

            config.NewConfig<ContractWaybillItemDto, ContractWaybillItem>();


            config.NewConfig<ContractWaybill, ContractWaybillDto>()
                .Map(dest => dest.CurrencyName, src => src.Currency != null
                ? src.Currency.CurrencyName
                : string.Empty)
                .PreserveReference(true);

            config.NewConfig<ContractWaybillDto, ContractWaybill>()
                .Ignore(dest => dest.Contract)
                .Ignore(dest => dest.Invoice)
                .Ignore(dest => dest.Currency);
        }
    }
}
