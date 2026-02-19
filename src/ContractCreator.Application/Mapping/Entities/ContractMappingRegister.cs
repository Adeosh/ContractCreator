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
                .Ignore(dest => dest.Currency);

            config.NewConfig<Contract, ContractDto>()
                .Map(dest => dest.Type, src => (byte)src.Type)
                .Map(dest => dest.EnterpriseRole, src => (byte)src.EnterpriseRole)
                .Map(dest => dest.Initiator, src => src.Initiator.HasValue
                ? (byte?)src.Initiator.Value
                : null)
                .PreserveReference(true);

            config.NewConfig<ContractDto, Contract>()
                .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x)
                .Map(dest => dest.Type, src => (ContractType)src.Type)
                .Map(dest => dest.EnterpriseRole, src => (ContractEnterpriseRole)src.EnterpriseRole)
                .Map(dest => dest.Initiator, src => src.Initiator.HasValue
                ? (TerminationInitiator?)src.Initiator.Value
                : null);

            config.NewConfig<ContractStageType, ContractStageDto>();
            config.NewConfig<ContractStageChangeHistory, ContractStageChangeHistoryDto>();
        }
    }
}
