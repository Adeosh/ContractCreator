using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public WorkerService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<WorkerDto>> GetWorkersByFirmIdAsync(int firmId)
        {
            using var factory = _uowFactory.Create();

            var workers = await factory.Repository<Worker>()
                .FindAsync(w => w.FirmId == firmId && !w.IsDeleted);

            return workers.Adapt<IEnumerable<WorkerDto>>();
        }

        public async Task<WorkerDto?> GetWorkerByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var worker = await factory.Repository<Worker>().GetByIdAsync(id);
            return worker?.Adapt<WorkerDto>();
        }

        public async Task<int> CreateWorkerAsync(WorkerDto dto)
        {
            using var factory = _uowFactory.Create();

            var worker = dto.Adapt<Worker>();
            worker.IsDeleted = false;

            await factory.Repository<Worker>().AddAsync(worker);
            await factory.SaveChangesAsync();
            return worker.Id;
        }

        public async Task UpdateWorkerAsync(WorkerDto dto)
        {
            using var factory = _uowFactory.Create();

            var worker = await factory.Repository<Worker>().GetByIdAsync(dto.Id);
            if (worker == null) 
                throw new Exception("Сотрудник не найден");

            dto.Adapt(worker);

            await factory.Repository<Worker>().UpdateAsync(worker);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteWorkerAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var worker = await factory.Repository<Worker>().GetByIdAsync(id);
            if (worker != null)
            {
                worker.IsDeleted = true;
                await factory.Repository<Worker>().UpdateAsync(worker);
                await factory.SaveChangesAsync();
            }
        }
    }
}
