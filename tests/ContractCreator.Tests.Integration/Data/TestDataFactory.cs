using ContractCreator.Domain.Models;
using ContractCreator.Domain.Models.Dictionaries;
using ContractCreator.Domain.ValueObjects;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Shared.Enums;

namespace ContractCreator.Tests.Integration.Data
{
    public static class TestDataFactory
    {
        public static void SeedDictionaries(AppDbContext context)
        {
            if (!context.ClassifierOkvs.Any(c => c.Id == 95))
                context.ClassifierOkvs.Add(CreateCurrencyRUB());

            if (!context.ContractStages.Any(s => s.Id == 1))
                context.ContractStages.Add(CreateContractStage());

            if (!context.ClassifierOkopfs.Any(o => o.Id == 12))
                context.ClassifierOkopfs.Add(CreateOkopf(12));

            context.SaveChanges();
        }

        public static AddressData CreateAddress() => new AddressData
        {
            ObjectId = 753471,
            FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
            House = "10",
            Flat = "1",
            Building = "A",
            PostalIndex = "188540",
        };

        public static ClassifierOkv CreateCurrencyRUB() => new ClassifierOkv
        {
            Id = 95,
            NumericCode = 643,
            LetterCode = "RUB",
            CurrencyName = "Российский рубль",
            CountriesCurrencyUsed = "Россия",
            Note = "Базовая валюта"
        };

        public static ClassifierOkopf CreateOkopf(int id = 12) => new ClassifierOkopf
        {
            Id = id,
            Code = "12300",
            Name = "Общества с ограниченной ответственностью"
        };

        public static ContractStage CreateContractStage(int id = 1) => new ContractStage
        {
            Id = id,
            TypeIds = new[] { 1 },
            Name = "Черновик",
            Description = "Начальная стадия контракта"
        };

        public static Firm CreateFirm(int? id = null) => new Firm
        {
            Id = id ?? 1,
            FullName = $"Тестовая фирма {id}",
            ShortName = $"ТФ {id}",
            Phone = "+7 999 123 45 67",
            Email = new EmailAddress($"firm@test.ru"),
            LegalAddress = CreateAddress(),
            ActualAddress = CreateAddress(),
            INN = "1234567890",
            KPP = "123456789",
            OGRN = "1027739460737",
            OKTMO = "45300000",
            OKPO = "12345678",
            LegalFormType = LegalFormType.LimitedLiabilityCompany,
            TaxationType = TaxationSystemType.ESHN,
            IsVATPayment = true,
            CreatedDate = new DateOnly(2024, 1, 1),
            UpdatedDate = new DateOnly(2024, 1, 2),
            IsDeleted = false,
            OkopfId = 12
        };

        public static Worker CreateWorker(int id = 1, int firmId = 1) => new Worker
        {
            Id = id,
            FirmId = firmId,
            FirstName = $"Иван {id}",
            LastName = $"Иванов {id}",
            MiddleName = "Иванович",
            Position = "Директор",
            INN = "1234567890",
            IsDirector = true,
            Phone = "+7 999 000 00 00",
            Email = new EmailAddress($"worker{id}@test.ru"),
            IsDeleted = false
        };

        public static Counterparty CreateCounterparty(int id = 1, int firmId = 1) => new Counterparty
        {
            Id = id,
            FirmId = firmId,
            FullName = $"Тестовый контрагент {id}",
            ShortName = $"ТК {id}",
            INN = "0987654321",
            Phone = "+7 888 123 45 67",
            Email = new EmailAddress($"counterparty{id}@test.ru"),
            LegalAddress = CreateAddress(),
            ActualAddress = CreateAddress(),
            LegalForm = LegalFormType.LimitedLiabilityCompany,
            CreatedDate = new DateOnly(2024, 1, 1),
            IsDeleted = false
        };

        public static Contact CreateContact(int id = 1, int counterpartyId = 1) => new Contact
        {
            Id = id,
            CounterpartyId = counterpartyId,
            FirstName = $"Петр {id}",
            LastName = $"Петров {id}",
            MiddleName = "Петрович",
            Position = "Менеджер",
            Phone = "+7 888 111 22 33",
            Email = new EmailAddress($"contact{id}@test.ru"),
            IsDeleted = false
        };

        public static Contract CreateContract(int id = 1, int firmId = 1, int counterpartyId = 1) => new Contract
        {
            Id = id,
            FirmId = firmId,
            CounterpartyId = counterpartyId,
            Type = ContractType.Contract,
            ContractNumber = $"DOC-{id}",
            ContractPrice = 150000m,
            ContractSubject = "Оказание услуг по разработке",
            IssueDate = new DateOnly(2024, 2, 1),
            CurrencyId = 95,
            StageTypeId = 1
        };

        public static GoodsAndService CreateProduct(int id = 1) => new GoodsAndService
        {
            Id = id,
            Name = $"Тестовая номенклатура {id}",
            UnitOfMeasure = "шт",
            Price = 1500m,
            CurrencyId = 95,
            IsDeleted = false
        };

        public static ContractSpecification CreateSpecification(int id = 1, int contractId = 1) => new ContractSpecification
        {
            Id = id,
            ContractId = contractId,
            NomenclatureName = $"Услуга/Товар {id}",
            UnitOfMeasure = "шт",
            Quantity = 10,
            UnitPrice = 1000m,
            TotalAmount = 10000m,
            VATRate = 20m,
            CurrencyId = 95
        };

        public static ContractInvoice CreateInvoice(int id = 1, int contractId = 1) => new ContractInvoice
        {
            Id = id,
            ContractId = contractId,
            InvoiceNumber = $"INV-{id}",
            InvoiceDate = new DateOnly(2024, 3, 1),
            BankAccountId = 1,
            PurchaserINN = "1234567890",
            PurchaserKPP = "123456789",
            TotalAmount = 10000m,
            VATAmount = 2000m,
            VATRate = 20m,
            AggregateAmount = 12000m,
            CurrencyId = 95
        };

        public static ContractAct CreateAct(int id = 1, int contractId = 1, int invoiceId = 1) => new ContractAct
        {
            Id = id,
            ContractId = contractId,
            InvoiceId = invoiceId,
            ActNumber = $"ACT-{id}",
            ActDate = new DateOnly(2024, 4, 1),
            TotalAmount = 10000m,
            VATRate = 20m,
            VATAmount = 2000m,
            AggregateAmount = 12000m,
            CurrencyId = 95
        };

        public static ContractWaybill CreateWaybill(int id = 1, int contractId = 1, int invoiceId = 1) => new ContractWaybill
        {
            Id = id,
            ContractId = contractId,
            InvoiceId = invoiceId,
            WaybillNumber = $"WB-{id}",
            WaybillDate = new DateOnly(2024, 5, 1),
            TotalAmount = 10000m,
            VATRate = 20m,
            VATAmount = 2000m,
            AggregateAmount = 12000m,
            CurrencyId = 95
        };

        public static ContractStep CreateStep(int id = 1, int contractId = 1) => new ContractStep
        {
            Id = id,
            ContractId = contractId,
            StepName = $"Этап {id}",
            TotalAmount = 50000m,
            CurrencyId = 95,
            StartStepDate = new DateOnly(2024, 6, 1),
            EndStepDate = new DateOnly(2024, 7, 1)
        };
    }
}
