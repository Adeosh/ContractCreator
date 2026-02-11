using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;


namespace ContractCreator.Application.Mapping.Entities
{
    public class WorkerMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Worker, WorkerDto>();
            config.NewConfig<WorkerDto, Worker>()
                .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x);
        }
    }
}
