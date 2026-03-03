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
    public class ContractWaybillService : IContractWaybillService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractWaybillService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractWaybillDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allWaybills = await factory.Repository<ContractWaybill>().ListAllAsync();
            var filtered = allWaybills.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractWaybillDto>>();
        }

        public async Task<ContractWaybillDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new WaybillByIdWithDetailsSpec(id);
            var waybill = await factory.Repository<ContractWaybill>().FirstOrDefaultAsync(spec);

            return waybill?.Adapt<ContractWaybillDto>();
        }

        public async Task<int> CreateAsync(ContractWaybillDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync(); // Открываем транзакцию

            try
            {
                var entity = dto.Adapt<ContractWaybill>();

                await factory.Repository<ContractWaybill>().AddAsync(entity);
                await factory.SaveChangesAsync(); // Сохраняем, чтобы получить Id шапки

                if (dto.Items != null && dto.Items.Any())
                {
                    var itemRepo = factory.Repository<ContractWaybillItem>();
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractWaybillItem>();
                        itemEntity.Id = 0;
                        itemEntity.WaybillId = entity.Id;
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

        public async Task UpdateAsync(ContractWaybillDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var waybillRepo = factory.Repository<ContractWaybill>();
                var itemRepo = factory.Repository<ContractWaybillItem>();

                var spec = new WaybillByIdWithDetailsSpec(dto.Id);
                var entity = await waybillRepo.FirstOrDefaultAsync(spec);

                if (entity == null) throw new Exception("Накладная не найдена");

                dto.Adapt(entity);
                await waybillRepo.UpdateAsync(entity);

                foreach (var oldItem in entity.Items.ToList())
                    await itemRepo.DeleteAsync(oldItem);

                await factory.SaveChangesAsync();

                if (dto.Items != null)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractWaybillItem>();
                        itemEntity.Id = 0;
                        itemEntity.WaybillId = entity.Id;
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
            var entity = await factory.Repository<ContractWaybill>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractWaybill>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }

        public async Task<WaybillPrintDto> GetPrintDataAsync(int waybillId)
        {
            using var factory = _uowFactory.Create();

            var spec = new WaybillPrintSpec(waybillId);
            var waybill = await factory.Repository<ContractWaybill>().FirstOrDefaultAsync(spec);

            if (waybill == null)
                throw new Exception($"Накладная с ID {waybillId} не найдена.");
            if (waybill.Contract == null)
                throw new Exception($"У накладной №{waybill.WaybillNumber} отсутствует привязка к контракту.");
            if (waybill.Contract.Firm == null)
                throw new Exception($"В контракте накладной №{waybill.WaybillNumber} не указан Поставщик (Ваша фирма).");
            if (waybill.Contract.Counterparty == null)
                throw new Exception($"В контракте накладной №{waybill.WaybillNumber} не указан Грузополучатель (Контрагент).");

            var firm = waybill.Contract.Firm;
            var counterparty = waybill.Contract.Counterparty;

            BankAccount? bank = null;
            if (waybill.Invoice != null)
                bank = firm.BankAccounts?.FirstOrDefault(b => b.Id == waybill.Invoice.BankAccountId);

            string bankRequisitesStr = bank != null
                ? DataHelper.CreateBankRequisitesString(bank.BankName, bank.BIC, bank.CorrespondentAccount ?? "", bank.AccountNumber, bank.BankAddress)
                : "";

            string supplierBaseData = DataHelper.CreateOrganizationFullDataString(
                fullName: firm.FullName,
                inn: firm.INN,
                kpp: firm.KPP,
                address: firm.LegalAddress,
                phone: firm.Phone,
                email: firm.Email
            );

            string supplierFullData = string.IsNullOrWhiteSpace(bankRequisitesStr)
                ? supplierBaseData
                : $"{supplierBaseData}, {bankRequisitesStr.Replace(Environment.NewLine, ", ")}";

            string payerFullData = DataHelper.CreateOrganizationFullDataString(
                fullName: counterparty.FullName,
                inn: counterparty.INN,
                kpp: counterparty.KPP,
                address: counterparty.LegalAddress,
                phone: counterparty.Phone,
                email: counterparty.Email ?? string.Empty
            );

            string directorName = "Не указан";
            string accountantName = "Не указан";

            if (firm.Workers != null)
            {
                var dir = firm.Workers.FirstOrDefault(w => w.IsDirector && !w.IsDeleted);
                if (dir != null) 
                    directorName = DataHelper.CreateFIOString(dir.LastName, dir.FirstName, dir.MiddleName ?? "", "s");

                var acc = firm.Workers.FirstOrDefault(w => w.IsAccountant && !w.IsDeleted);
                if (acc != null) 
                    accountantName = DataHelper.CreateFIOString(acc.LastName, acc.FirstName, acc.MiddleName ?? "", "s");
            }

            string customerSignatory = "Не указан";
            if (counterparty.Contacts != null)
            {
                var signer = counterparty.Contacts.FirstOrDefault(c => c.Id == waybill.Contract.CounterpartySignerId && !c.IsDeleted);
                if (signer != null)
                    customerSignatory = DataHelper.CreateFIOString(signer.LastName, signer.FirstName, signer.MiddleName ?? string.Empty, "f");
            }

            var dto = new WaybillPrintDto
            {
                DocumentId = Guid.NewGuid(),
                DocumentDate = DateTime.Now,
                ApplicationName = "Актум",

                WaybillNumber = waybill.WaybillNumber,
                WaybillDate = waybill.WaybillDate.ToString("dd.MM.yyyy"),

                ContractInfo = $"Договор № {waybill.Contract.ContractNumber} от {(waybill.Contract.IssueDate.HasValue ? waybill.Contract.IssueDate.Value.ToString("dd.MM.yyyy") : "___")} г.",

                ConsignorFullData = supplierFullData,
                SupplierFullData = supplierFullData,
                ConsigneeFullData = payerFullData,
                PayerFullData = payerFullData,

                TotalAmount = waybill.TotalAmount ?? 0,
                FormattedTotalAmount = CurrencyFormatter.ShortFormatCurrency(waybill.TotalAmount ?? 0, waybill.CurrencyId),
                VATAmount = waybill.VATAmount ?? 0,
                FormattedVATAmount = CurrencyFormatter.ShortFormatCurrency(waybill.VATAmount ?? 0, waybill.CurrencyId),
                AggregateAmount = waybill.AggregateAmount,
                FormattedAggregateAmount = CurrencyFormatter.ShortFormatCurrency(waybill.AggregateAmount, waybill.CurrencyId),

                AmountInWords = CurrencyFormatter.AmountToWords(waybill.AggregateAmount, waybill.CurrencyId),

                TotalItemsCount = waybill.Items?.Count ?? 0,
                TotalQuantity = waybill.Items?.Sum(i => i.Quantity) ?? 0,

                DirectorName = directorName,
                ChiefAccountantName = accountantName,
                StorekeeperName = directorName,
                CustomerSignatoryName = customerSignatory
            };

            if (waybill.Items != null && waybill.Items.Any())
            {
                int num = 1;
                foreach (var item in waybill.Items.OrderBy(i => i.Id))
                {
                    dto.Items.Add(new WaybillPrintItemDto
                    {
                        Number = num++,
                        Nomenclature = item.NomenclatureName,
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
