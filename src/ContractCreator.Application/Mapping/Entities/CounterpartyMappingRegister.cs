using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.Data;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities
{
    public class CounterpartyMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BankAccount, BankAccountDto>();
            config.NewConfig<BankAccountDto, BankAccount>();

            config.NewConfig<Counterparty, CounterpartyDto>()
                .Map(dest => dest.LegalForm, src => src.LegalForm)
                .PreserveReference(true);

            config.NewConfig<CounterpartyDto, Counterparty>()
                .AddDestinationTransform((string? x) => string.IsNullOrWhiteSpace(x) ? null : x)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.BankAccounts)
                .Ignore(dest => dest.Contacts)
                .Ignore(dest => dest.Contracts)
                .Map(dest => dest.LegalForm, src => src.LegalForm);

            config.NewConfig<CounterpartyFile, EntityFileDto>();
            config.NewConfig<EntityFileDto, CounterpartyFile>();
        }
    }
}
