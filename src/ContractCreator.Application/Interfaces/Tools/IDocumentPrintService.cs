using ContractCreator.Shared.Enums;

namespace ContractCreator.Application.Interfaces.Tools
{
    public interface IDocumentPrintService
    {
        Task<string> GenerateHtmlAsync(int documentId, DocumentType documentType);
    }
}
