using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class ProductMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<GoodsAndService, GoodsAndServiceDto>()
                .Map(dest => dest.Type, src => src.Type);

            config.NewConfig<GoodsAndServiceDto, GoodsAndService>()
                .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.Currency)
                .Map(dest => dest.Type, src => src.Type);
        }
    }
}
