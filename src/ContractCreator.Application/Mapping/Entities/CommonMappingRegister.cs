using ContractCreator.Domain.Enums;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
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

            config.NewConfig<FirmFile, FirmFileDto>();
            config.NewConfig<FirmFileDto, FirmFile>();

            config.NewConfig<GoodsAndService, GoodsAndServiceDto>()
                  .Map(dest => dest.Type, src => (byte)src.Type);

            config.NewConfig<GoodsAndServiceDto, GoodsAndService>()
                  .Map(dest => dest.Type, src => (ProductType)src.Type)
                  .Ignore(dest => dest.CreatedDate);
        }
    }
}
