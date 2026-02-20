using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Firms;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class FirmService : IFirmService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public FirmService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<FirmDto?> GetFirmByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new FirmByIdWithDetailsSpec(id);
            var firm = await factory.Repository<Firm>().FirstOrDefaultAsync(spec);

            return firm?.Adapt<FirmDto>();
        }

        public async Task<int> CreateFirmAsync(FirmDto dto)
        {
            using var factory = _uowFactory.Create();

            var firm = dto.Adapt<Firm>();

            firm.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            firm.IsDeleted = false;

            await factory.Repository<Firm>().AddAsync(firm);
            await factory.SaveChangesAsync();

            return firm.Id;
        }

        public async Task UpdateFirmAsync(FirmDto dto)
        {
            using var factory = _uowFactory.Create();

            var spec = new FirmByIdWithDetailsSpec(dto.Id);
            var firm = await factory.Repository<Firm>().FirstOrDefaultAsync(spec);
            if (firm == null) throw new Exception("Фирма не найдена");

            dto.Adapt(firm);
            firm.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await factory.Repository<Firm>().UpdateAsync(firm);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteFirmAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new FirmByIdWithDetailsSpec(id);
            var firm = await factory.Repository<Firm>().FirstOrDefaultAsync(spec);
            if (firm != null)
            {
                await factory.Repository<Firm>().DeleteAsync(firm);
                await factory.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FirmDto>> GetAllFirmsAsync()
        {
            using var factory = _uowFactory.Create();

            var firms = await factory.Repository<Firm>().ListAllAsync();
            return firms.Adapt<IEnumerable<FirmDto>>();
        }
    }
}
