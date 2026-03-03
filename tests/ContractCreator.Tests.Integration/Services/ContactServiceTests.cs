using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ContactServiceTests : IntegrationTestBase
    {
        private readonly ContactService _contactService;

        public ContactServiceTests() : base()
        {
            _contactService = new ContactService(UowFactory);
        }

        private async Task<int> CreateTestContactViaServiceAsync(int counterpartyId = 1, string lastName = "Контактный")
        {
            var contactModel = TestDataFactory.CreateContact(0, counterpartyId);
            var dto = contactModel.Adapt<ContactDto>();

            dto.Id = 0;
            dto.CounterpartyId = counterpartyId;
            dto.LastName = lastName;
            dto.Email = "contact@test.ru";

            return await _contactService.CreateContactAsync(dto);
        }

        [Fact]
        public async Task CreateContactAsync_ShouldAddContactToDatabase()
        {
            // Arrange
            var contactModel = TestDataFactory.CreateContact(0, 2);
            var dto = contactModel.Adapt<ContactDto>();
            dto.Id = 0;
            dto.Email = "new_contact@test.ru";

            // Act
            var createdId = await _contactService.CreateContactAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var contactInDb = await db.Contacts.FirstOrDefaultAsync(c => c.Id == createdId);

            contactInDb.Should().NotBeNull();
            contactInDb!.LastName.Should().Be(dto.LastName);
            contactInDb.CounterpartyId.Should().Be(2);
            contactInDb.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetContactByIdAsync_ShouldReturnCorrectContact()
        {
            // Arrange
            var targetId = await CreateTestContactViaServiceAsync(1, "Иванов");

            // Act
            var result = await _contactService.GetContactByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.LastName.Should().Be("Иванов");
        }

        [Fact]
        public async Task UpdateContactAsync_ShouldModifyExistingContact()
        {
            // Arrange
            var targetId = await CreateTestContactViaServiceAsync(1, "ДоСвадьбы");
            var updateDto = await _contactService.GetContactByIdAsync(targetId);

            updateDto!.LastName = "ПослеСвадьбы";
            updateDto.Phone = "+7 000 000 00 00";

            // Act
            await _contactService.UpdateContactAsync(updateDto);

            // Assert
            var result = await _contactService.GetContactByIdAsync(targetId);
            result!.LastName.Should().Be("ПослеСвадьбы");
            result.Phone.Should().Be("+7 000 000 00 00");
        }

        [Fact]
        public async Task DeleteContactAsync_ShouldSoftDeleteContact()
        {
            // Arrange
            var targetId = await CreateTestContactViaServiceAsync(1, "Лишний Контакт");

            // Act
            await _contactService.DeleteContactAsync(targetId);

            // Assert
            using var db = CreateContext();
            var contactInDb = await db.Contacts.FirstOrDefaultAsync(c => c.Id == targetId);
            contactInDb.Should().NotBeNull();
            contactInDb!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetContactsByCounterpartyIdAsync_ShouldReturnOnlyActiveContactsForSpecificCounterparty()
        {
            // Arrange
            int targetCpId = 5;
            int otherCpId = 6;

            var id1 = await CreateTestContactViaServiceAsync(targetCpId, "Контакт 5 - А");
            var id2 = await CreateTestContactViaServiceAsync(targetCpId, "Контакт 5 - Б");
            var id3 = await CreateTestContactViaServiceAsync(otherCpId, "Контакт 6");

            await _contactService.DeleteContactAsync(id2);

            // Act
            var result = await _contactService.GetContactsByCounterpartyIdAsync(targetCpId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().ContainSingle();
            dtos.Should().Contain(c => c.Id == id1);
            dtos.Should().NotContain(c => c.Id == id2);
            dtos.Should().NotContain(c => c.Id == id3);
        }

        [Fact]
        public async Task GetAllContactsAsync_ShouldReturnEvenDeleted()
        {
            // Arrange
            var idActive = await CreateTestContactViaServiceAsync(1, "Активный");
            var idDeleted = await CreateTestContactViaServiceAsync(1, "Удаленный");

            await _contactService.DeleteContactAsync(idDeleted);

            // Act
            var result = await _contactService.GetAllContactsAsync();

            // Assert
            var dtos = result.ToList();
            dtos.Should().Contain(c => c.Id == idActive);
            dtos.Should().Contain(c => c.Id == idDeleted);
        }
    }
}
