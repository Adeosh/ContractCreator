namespace ContractCreator.Domain.Models
{
    public class ContractStage
    {
        public int Id { get; set; }
        public required int[] TypeIds { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
