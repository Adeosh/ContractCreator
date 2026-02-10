using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class InvoiceMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ContractInvoiceItem, ContractInvoiceItemDto>()
            .Map(dest => dest.CurrencyName, src => src.Currency != null
            ? src.Currency.CurrencyName
            : string.Empty);

            config.NewConfig<ContractInvoiceItemDto, ContractInvoiceItem>();


            config.NewConfig<ContractInvoice, ContractInvoiceDto>()
                .Map(dest => dest.CurrencyName, src => src.Currency != null
                ? src.Currency.CurrencyName
                : string.Empty)
                .PreserveReference(true);

            config.NewConfig<ContractInvoiceDto, ContractInvoice>()
                .Ignore(dest => dest.Contract)
                .Ignore(dest => dest.Currency);
        }
    }
}
