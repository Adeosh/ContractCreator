using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Counterparties;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class CounterpartyService : ICounterpartyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CounterpartyService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<CounterpartyDto>> GetAllCounterpartiesAsync()
        {
            var list = await _unitOfWork.Repository<Counterparty>()
                .FindAsync(c => !c.IsDeleted);
            return list.Adapt<IEnumerable<CounterpartyDto>>();
        }

        public async Task<CounterpartyDto?> GetCounterpartyByIdAsync(int id)
        {
            var spec = new CounterpartyByIdWithDetailsSpec(id);
            var counterparty = await _unitOfWork.Repository<Counterparty>().FirstOrDefaultAsync(spec);

            return counterparty?.Adapt<CounterpartyDto>();
        }

        public async Task<int> CreateCounterpartyAsync(CounterpartyDto dto)
        {
            var entity = dto.Adapt<Counterparty>();
            entity.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            entity.IsDeleted = false;

            await _unitOfWork.Repository<Counterparty>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateCounterpartyAsync(CounterpartyDto dto)
        {
            var spec = new CounterpartyByIdWithDetailsSpec(dto.Id);
            var entity = await _unitOfWork.Repository<Counterparty>().FirstOrDefaultAsync(spec);

            if (entity == null) throw new Exception("Контрагент не найден");

            dto.Adapt(entity);
            entity.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.Repository<Counterparty>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCounterpartyAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Counterparty>().GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                await _unitOfWork.Repository<Counterparty>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
