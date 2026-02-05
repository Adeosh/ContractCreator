using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Shared.DTOs.Data;
using Mapster;

namespace ContractCreator.Application.Mapping.Entities.Dictionaries
{
    public class DictionaryMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ClassifierOkved, ClassifierDto>()
                  .Map(dest => dest.Code, src => src.Code)
                  .Map(dest => dest.Name, src => src.Name);

            config.NewConfig<ClassifierOkopf, ClassifierDto>()
                  .Map(dest => dest.Code, src => src.Code)
                  .Map(dest => dest.Name, src => src.Name);

            config.NewConfig<ClassifierOkv, ClassifierDto>()
                  .Map(dest => dest.Code, src => src.LetterCode)
                  .Map(dest => dest.Name, src => src.CurrencyName);

            config.NewConfig<ClassifierBic, BankDto>()
                  .Map(dest => dest.Bic, src => src.BIC)
                  .Map(dest => dest.Name, src => src.Name)
                  .Map(dest => dest.CorrespondentAccount, src => src.Account ?? string.Empty);
        }
    }
}
