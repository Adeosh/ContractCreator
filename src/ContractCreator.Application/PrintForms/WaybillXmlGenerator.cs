using ContractCreator.Application.PrintForms.XmlStructs;
using ContractCreator.Shared.DTOs.PrintForms;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ContractCreator.Application.PrintForms
{
    public class WaybillXmlGenerator
    {
        public string Generate(WaybillPrintDto dto)
        {
            var errors = CheckValues(dto, throwFirstException: false);
            if (errors.Any())
                throw new Exception($"Ошибки при формировании печатной формы накладной:\n{string.Join("\n", errors)}");

            var xmlDoc = FillXmlDocument(dto);
            return SerializeXmlDocument(xmlDoc);
        }

        private List<string> CheckValues(WaybillPrintDto dto, bool throwFirstException = true)
        {
            List<string> lstErrors = new List<string>();

            void HandleException(string error)
            {
                if (throwFirstException) throw new Exception(error);
                else lstErrors.Add(error);
            }

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Передан пустой набор данных");

            if (dto.DocumentId == Guid.Empty)
                HandleException("Не заполнен уникальный идентификатор документа (Guid)");

            if (string.IsNullOrWhiteSpace(dto.ApplicationName))
                HandleException("Не указано название системы-источника");

            if (string.IsNullOrWhiteSpace(dto.WaybillNumber))
                HandleException("Не заполнен номер накладной");

            if (string.IsNullOrWhiteSpace(dto.WaybillDate))
                HandleException("Не заполнена дата накладной");

            if (string.IsNullOrWhiteSpace(dto.ContractInfo))
                HandleException("Отсутствует информация об основании (договоре)");

            if (string.IsNullOrWhiteSpace(dto.SupplierFullData))
                HandleException("Не заполнены полные реквизиты Поставщика");

            if (string.IsNullOrWhiteSpace(dto.PayerFullData))
                HandleException("Не заполнены полные реквизиты Плательщика");

            if (string.IsNullOrWhiteSpace(dto.ConsignorFullData))
                HandleException("Не заполнены реквизиты Грузоотправителя");

            if (string.IsNullOrWhiteSpace(dto.ConsigneeFullData))
                HandleException("Не заполнены реквизиты Грузополучателя");

            if (string.IsNullOrWhiteSpace(dto.DirectorName))
                HandleException("Не указано ФИО руководителя");

            if (string.IsNullOrWhiteSpace(dto.CustomerSignatoryName))
                HandleException("Не указано ФИО представителя заказчика");

            if (dto.AggregateAmount <= 0)
                HandleException("Итоговая сумма (с НДС) должна быть больше нуля");

            if (string.IsNullOrWhiteSpace(dto.AmountInWords))
                HandleException("Не заполнена сумма прописью");

            if (dto.Items == null || !dto.Items.Any())
            {
                HandleException("В накладной не может быть 0 позиций");
            }
            else
            {
                foreach (var item in dto.Items)
                {
                    var idx = dto.Items.IndexOf(item) + 1;
                    if (string.IsNullOrWhiteSpace(item.Nomenclature))
                        HandleException($"В позиции №{idx} не указано наименование товара");

                    if (item.Quantity <= 0)
                        HandleException($"В позиции №{idx} ({item.Nomenclature}) количество должно быть больше 0");

                    if (item.UnitPrice < 0)
                        HandleException($"В позиции №{idx} указана отрицательная цена");

                    if (string.IsNullOrWhiteSpace(item.Unit))
                        HandleException($"В позиции №{idx} не указана единица измерения");
                }
            }

            return lstErrors;
        }

        private WaybillXmlStruct FillXmlDocument(WaybillPrintDto dto)
        {
            return new WaybillXmlStruct
            {
                ServiceInfo = new ServiceInfo
                {
                    Guid = dto.DocumentId.ToString(),
                    DateTime = dto.DocumentDate.ToString("s"),
                    Application = dto.ApplicationName
                },
                WaybillInfo = new WaybillInfo
                {
                    Number = dto.WaybillNumber,
                    Date = dto.WaybillDate,
                    ContractInfo = dto.ContractInfo
                },
                Parties = new PartiesInfo
                {
                    Consignor = dto.ConsignorFullData,
                    Supplier = dto.SupplierFullData,
                    Consignee = dto.ConsigneeFullData,
                    Payer = dto.PayerFullData
                },
                Totals = new WaybillTotalPriceInfo
                {
                    TotalAmount = dto.TotalAmount.ToString("N2"),
                    FormattedTotalAmount = dto.FormattedTotalAmount,
                    VATAmount = dto.VATAmount.ToString("N2"),
                    FormattedVATAmount = dto.FormattedVATAmount,
                    AggregateAmount = dto.AggregateAmount.ToString("N2"),
                    FormattedAggregateAmount = dto.FormattedAggregateAmount,
                    AmountInWords = dto.AmountInWords,
                    TotalItemsCount = dto.TotalItemsCount,
                    TotalQuantity = dto.TotalQuantity
                },
                Signatories = new WaybillSignatories
                {
                    DirectorName = dto.DirectorName,
                    ChiefAccountantName = dto.ChiefAccountantName,
                    StorekeeperName = dto.StorekeeperName,
                    CustomerSignatoryName = dto.CustomerSignatoryName
                },
                Items = dto.Items.Select(i => new XmlStructs.WaybillItem
                {
                    Number = i.Number,
                    Nomenclature = i.Nomenclature,
                    Unit = i.Unit,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice.ToString("N2"),
                    TotalAmount = i.TotalAmount.ToString("N2")
                }).ToArray()
            };
        }

        private string SerializeXmlDocument(WaybillXmlStruct documentStruct)
        {
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
            using StringWriter stringWriter = new StringWriter();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "urn:waybill:v1");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
            XmlSerializer serializer = new XmlSerializer(typeof(WaybillXmlStruct));
            serializer.Serialize(xmlWriter, documentStruct, namespaces);

            return stringWriter.ToString();
        }
    }
}
