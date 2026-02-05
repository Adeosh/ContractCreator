namespace ContractCreator.Shared.DTOs
{
    public class ContactDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Note { get; set; }
        public bool IsDirector { get; set; }
        public bool IsAccountant { get; set; }
        public bool IsDeleted { get; set; }
        public int CounterpartyId { get; set; }
    }
}
