using ContractCreator.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContractCreator.Infrastructure.Persistence.Converters
{
    public class EmailAddressConverter : ValueConverter<EmailAddress, string>
    {
        public EmailAddressConverter()
            : base(
                email => email.Value,
                value => EmailAddress.CreateConfig(value)
            )
        {
        }
    }
}
