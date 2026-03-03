using ContractCreator.Application.PrintForms.XmlStructs;
using ContractCreator.Shared.DTOs.PrintForms;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ContractCreator.Application.PrintForms
{
    public class ActXmlGenerator
    {
        public string Generate(ActPrintDto dto)
        {
            var errors = CheckValues(dto, throwFirstException: false);
            if (errors.Any())
                throw new Exception($"Ошибки при формировании печатной формы акта:\n{string.Join("\n", errors)}");

            var xmlDoc = FillXmlDocument(dto);

            return SerializeXmlDocument(xmlDoc);
        }

        private List<string> CheckValues(ActPrintDto dto, bool throwFirstException = true)
        {
            List<string> lstErrors = new List<string>();

            void HandleException(string error)
            {
                if (throwFirstException) throw new Exception(error);
                else lstErrors.Add(error);
            }

            if (dto == null) 
                throw new ArgumentNullException(nameof(dto), "Передан пустой набор данных");

            if (string.IsNullOrWhiteSpace(dto.ActNumber)) 
                HandleException("Не заполнен номер акта");

            if (string.IsNullOrWhiteSpace(dto.ActDate)) 
                HandleException("Не заполнена дата акта");

            if (string.IsNullOrWhiteSpace(dto.ContractNumber)) 
                HandleException("Не заполнен номер договора");

            if (dto.AggregateAmount <= 0) 
                HandleException("Общая сумма по акту должна быть больше нуля");

            if (string.IsNullOrWhiteSpace(dto.AmountInWords))
                HandleException("Не заполнена сумма прописью");

            if (string.IsNullOrWhiteSpace(dto.ContractorFullData)) 
                HandleException("Не заполнена полная информация об исполнителе");

            if (string.IsNullOrWhiteSpace(dto.CustomerFullData)) 
                HandleException("Не заполнена полная информация о заказчике");

            if (dto.Items == null || !dto.Items.Any()) 
                HandleException("Не добавлено ни одной позиции в акт");

            if (dto.Items != null)
            {
                for (int i = 0; i < dto.Items.Count; i++)
                {
                    var item = dto.Items[i];
                    if (string.IsNullOrWhiteSpace(item.NomenclatureName))
                        HandleException($"Не заполнено наименование услуги в позиции {i + 1}");

                    if (item.Quantity <= 0)
                        HandleException($"Количество должно быть больше нуля в позиции {i + 1} ({item.NomenclatureName})");

                    if (item.TotalAmount < 0)
                        HandleException($"Сумма не может быть отрицательной в позиции {i + 1} ({item.NomenclatureName})");
                }
            }

            return lstErrors;
        }

        private ActXmlStruct FillXmlDocument(ActPrintDto dto)
        {
            return new ActXmlStruct
            {
                ServiceInfo = new ServiceInfo
                {
                    Guid = dto.DocumentId.ToString(),
                    DateTime = dto.DocumentDate.ToString("s"),
                    Application = dto.ApplicationName
                },
                ActInfo = new ActInfo
                {
                    Number = dto.ActNumber,
                    Date = dto.ActDate,
                    ContractNumber = dto.ContractNumber,
                    ContractDate = dto.ContractDate,
                    FirmName = dto.FirmName,
                    CounterpartyName = dto.CounterpartyName
                },
                Contractor = new Organization { FullData = dto.ContractorFullData },
                Customer = new Organization { FullData = dto.CustomerFullData },
                Totals = new ActTotalPriceInfo
                {
                    TotalAmount = dto.TotalAmount.ToString("N2"),
                    FormattedTotalAmount = dto.FormattedTotalAmount,
                    VATAmount = dto.VATAmount.ToString("N2"),
                    FormattedVATAmount = dto.FormattedVATAmount,
                    AggregateAmount = dto.AggregateAmount.ToString("N2"),
                    FormattedAggregateAmount = dto.FormattedAggregateAmount,
                    AmountInWords = dto.AmountInWords
                },
                Signatories = new ActSignatories
                {
                    ContractorDirectorName = dto.ContractorDirectorName,
                    CustomerSignatoryName = dto.CustomerSignatoryName
                },
                Items = dto.Items.Select(i => new XmlStructs.ActItem
                {
                    Number = i.Number,
                    Nomenclature = i.NomenclatureName,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    UnitPrice = i.UnitPrice.ToString("N2"),
                    TotalAmount = i.TotalAmount.ToString("N2")
                }).ToArray()
            };
        }

        private string SerializeXmlDocument(ActXmlStruct documentStruct)
        {
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
            using StringWriter stringWriter = new StringWriter();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "urn:act:v1");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
            XmlSerializer serializer = new XmlSerializer(typeof(ActXmlStruct));
            serializer.Serialize(xmlWriter, documentStruct, namespaces);

            return stringWriter.ToString();
        }
    }
}
