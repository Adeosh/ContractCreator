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
    public class ContractInvoiceService : IContractInvoiceService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ContractInvoiceService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<ContractInvoiceDto>> GetByContractIdAsync(int contractId)
        {
            using var factory = _uowFactory.Create();

            var allInvoices = await factory.Repository<ContractInvoice>().ListAllAsync();
            var filtered = allInvoices.Where(x => x.ContractId == contractId).ToList();

            return filtered.Adapt<IEnumerable<ContractInvoiceDto>>();
        }

        public async Task<ContractInvoiceDto?> GetByIdAsync(int id)
        {
            using var factory = _uowFactory.Create();

            var spec = new InvoiceByIdWithDetailsSpec(id);
            var invoice = await factory.Repository<ContractInvoice>().FirstOrDefaultAsync(spec);

            return invoice?.Adapt<ContractInvoiceDto>();
        }

        public async Task<int> CreateAsync(ContractInvoiceDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var entity = dto.Adapt<ContractInvoice>();

                await factory.Repository<ContractInvoice>().AddAsync(entity);
                await factory.SaveChangesAsync();

                if (dto.Items != null && dto.Items.Any())
                {
                    var itemRepo = factory.Repository<ContractInvoiceItem>();
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractInvoiceItem>();
                        itemEntity.Id = 0;
                        itemEntity.InvoiceId = entity.Id;
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

        public async Task UpdateAsync(ContractInvoiceDto dto)
        {
            using var factory = _uowFactory.Create();
            await factory.BeginTransactionAsync();

            try
            {
                var invoiceRepo = factory.Repository<ContractInvoice>();
                var itemRepo = factory.Repository<ContractInvoiceItem>();

                var spec = new InvoiceByIdWithDetailsSpec(dto.Id);
                var entity = await invoiceRepo.FirstOrDefaultAsync(spec);

                if (entity == null) throw new Exception("Счет не найден");

                dto.Adapt(entity);
                await invoiceRepo.UpdateAsync(entity);

                foreach (var oldItem in entity.Items.ToList())
                    await itemRepo.DeleteAsync(oldItem);

                await factory.SaveChangesAsync();

                if (dto.Items != null)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var itemEntity = itemDto.Adapt<ContractInvoiceItem>();
                        itemEntity.Id = 0;
                        itemEntity.InvoiceId = entity.Id;
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
            var entity = await factory.Repository<ContractInvoice>().GetByIdAsync(id);
            if (entity != null)
            {
                await factory.Repository<ContractInvoice>().DeleteAsync(entity);
                await factory.SaveChangesAsync();
            }
        }

        public async Task<InvoicePrintDto> GetPrintDataAsync(int invoiceId)
        {
            using var factory = _uowFactory.Create();

            var spec = new InvoicePrintSpec(invoiceId);
            var invoice = await factory.Repository<ContractInvoice>().FirstOrDefaultAsync(spec);

            if (invoice == null)
                throw new Exception($"Счет с ID {invoiceId} не найден.");
            if (invoice.Contract == null)
                throw new Exception($"У счета №{invoice.InvoiceNumber} отсутствует привязка к контракту.");
            if (invoice.Contract.Firm == null)
                throw new Exception($"В контракте счета №{invoice.InvoiceNumber} не указана ваша фирма (Исполнитель).");
            if (invoice.Contract.Counterparty == null)
                throw new Exception($"В контракте счета №{invoice.InvoiceNumber} не указан контрагент (Заказчик).");

            var firm = invoice.Contract.Firm;
            var counterparty = invoice.Contract.Counterparty;

            var bank = firm.BankAccounts?.FirstOrDefault(b => b.Id == invoice.BankAccountId);
            if (bank == null)
                throw new Exception("Не найдены банковские реквизиты, указанные в счете. Возможно, счет был удален.");

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

            string directorName = "Не указан";
            string accountantName = "Не указан";

            if (firm.Workers != null)
            {
                var dir = firm.Workers.FirstOrDefault(w => w.IsDirector && !w.IsDeleted);
                if (dir != null)
                    directorName = DataHelper.CreateFIOString(dir.LastName, dir.FirstName, dir.MiddleName ?? string.Empty, "s");

                var acc = firm.Workers.FirstOrDefault(w => w.IsAccountant && !w.IsDeleted);
                if (acc != null)
                    accountantName = DataHelper.CreateFIOString(acc.LastName, acc.FirstName, acc.MiddleName ?? string.Empty, "s");
            }

            var dto = new InvoicePrintDto
            {
                DocumentId = Guid.NewGuid(),
                DocumentDate = DateTime.Now,

                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate.ToString("dd.MM.yyyy"),

                BIC = bank.BIC,
                CorrespondentAccount = bank.CorrespondentAccount ?? "",
                PaidAccount = bank.AccountNumber,
                BankInfo = string.Join(" - ", new[] { bank.BankName, bank.BankAddress }.Where(s => !string.IsNullOrWhiteSpace(s))),

                INN = invoice.PurchaserINN,
                KPP = invoice.PurchaserKPP,
                RecipientName = firm.FullName,

                FirmFullData = firmFullData,
                CounterpartyFullData = counterpartyFullData,

                TotalAmount = invoice.TotalAmount ?? 0,
                FormattedTotalAmount = CurrencyFormatter.ShortFormatCurrency(invoice.TotalAmount ?? 0, invoice.CurrencyId),
                VATAmount = invoice.VATAmount ?? 0,
                FormattedVATAmount = CurrencyFormatter.ShortFormatCurrency(invoice.VATAmount ?? 0, invoice.CurrencyId),
                AggregateAmount = invoice.AggregateAmount,
                FormattedAggregateAmount = CurrencyFormatter.ShortFormatCurrency(invoice.AggregateAmount, invoice.CurrencyId),

                AmountInWords = CurrencyFormatter.AmountToWords(invoice.AggregateAmount, invoice.CurrencyId),
                CountNomenclatureNames = invoice.Items?.Count ?? 0,

                DirectorName = directorName,
                AccountantName = accountantName,
            };

            if (invoice.Items != null && invoice.Items.Any())
            {
                int num = 1;
                foreach (var item in invoice.Items.OrderBy(i => i.Id))
                {
                    dto.Items.Add(new InvoicePrintItemDto
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
