using ContractCreator.Domain.ValueObjects;

namespace ContractCreator.Domain.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public required string Position { get; set; }
        public required string Phone { get; set; }
        public EmailAddress? Email { get; set; }
        public string? Note { get; set; }
        public bool IsDirector { get; set; }
        public bool IsAccountant { get; set; }
        public bool IsDeleted { get; set; }

        public int CounterpartyId { get; set; }
        public virtual Counterparty Counterparty { get; set; } = null!;
    }
}
