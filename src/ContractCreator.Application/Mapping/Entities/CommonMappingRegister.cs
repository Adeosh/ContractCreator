using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.Data;
using ContractCreator.Shared.Enums;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class CommonMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FileStorage, FileStorageDto>()
                  .Map(dest => dest.Type, src => (byte)src.Type);

            config.NewConfig<FileStorageDto, FileStorage>()
                  .Map(dest => dest.Type, src => (FileType)src.Type);

            config.NewConfig<ContractFile, EntityFileDto>();
            config.NewConfig<EntityFileDto, ContractFile>();

            config.NewConfig<CounterpartyFile, EntityFileDto>();
            config.NewConfig<EntityFileDto, CounterpartyFile>();

            config.NewConfig<FirmFile, EntityFileDto>();
            config.NewConfig<EntityFileDto, FirmFile>();

            config.NewConfig<GoodsAndService, GoodsAndServiceDto>()
                  .Map(dest => dest.Type, src => (byte)src.Type);

            config.NewConfig<GoodsAndServiceDto, GoodsAndService>()
                  .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x)
                  .Map(dest => dest.Type, src => (ProductType)src.Type)
                  .Ignore(dest => dest.CreatedDate);
        }
    }
}
