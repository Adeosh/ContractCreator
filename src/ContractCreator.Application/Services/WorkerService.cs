using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkerService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<WorkerDto>> GetWorkersByFirmIdAsync(int firmId)
        {
            var workers = await _unitOfWork.Repository<Worker>()
                .FindAsync(w => w.FirmId == firmId && !w.IsDeleted);

            return workers.Adapt<IEnumerable<WorkerDto>>();
        }

        public async Task<WorkerDto?> GetWorkerByIdAsync(int id)
        {
            var worker = await _unitOfWork.Repository<Worker>().GetByIdAsync(id);
            return worker?.Adapt<WorkerDto>();
        }

        public async Task<int> CreateWorkerAsync(WorkerDto dto)
        {
            var worker = dto.Adapt<Worker>();
            worker.IsDeleted = false;

            await _unitOfWork.Repository<Worker>().AddAsync(worker);
            await _unitOfWork.SaveChangesAsync();
            return worker.Id;
        }

        public async Task UpdateWorkerAsync(WorkerDto dto)
        {
            var worker = await _unitOfWork.Repository<Worker>().GetByIdAsync(dto.Id);
            if (worker == null) throw new Exception("Сотрудник не найден");

            dto.Adapt(worker);

            await _unitOfWork.Repository<Worker>().UpdateAsync(worker);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteWorkerAsync(int id)
        {
            var worker = await _unitOfWork.Repository<Worker>().GetByIdAsync(id);
            if (worker != null)
            {
                worker.IsDeleted = true;
                await _unitOfWork.Repository<Worker>().UpdateAsync(worker);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
