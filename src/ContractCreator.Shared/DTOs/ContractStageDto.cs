namespace ContractCreator.Shared.DTOs
{
    public class ContractStageDto
    {
        public int Id { get; set; }
        public int[] TypeIds { get; set; } = new int[0];
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
