using ContractCreator.Domain.Enums;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
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
                .Map(dest => dest.LegalForm, src => (byte)src.LegalForm)
                .PreserveReference(true);

            config.NewConfig<CounterpartyDto, Counterparty>()
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Map(dest => dest.LegalForm, src => (LegalFormType)src.LegalForm);

            config.NewConfig<CounterpartyFile, CounterpartyFileDto>();
            config.NewConfig<CounterpartyFileDto, CounterpartyFile>();
        }
    }
}
