using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Mapster;
using ContractCreator.Application.Services;
using ContractCreator.Shared.DTOs;
using ContractCreator.Tests.Integration.Data;

namespace ContractCreator.Tests.Integration
{
    public class ContractSpecificationServiceTests : IntegrationTestBase
    {
        private readonly ContractSpecificationService _specificationService;

        public ContractSpecificationServiceTests() : base()
        {
            _specificationService = new ContractSpecificationService(UowFactory);
        }

        private async Task<int> CreateTestSpecificationViaServiceAsync(int contractId = 1, string nomenclature = "Товар")
        {
            var model = TestDataFactory.CreateSpecification(0, contractId);
            var dto = model.Adapt<ContractSpecificationDto>();

            dto.Id = 0;
            dto.ContractId = contractId;
            dto.NomenclatureName = nomenclature;

            return await _specificationService.CreateAsync(dto);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddSpecificationToDatabase()
        {
            // Arrange
            var model = TestDataFactory.CreateSpecification(0, 2);
            var dto = model.Adapt<ContractSpecificationDto>();
            dto.Id = 0;
            dto.NomenclatureName = "Новая Спецификация";

            // Act
            var createdId = await _specificationService.CreateAsync(dto);

            // Assert
            createdId.Should().BeGreaterThan(0);

            using var db = CreateContext();
            var specInDb = await db.ContractSpecifications.FirstOrDefaultAsync(s => s.Id == createdId);

            specInDb.Should().NotBeNull();
            specInDb!.NomenclatureName.Should().Be(dto.NomenclatureName);
            specInDb.ContractId.Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectSpecification()
        {
            // Arrange
            var targetId = await CreateTestSpecificationViaServiceAsync(1, "Искомый Товар");

            // Act
            var result = await _specificationService.GetByIdAsync(targetId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.NomenclatureName.Should().Be("Искомый Товар");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingSpecification()
        {
            // Arrange
            var targetId = await CreateTestSpecificationViaServiceAsync(1, "Старое Название");
            var updateDto = await _specificationService.GetByIdAsync(targetId);

            updateDto!.NomenclatureName = "Новое Название";
            updateDto.Quantity = 99;

            // Act
            await _specificationService.UpdateAsync(updateDto);

            // Assert
            var result = await _specificationService.GetByIdAsync(targetId);
            result!.NomenclatureName.Should().Be("Новое Название");
            result.Quantity.Should().Be(99);
        }

        [Fact]
        public async Task DeleteAsync_ShouldHardDeleteSpecification()
        {
            // Arrange
            var targetId = await CreateTestSpecificationViaServiceAsync();

            // Act
            await _specificationService.DeleteAsync(targetId);

            // Assert
            using var db = CreateContext();
            var specInDb = await db.ContractSpecifications.FirstOrDefaultAsync(s => s.Id == targetId);
            specInDb.Should().BeNull();
        }

        [Fact]
        public async Task GetByContractIdAsync_ShouldReturnOnlyForSpecificContract()
        {
            // Arrange
            int targetContractId = 100;
            int otherContractId = 200;

            var id1 = await CreateTestSpecificationViaServiceAsync(targetContractId, "Спец 100 - А");
            var id2 = await CreateTestSpecificationViaServiceAsync(targetContractId, "Спец 100 - Б");
            var id3 = await CreateTestSpecificationViaServiceAsync(otherContractId, "Спец 200");

            // Act
            var result = await _specificationService.GetByContractIdAsync(targetContractId);

            // Assert
            var dtos = result.ToList();
            dtos.Should().HaveCount(2);
            dtos.Should().Contain(s => s.Id == id1);
            dtos.Should().Contain(s => s.Id == id2);
            dtos.Should().NotContain(s => s.Id == id3);
        }
    }
}