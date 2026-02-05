namespace ContractCreator.Shared.DTOs
{
    public class ContractStageChangeHistoryDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int WorkerId { get; set; }
        public int StageTypeId { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
