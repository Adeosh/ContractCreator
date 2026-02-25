using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.Enums;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class ContractMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ContractSpecification, ContractSpecificationDto>()
                .Map(dest => dest.CurrencyName, src => src.Currency != null
                    ? src.Currency.CurrencyName
                    : string.Empty);

            config.NewConfig<ContractSpecificationDto, ContractSpecification>()
                .Ignore(dest => dest.Contract)
                .Ignore(dest => dest.Currency);

            config.NewConfig<Contract, ContractDto>()
                .Map(dest => dest.Type, src => src.Type)
                .Map(dest => dest.EnterpriseRole, src => src.EnterpriseRole)
                .Map(dest => dest.Initiator, src => src.Initiator.HasValue ? (byte?)src.Initiator.Value : null)
                .Map(dest => dest.CounterpartyName, src => src.Counterparty != null ? src.Counterparty.ShortName : string.Empty)
                .PreserveReference(true);

            config.NewConfig<ContractDto, Contract>()
                .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x)
                .Map(dest => dest.Type, src => src.Type)
                .Map(dest => dest.EnterpriseRole, src => src.EnterpriseRole)
                .Map(dest => dest.Initiator, src => src.Initiator.HasValue ? (TerminationInitiator?)src.Initiator.Value : null)
                .Ignore(dest => dest.Counterparty)
                .Ignore(dest => dest.CounterpartySigner)
                .Ignore(dest => dest.Firm)
                .Ignore(dest => dest.FirmSigner)
                .Ignore(dest => dest.Currency)
                .Ignore(dest => dest.StageType)
                .Ignore(dest => dest.Specifications)
                .Ignore(dest => dest.Files)
                .Ignore(dest => dest.Acts)
                .Ignore(dest => dest.Invoices)
                .Ignore(dest => dest.Waybills)
                .Ignore(dest => dest.Steps);

            config.NewConfig<ContractStageType, ContractStageDto>();
            config.NewConfig<ContractStageChangeHistory, ContractStageChangeHistoryDto>();
        }
    }
}
