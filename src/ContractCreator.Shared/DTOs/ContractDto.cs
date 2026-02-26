using ContractCreator.Shared.DTOs.Data;
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
        public string CounterpartyName { get; set; } = string.Empty;
        public int? CounterpartySignerId { get; set; }
        public int FirmId { get; set; }
        public int FirmSignerId { get; set; }
        public int CurrencyId { get; set; }
        public int StageTypeId { get; set; }

        public List<ContractSpecificationDto> Specifications { get; set; } = new();
        public List<ContractInvoiceDto> Invoices { get; set; } = new();
        public List<ContractActDto> Acts { get; set; } = new();
        public List<ContractWaybillDto> Waybills { get; set; } = new();
        public List<ContractStepDto> Steps { get; set; } = new();
        public List<EntityFileDto> Files { get; set; } = new();

        public int? DaysUntilEnd => EndDate.HasValue
            ? EndDate.Value.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber
            : null;

        public string DeadlineStatus
        {
            get
            {
                if (!EndDate.HasValue) return "Бессрочный";
                if (DaysUntilEnd < 0) return "Просрочен";
                if (DaysUntilEnd == 0) return "Заканчивается сегодня!";
                if (DaysUntilEnd <= 3) return $"Критично: {DaysUntilEnd} дн.";
                if (DaysUntilEnd <= 7) return $"Срочно: {DaysUntilEnd} дн.";
                if (DaysUntilEnd <= 14) return $"Скоро: {DaysUntilEnd} дн.";
                return $"В норме ({DaysUntilEnd} дн.)";
            }
        }

        public string DeadlineColor
        {
            get
            {
                if (!EndDate.HasValue) return "Gray";
                if (DaysUntilEnd < 0) return "#D32F2F";
                if (DaysUntilEnd <= 3) return "#F44336";
                if (DaysUntilEnd <= 7) return "#FF9800";
                if (DaysUntilEnd <= 14) return "#FBC02D";
                return "#388E3C";
            }
        }
    }
}
