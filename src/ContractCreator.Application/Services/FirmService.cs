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
        private readonly IUnitOfWork _unitOfWork;

        public FirmService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FirmDto?> GetFirmByIdAsync(int id)
        {
            var spec = new FirmByIdWithDetailsSpec(id);
            var firm = await _unitOfWork.Repository<Firm>().FirstOrDefaultAsync(spec);

            return firm?.Adapt<FirmDto>();
        }

        public async Task<int> CreateFirmAsync(FirmDto dto)
        {
            var firm = dto.Adapt<Firm>();

            firm.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            firm.IsDeleted = false;

            await _unitOfWork.Repository<Firm>().AddAsync(firm);
            await _unitOfWork.SaveChangesAsync();

            return firm.Id;
        }

        public async Task UpdateFirmAsync(FirmDto dto)
        {
            var firm = await _unitOfWork.Repository<Firm>().GetByIdAsync(dto.Id);
            if (firm == null) throw new Exception("Фирма не найдена");

            dto.Adapt(firm);
            firm.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.Repository<Firm>().UpdateAsync(firm);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteFirmAsync(int id)
        {
            var firm = await _unitOfWork.Repository<Firm>().GetByIdAsync(id);
            if (firm != null)
            {
                await _unitOfWork.Repository<Firm>().DeleteAsync(firm);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FirmDto>> GetAllFirmsAsync()
        {
            var firms = await _unitOfWork.Repository<Firm>().ListAllAsync();
            return firms.Adapt<IEnumerable<FirmDto>>();
        }
    }
}
