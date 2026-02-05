using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<ContactDto>> GetContactsByCounterpartyIdAsync(int counterpartyId)
        {
            var contacts = await _unitOfWork.Repository<Contact>()
                .FindAsync(c => c.CounterpartyId == counterpartyId && !c.IsDeleted);
            return contacts.Adapt<IEnumerable<ContactDto>>();
        }

        public async Task<ContactDto?> GetContactByIdAsync(int id)
        {
            var contact = await _unitOfWork.Repository<Contact>().GetByIdAsync(id);
            return contact?.Adapt<ContactDto>();
        }

        public async Task<int> CreateContactAsync(ContactDto dto)
        {
            var contact = dto.Adapt<Contact>();
            await _unitOfWork.Repository<Contact>().AddAsync(contact);
            await _unitOfWork.SaveChangesAsync();
            return contact.Id;
        }

        public async Task UpdateContactAsync(ContactDto dto)
        {
            var contact = await _unitOfWork.Repository<Contact>().GetByIdAsync(dto.Id);
            if (contact == null) throw new Exception("Контакт не найден");

            dto.Adapt(contact);
            await _unitOfWork.Repository<Contact>().UpdateAsync(contact);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteContactAsync(int id)
        {
            var contact = await _unitOfWork.Repository<Contact>().GetByIdAsync(id);
            if (contact != null)
            {
                contact.IsDeleted = true;
                await _unitOfWork.Repository<Contact>().UpdateAsync(contact);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
