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
                .Ignore(dest => dest.CreatedDate);
        }
    }
}
