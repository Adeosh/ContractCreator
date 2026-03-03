namespace ContractCreator.Domain.Models.Templates
{
    public class DocumentTemplate
    {
        public int Id { get; set; }
        public required string TemplateName { get; set; }
        public required string TemplateType { get; set; }
        public required string Content { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
