using ContractCreator.Application.Interfaces;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models;
using ContractCreator.Domain.Services;
using ContractCreator.Domain.Specifications.Contracts.Documents;
using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.PrintForms;
using ContractCreator.Shared.Helpers;
using Mapster;

namespace ContractCreator.Application.Services
{
    public class ContractActService : IContractActService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractActService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractActDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allActs = await factory.Repository<ContractAct>().ListAllAsync();
            var filtered = allActs.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractActDto>>();
        }

        public async Task<ContractActDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new ActByIdWithDetailsSpec(id);
            var act = await factory.Repository<ContractAct>().FirstOrDefaultAsync(spec);

            return act?.Adapt<ContractActDto>();
        }

        public async Task<int> CreateAsync(ContractActDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var entity = dto.Adapt<ContractAct>();

                await factory.Repository<ContractAct>().AddAsync(entity);
                await factory.SaveChangesAsync();

                if (dto.Items != null && dto.Items.Any())
                {
                    var itemRepo = factory.Repository<ContractActItem>();
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractActItem>();
                        itemEntity.Id = 0;
                        itemEntity.ActId = entity.Id;
                        itemEntity.CurrencyId = entity.CurrencyId;
                        await itemRepo.AddAsync(itemEntity);
                    }
                    await factory.SaveChangesAsync();
                }

                await factory.CommitTransactionAsync();
                return entity.Id;
            }
            catch
            {
                await factory.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateAsync(ContractActDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var actRepo = factory.Repository<ContractAct>();
                var itemRepo = factory.Repository<ContractActItem>();

                var spec = new ActByIdWithDetailsSpec(dto.Id);
                var entity = await actRepo.FirstOrDefaultAsync(spec);

                if (entity == null) throw new Exception("Акт не найден");

                dto.Adapt(entity);
                await actRepo.UpdateAsync(entity);

                foreach (var oldItem in entity.Items.ToList())
                    await itemRepo.DeleteAsync(oldItem);

                await factory.SaveChangesAsync();

                if (dto.Items != null)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractActItem>();
                        itemEntity.Id = 0;
                        itemEntity.ActId = entity.Id;
                        await itemRepo.AddAsync(itemEntity);
                    }
                }

                await factory.SaveChangesAsync();
                await factory.CommitTransactionAsync();
            }
            catch
            {
                await factory.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var factory = _uowFactory.Create();
            var entity = await factory.Repository<ContractAct>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractAct>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }

        public async Task<ActPrintDto> GetPrintDataAsync(int actId)
        {
            using var factory = _uowFactory.Create();

            var spec = new ActPrintSpec(actId);
            var act = await factory.Repository<ContractAct>().FirstOrDefaultAsync(spec);

            if (act == null) 
                throw new Exception($"Акт с ID {actId} не найден.");
            if (act.Contract == null) 
                throw new Exception($"У акта №{act.ActNumber} отсутствует привязка к контракту.");
            if (act.Contract.Firm == null) 
                throw new Exception($"В контракте акта №{act.ActNumber} не указан Исполнитель.");
            if (act.Contract.Counterparty == null) 
                throw new Exception($"В контракте акта №{act.ActNumber} не указан Заказчик.");

            var firm = act.Contract.Firm;
            var counterparty = act.Contract.Counterparty;

            string firmFullData = DataHelper.CreateOrganizationFullDataString(
                fullName: firm.FullName,
                inn: firm.INN,
                kpp: firm.KPP,
                address: firm.ActualAddress,
                phone: firm.Phone,
                email: firm.Email
            );

            string counterpartyFullData = DataHelper.CreateOrganizationFullDataString(
                fullName: counterparty.FullName,
                inn: counterparty.INN,
                kpp: counterparty.KPP,
                address: counterparty.ActualAddress,
                phone: counterparty.Phone,
                email: counterparty.Email ?? string.Empty
            );

            string contractorName = "Не указан";
            string customerName = "Не указан";

            if (firm.Workers != null)
            {
                var dir = firm.Workers.FirstOrDefault(w => w.Id == act.ContractorId && !w.IsDeleted);
                if (dir != null)
                    contractorName = DataHelper.CreateFIOString(dir.LastName, dir.FirstName, dir.MiddleName ?? string.Empty, "f");
            }

            if (counterparty.Contacts != null)
            {
                var signer = counterparty.Contacts.FirstOrDefault(c => c.Id == act.CustomerId && !c.IsDeleted);
                if (signer != null)
                    customerName = DataHelper.CreateFIOString(signer.LastName, signer.FirstName, signer.MiddleName ?? string.Empty, "f");
            }

            var dto = new ActPrintDto
            {
                DocumentId = Guid.NewGuid(),
                DocumentDate = DateTime.Now,

                ActNumber = act.ActNumber,
                ActDate = act.ActDate.ToString("dd.MM.yyyy"),

                ContractNumber = act.Contract.ContractNumber,
                ContractDate = act.Contract.IssueDate != null ? act.Contract.IssueDate.Value.ToString("dd.MM.yyyy") : string.Empty,
                FirmName = firm.FullName,
                CounterpartyName = counterparty.FullName,

                ContractorFullData = firmFullData,
                CustomerFullData = counterpartyFullData,
                
                TotalAmount = act.TotalAmount ?? 0,
                FormattedTotalAmount = CurrencyFormatter.ShortFormatCurrency(act.TotalAmount ?? 0, act.CurrencyId),
                VATAmount = act.VATAmount ?? 0,
                FormattedVATAmount = CurrencyFormatter.ShortFormatCurrency(act.VATAmount ?? 0, act.CurrencyId),
                AggregateAmount = act.AggregateAmount,
                FormattedAggregateAmount = CurrencyFormatter.ShortFormatCurrency(act.AggregateAmount, act.CurrencyId),

                AmountInWords = CurrencyFormatter.AmountToWords(act.AggregateAmount, act.CurrencyId),

                ContractorDirectorName = contractorName,
                CustomerSignatoryName = customerName
            };

            if (act.Items != null && act.Items.Any())
            {
                int num = 1;
                foreach (var item in act.Items.OrderBy(i => i.Id))
                {
                    dto.Items.Add(new ActPrintItemDto
                    {
                        Number = num++,
                        NomenclatureName = item.NomenclatureName,
                        Quantity = item.Quantity,
                        Unit = item.UnitOfMeasure ?? "шт.",
                        UnitPrice = item.UnitPrice,
                        TotalAmount = item.TotalAmount
                    });
                }
            }

            return dto;
        }
    }
}
