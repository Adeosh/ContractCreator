using System.Text.RegularExpressions;

namespace ContractCreator.Domain.ValueObjects
{
    public class EmailAddress
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Эл. почта не заполнена!");
            Value = value;
        }

        protected EmailAddress() { }

        public static EmailAddress Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым");

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException($"Некорректный формат Email: {email}");

            return new EmailAddress(email);
        }

        public static EmailAddress CreateConfig(string email) => new EmailAddress(email);

        public override string ToString() => Value;

        public static implicit operator string(EmailAddress email) => email.Value;
    }
}
