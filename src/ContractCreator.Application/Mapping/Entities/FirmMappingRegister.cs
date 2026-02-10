using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class FirmMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Firm, FirmDto>()
                .Map(dest => dest.LegalFormType, src => (byte)src.LegalFormType)
                .Map(dest => dest.TaxationType, src => (byte)src.TaxationType);

            config.NewConfig<FirmDto, Firm>()
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.BankAccounts)
                .Ignore(dest => dest.Workers)
                .Ignore(dest => dest.Contracts)
                .Ignore(dest => dest.Files)
                .Ignore(dest => dest.EconomicActivities)
                .Ignore(dest => dest.Okopf);
        }
    }
}
