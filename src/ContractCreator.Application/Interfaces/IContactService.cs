using ContractCreator.Shared.DTOs;

namespace ContractCreator.Application.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllContactsAsync();
        Task<IEnumerable<ContactDto>> GetContactsByCounterpartyIdAsync(int counterpartyId);
        Task<ContactDto?> GetContactByIdAsync(int id);
        Task<int> CreateContactAsync(ContactDto dto);
        Task UpdateContactAsync(ContactDto dto);
        Task DeleteContactAsync(int id);
    }
}
