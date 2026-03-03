using ContractCreator.Application.Interfaces;
using ContractCreator.Application.Interfaces.Tools;
using ContractCreator.Application.PrintForms;
using ContractCreator.Domain.Interfaces;
using ContractCreator.Domain.Models.Templates;
using ContractCreator.Shared.Enums;
using System.Xml;
using System.Xml.Xsl;

namespace ContractCreator.Application.Services.Tools
{
    public class DocumentPrintService : IDocumentPrintService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IContractInvoiceService _invoiceService;
        private readonly IContractActService _actService;
        private readonly IContractWaybillService _waybillService;

        private static XslCompiledTransform? _cachedTransform = null;
        private static string? _cachedTemplateName = null;
        private static readonly object _lock = new object();

        public DocumentPrintService(
            IUnitOfWorkFactory uowFactory,
            IContractInvoiceService invoiceService,
            IContractActService actService,
            IContractWaybillService waybillService)
        {
            _uowFactory = uowFactory;
            _invoiceService = invoiceService;
            _actService = actService;
            _waybillService = waybillService;
        }

        public async Task<string> GenerateHtmlAsync(int documentId, DocumentType documentType)
        {
            string xmlContent = "";
            string templateName = "";

            switch (documentType)
            {
                case DocumentType.Invoice:
                    var invoiceDto = await _invoiceService.GetPrintDataAsync(documentId);
                    var invoiceGenerator = new InvoiceXmlGenerator();
                    xmlContent = invoiceGenerator.Generate(invoiceDto);
                    templateName = "Invoice";
                    break;

                case DocumentType.Act:
                    var actDto = await _actService.GetPrintDataAsync(documentId);
                    var actGenerator = new ActXmlGenerator();
                    xmlContent = actGenerator.Generate(actDto);
                    templateName = "Act";
                    break;

                case DocumentType.Waybill:
                    var waybillDto = await _waybillService.GetPrintDataAsync(documentId);
                    var waybillGenerator = new WaybillXmlGenerator();
                    xmlContent = waybillGenerator.Generate(waybillDto);
                    templateName = "Waybill";
                    break;

                default:
                    throw new NotSupportedException($"Печать для типа {documentType} не поддерживается.");
            }

            string xsltContent = await GetTemplateFromDbAsync(templateName);

            return TransformToHtml(xmlContent, xsltContent, templateName);
        }

        private async Task<string> GetTemplateFromDbAsync(string templateName)
        {
            using var factory = _uowFactory.Create();

            var templateRepo = factory.Repository<DocumentTemplate>();
            var allTemplates = await templateRepo.ListAllAsync();
            var template = allTemplates.FirstOrDefault(t => t.TemplateName == templateName && t.IsActive);

            if (template == null)
                throw new Exception($"Активный XSLT шаблон '{templateName}' не найден в базе данных.");

            return template.Content;
        }

        private string TransformToHtml(string xmlContent, string xsltContent, string templateName)
        {
            lock (_lock)
            {
                if (_cachedTransform == null || _cachedTemplateName != templateName)
                {
                    _cachedTransform = new XslCompiledTransform();
                    using (StringReader xsltReader = new StringReader(xsltContent))
                    using (XmlReader xmlReader = XmlReader.Create(xsltReader))
                    {
                        _cachedTransform.Load(xmlReader);
                    }
                    _cachedTemplateName = templateName;
                }
            }

            using (StringReader xmlReader = new StringReader(xmlContent))
            using (XmlReader reader = XmlReader.Create(xmlReader))
            using (StringWriter results = new StringWriter())
            {
                _cachedTransform.Transform(reader, null, results);
                return results.ToString();
            }
        }
    }
}
