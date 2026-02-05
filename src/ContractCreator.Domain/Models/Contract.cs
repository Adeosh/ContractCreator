using ContractCreator.Domain.Enums;
using ContractCreator.Domain.Models.Dictionaries;

namespace ContractCreator.Domain.Models
{
    public class Contract
    {
        public int Id { get; set; }
        /// <summary> Тип (Контракт/Договор) </summary>
        public ContractType Type { get; set; }
        /// <summary> Роль предприятия </summary>
        public ContractEnterpriseRole EnterpriseRole { get; set; }
        /// <summary> Номер </summary>
        public string ContractNumber { get; set; } = string.Empty;
        /// <summary> Цена </summary>
        public decimal ContractPrice { get; set; }
        /// <summary> Предмет контракта\договора(на какую тему) </summary>
        public string? ContractSubject { get; set; }
        /// <summary> Дата и время окончания подачи заявок </summary>
        public DateOnly? SubmissionDate { get; set; }
        /// <summary> Идентификационный код закупки </summary>
        public string? SubmissionCode { get; set; }
        /// <summary> Адрес электронной площадки </summary>
        public string? SubmissionLink { get; set; }
        /// <summary> Дата и время начала торгов </summary>
        public DateOnly? TenderDate { get; set; }
        /// <summary> Дата подписания </summary>
        public DateOnly? IssueDate { get; set; }
        /// <summary> Дата начала дейсвия контракта/договора </summary>
        public DateOnly? StartDate { get; set; }
        /// <summary> Дата окончания дейсвия контракта/договора </summary>
        public DateOnly? EndDate { get; set; }
        /// <summary> Дата завершения или расторжения контракта/договора </summary>
        public DateOnly? ExecutionDate { get; set; }
        /// <summary> Причина расторжения контракта/договора </summary>
        public string? TerminationReason { get; set; }
        /// <summary> Инициатор расторжения контракта/договора </summary>
        public TerminationInitiator? Initiator { get; set; }

        /// <summary> Организация заказчика (Контрагент) <see cref="Counterparty.ID"/></summary>
        public int CounterpartyId { get; set; }
        public virtual Counterparty Counterparty { get; set; } = null!;

        /// <summary> Подписант со стороны контрагента <see cref="Contact.ID"/></summary>
        public int? CounterpartySignerId { get; set; }
        public virtual Contact? CounterpartySigner { get; set; }

        /// <summary> Предприятие </summary>
        public int FirmId { get; set; }
        public virtual Firm Firm { get; set; } = null!;

        /// <summary> Подписант со стороны предприятия <see cref="Worker.ID"/></summary>
        public int FirmSignerId { get; set; }
        public virtual Worker? FirmSigner { get; set; }

        /// <summary> Валюта </summary>
        public int CurrencyId { get; set; }
        public virtual ClassifierOkv Currency { get; set; } = null!;

        /// <summary> Стадия контракта\договора </summary>
        public int StageTypeId { get; set; }
        public virtual ContractStage StageType { get; set; } = null!;

        public virtual ICollection<ContractSpecification> Specifications { get; set; } = new List<ContractSpecification>();
        public virtual ICollection<ContractAct> Acts { get; set; } = new List<ContractAct>();
        public virtual ICollection<ContractInvoice> Invoices { get; set; } = new List<ContractInvoice>();
        public virtual ICollection<ContractStep> Steps { get; set; } = new List<ContractStep>();
    }
}
