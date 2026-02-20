using ContractCreator.Application.Mapping;
using ContractCreator.Application.Services;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Shared.DTOs;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ContractCreator.Tests.Unit.Services
{
    public class ContactServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRepository<Contact>> _contactRepoMock;
        private readonly ContactService _service;

        public ContactServiceTests()
        {
            MappingConfig.Configure();

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            _uowMock = new Mock<IUnitOfWork>();
            _contactRepoMock = new Mock<IRepository<Contact>>();
            _uowMock.Setup(x => x.Repository<Contact>()).Returns(_contactRepoMock.Object);
            _uowFactoryMock.Setup(x => x.Create()).Returns(_uowMock.Object);
            _service = new ContactService(_uowFactoryMock.Object);
        }

        [Fact]
        public async Task GetContactsByCounterpartyIdAsync_ShouldReturnFilteredList()
        {
            // Arrange
            var targetId = 10;
            var allContacts = new List<Contact>
            {
                new Contact
                {
                    Id = 1,
                    CounterpartyId = targetId,
                    IsDeleted = false,
                    FirstName = "Valid",
                    LastName = "Contact",
                    Phone = "123",
                    Email = new EmailAddress("valid@test.com"),
                    Position = "Директор"
                },
                new Contact
                {
                    Id = 2,
                    CounterpartyId = 999,
                    IsDeleted = false,
                    FirstName = "Other",
                    LastName = "Contact",
                    Phone = "123",
                    Email = new EmailAddress("other@test.com"),
                    Position = "Бухгалтер"
                },
                new Contact
                {
                    Id = 3,
                    CounterpartyId = targetId,
                    IsDeleted = true,
                    FirstName = "Deleted",
                    LastName = "Contact",
                    Phone = "123",
                    Email = new EmailAddress("del@test.com"),
                    Position = "Мл. бухгалтер"
                }
            };

            // Настраиваем FindAsync с компиляцией выражения (чтобы Linq работал в памяти)
            _contactRepoMock
                .Setup(x => x.FindAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                .ReturnsAsync((Expression<Func<Contact, bool>> predicate) =>
                {
                    return allContacts.Where(predicate.Compile()).ToList();
                });

            // Act
            var result = await _service.GetContactsByCounterpartyIdAsync(targetId);

            // Assert
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(1);
            result.First().FirstName.Should().Be("Valid");
        }

        [Fact]
        public async Task GetContactByIdAsync_ShouldReturnDto_WhenExists()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 5,
                FirstName = "Ivan",
                LastName = "Ivanov",
                Phone = "000",
                Email = new EmailAddress("ivan@test.com"),
                Position = "Бухгалтер"
            };

            _contactRepoMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(contact);

            // Act
            var result = await _service.GetContactByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(5);
            result.Email.Should().Be("ivan@test.com");
        }

        [Fact]
        public async Task GetContactByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _contactRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Contact?)null);

            // Act
            var result = await _service.GetContactByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateContactAsync_ShouldAddEntityAndSave()
        {
            // Arrange
            var dto = new ContactDto
            {
                CounterpartyId = 10,
                FirstName = "New",
                LastName = "Contact",
                Phone = "+79990000000",
                Email = "new@test.com",
                Position = "Манагер"
            };

            // Act
            await _service.CreateContactAsync(dto);

            // Assert
            _contactRepoMock.Verify(x => x.AddAsync(It.Is<Contact>(c =>
                c.FirstName == "New" &&
                c.Email!.Value == "new@test.com" &&
                c.CounterpartyId == 10
            )), Times.Once);

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateContactAsync_ShouldUpdateFields_WhenExists()
        {
            // Arrange
            var contactId = 7;
            var existingContact = new Contact
            {
                Id = contactId,
                FirstName = "OldName",
                LastName = "OldLast",
                Phone = "111",
                Email = new EmailAddress("old@test.com"),
                Position = "Бухгалтер"
            };

            var updateDto = new ContactDto
            {
                Id = contactId,
                FirstName = "NewName",
                LastName = "NewLast",
                Phone = "222",
                Email = "new@test.com",
                Position = "Манагер"
            };

            _contactRepoMock.Setup(x => x.GetByIdAsync(contactId)).ReturnsAsync(existingContact);

            // Act
            await _service.UpdateContactAsync(updateDto);

            // Assert
            existingContact.FirstName.Should().Be("NewName");
            existingContact.Email!.Value.Should().Be("new@test.com");
            existingContact.Phone.Should().Be("222");

            _contactRepoMock.Verify(x => x.UpdateAsync(existingContact), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateContactAsync_ShouldThrow_WhenNotFound()
        {
            // Arrange
            _contactRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Contact?)null);
            var dto = new ContactDto { Id = 999 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateContactAsync(dto));
            ex.Message.Should().Be("Контакт не найден");

            _contactRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task DeleteContactAsync_ShouldSoftDelete()
        {
            // Arrange
            var contactId = 15;
            var contact = new Contact
            {
                Id = contactId,
                IsDeleted = false,
                FirstName = "Todelete",
                LastName = "Todelete",
                Phone = "0",
                Email = new EmailAddress("a@a.ru"),
                Position = "Бухгалтер"
            };

            _contactRepoMock.Setup(x => x.GetByIdAsync(contactId)).ReturnsAsync(contact);

            // Act
            await _service.DeleteContactAsync(contactId);

            // Assert
            contact.IsDeleted.Should().BeTrue();

            _contactRepoMock.Verify(x => x.UpdateAsync(contact), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
