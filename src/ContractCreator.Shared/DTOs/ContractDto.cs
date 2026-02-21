using ContractCreator.Shared.Enums;

namespace ContractCreator.Shared.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public ContractType Type { get; set; }
        public ContractEnterpriseRole EnterpriseRole { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public decimal ContractPrice { get; set; }
        public string? ContractSubject { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public string? SubmissionCode { get; set; }
        public string? SubmissionLink { get; set; }
        public DateOnly? TenderDate { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly? ExecutionDate { get; set; }
        public string? TerminationReason { get; set; }
        public TerminationInitiator? Initiator { get; set; }
        public int CounterpartyId { get; set; }
        public int? CounterpartySignerId { get; set; }
        public int FirmId { get; set; }
        public int FirmSignerId { get; set; }
        public int CurrencyId { get; set; }
        public int StageTypeId { get; set; }

        public List<ContractSpecificationDto> Specifications { get; set; } = new();
        public List<ContractActDto> Acts { get; set; } = new();
        public List<ContractInvoiceDto> Invoices { get; set; } = new();
        public List<ContractStepDto> Steps { get; set; } = new();
    }
}
