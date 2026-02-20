using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContactService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContactDto>> GetAllContactsAsync()
        {
            using var factory = _uowFactory.Create();

            var contacts = await factory.Repository<Contact>().ListAllAsync();
            return contacts.Adapt<IEnumerable<ContactDto>>();
        }

        public async Task<IEnumerable<ContactDto>> GetContactsByCounterpartyIdAsync(int counterpartyId)
        {
            using var factory = _uowFactory.Create();

            var contacts = await factory.Repository<Contact>()
                .FindAsync(c => c.CounterpartyId == counterpartyId && !c.IsDeleted);
            return contacts.Adapt<IEnumerable<ContactDto>>();
        }

        public async Task<ContactDto?> GetContactByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var contact = await factory.Repository<Contact>().GetByIdAsync(id);
            return contact?.Adapt<ContactDto>();
        }

        public async Task<int> CreateContactAsync(ContactDto dto)
        {
            using var factory = _uowFactory.Create();

            var contact = dto.Adapt<Contact>();
            await factory.Repository<Contact>().AddAsync(contact);
            await factory.SaveChangesAsync();
            return contact.Id;
        }

        public async Task UpdateContactAsync(ContactDto dto)
        {
            using var factory = _uowFactory.Create();

            var contact = await factory.Repository<Contact>().GetByIdAsync(dto.Id);
            if (contact == null) throw new Exception("Контакт не найден");

            dto.Adapt(contact);
            await factory.Repository<Contact>().UpdateAsync(contact);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteContactAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var contact = await factory.Repository<Contact>().GetByIdAsync(id);
            if (contact != null)
            {
                contact.IsDeleted = true;
                await factory.Repository<Contact>().UpdateAsync(contact);
                await factory.SaveChangesAsync();
            }
        }
    }
}
