using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Specifications.Contracts;
using ContractCreator.Shared.DTOs;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractDto>> GetContractsByFirmIdAsync(int firmId)
        {
            using var factory = _uowFactory.Create();

            var spec = new ContractsByFirmIdSpec(firmId);
            var contracts = await factory.Repository<Contract>().ListAsync(spec);

            return contracts.Adapt<IEnumerable<ContractDto>>();
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new ContractByIdWithDetailsSpec(id);
            var contract = await factory.Repository<Contract>().FirstOrDefaultAsync(spec);

            return contract?.Adapt<ContractDto>();
        }

        public async Task<int> CreateContractAsync(ContractDto dto)
        {
            using var factory = _uowFactory.Create();

            var entity = dto.Adapt<Contract>();

            await factory.Repository<Contract>().AddAsync(entity);
            await factory.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateContractAsync(ContractDto dto)
        {
            using var factory = _uowFactory.Create();

            var entity = await factory.Repository<Contract>().GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Контракт не найден");

            dto.Adapt(entity);

            await factory.Repository<Contract>().UpdateAsync(entity);
            await factory.SaveChangesAsync();
        }

        public async Task DeleteContractAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var entity = await factory.Repository<Contract>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<Contract>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }

        public async Task<List<ContractStageDto>> GetAllStagesAsync()
        {
            using var factory = _uowFactory.Create();

            var stages = await factory.Repository<ContractStage>().ListAllAsync();

            return stages.OrderBy(s => s.Id).Adapt<List<ContractStageDto>>();
        }

        public async Task<int> SaveContractWithDetailsAsync(
            ContractDto dto,
            IEnumerable<ContractSpecificationDto> specifications,
            IEnumerable<ContractStepDto> steps,
            int currentWorkerId)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var contractRepo = factory.Repository<Contract>();
                var specRepo = factory.Repository<ContractSpecification>();
                var stepRepo = factory.Repository<ContractStep>();
                var historyRepo = factory.Repository<ContractStageChangeHistory>();
                var fileRepo = factory.Repository<ContractFile>();

                Contract entity;
                bool isNewContract = dto.Id == 0;

                if (isNewContract)
                {
                    entity = dto.Adapt<Contract>();
                    await contractRepo.AddAsync(entity);
                    await factory.SaveChangesAsync();
                    dto.Id = entity.Id;
                }
                else
                {
                    var spec = new ContractByIdWithDetailsSpec(dto.Id);
                    entity = await contractRepo.FirstOrDefaultAsync(spec);

                    if (entity == null)
                        throw new Exception("Контракт не найден в базе данных.");

                    dto.Adapt(entity);
                    await contractRepo.UpdateAsync(entity);

                    foreach (var oldSpec in entity.Specifications.ToList())
                        await specRepo.DeleteAsync(oldSpec);

                    foreach (var oldStep in entity.Steps.ToList())
                        await stepRepo.DeleteAsync(oldStep);

                    foreach (var oldFile in entity.Files.ToList())
                        await fileRepo.DeleteAsync(oldFile);

                    await factory.SaveChangesAsync();
                }

                var specHistory = new ContractStageHistoryByContractIdSpec(entity.Id);
                var lastHistory = await historyRepo.FirstOrDefaultAsync(specHistory);

                if (isNewContract || lastHistory?.StageTypeId != dto.StageTypeId)
                {
                    await historyRepo.AddAsync(new ContractStageChangeHistory
                    {
                        ContractId = entity.Id,
                        WorkerId = currentWorkerId,
                        StageTypeId = dto.StageTypeId,
                        ChangeDate = DateTime.Now
                    });
                }

                foreach (var specDto in specifications)
                {
                    var specEntity = specDto.Adapt<ContractSpecification>();
                    specEntity.Id = 0;
                    specEntity.ContractId = entity.Id;
                    specEntity.CurrencyId = entity.CurrencyId;
                    await specRepo.AddAsync(specEntity);
                }

                foreach (var stepDto in steps)
                {
                    var stepEntity = stepDto.Adapt<ContractStep>();
                    stepEntity.Id = 0;
                    stepEntity.ContractId = entity.Id;
                    stepEntity.CurrencyId = entity.CurrencyId;

                    if (stepEntity.Items != null)
                        foreach (var item in stepEntity.Items)
                        {
                            item.Id = 0;
                            item.StepId = 0;
                        }

                    await stepRepo.AddAsync(stepEntity);
                }

                if (dto.Files != null)
                {
                    foreach (var fileDto in dto.Files)
                    {
                        await fileRepo.AddAsync(new ContractFile
                        {
                            ContractId = entity.Id,
                            FileId = fileDto.FileId,
                            Description = fileDto.Description
                        });
                    }
                }

                await factory.CommitTransactionAsync();

                return entity.Id;
            }
            catch (Exception)
            {
                await factory.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
