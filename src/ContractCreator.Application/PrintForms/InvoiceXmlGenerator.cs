using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ContractCreator.Application.PrintForms.XmlStructs;
using ContractCreator.Shared.DTOs.PrintForms;

namespace ContractCreator.Application.PrintForms
{
    public class InvoiceXmlGenerator
    {
        public string Generate(InvoicePrintDto dto)
        {
            var errors = CheckValues(dto, throwFirstException: false);
            if (errors.Any())
                throw new Exception($"Ошибки при формировании печатной формы:\n{string.Join("\n", errors)}");

            var xmlDoc = FillXmlDocument(dto);

            return SerializeXmlDocument(xmlDoc);
        }

        private List<string> CheckValues(InvoicePrintDto dto, bool throwFirstException = true)
        {
            List<string> lstErrors = new List<string>();

            void HandleException(string error)
            {
                if (throwFirstException)
                {
                    throw new Exception(error);
                }
                else
                {
                    lstErrors ??= new List<string>();
                    lstErrors.Add(error);
                }
            }

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Передан пустой набор данных");

            if (string.IsNullOrWhiteSpace(dto.InvoiceNumber))
                HandleException("Не заполнен номер счета");

            if (string.IsNullOrWhiteSpace(dto.InvoiceDate))
                HandleException("Не заполнена дата счета");

            if (dto.TotalAmount <= 0)
                HandleException("Сумма без НДС должна быть больше нуля");

            if (dto.VATAmount < 0)
                HandleException("Сумма НДС не может быть отрицательной");

            if (dto.AggregateAmount <= 0)
                HandleException("Общая сумма к оплате должна быть больше нуля");

            if (string.IsNullOrWhiteSpace(dto.AmountInWords))
                HandleException("Не заполнена сумма прописью");

            if (string.IsNullOrWhiteSpace(dto.INN))
                HandleException("Не заполнен ИНН получателя");

            if (string.IsNullOrWhiteSpace(dto.KPP))
                HandleException("Не заполнен КПП получателя");

            if (string.IsNullOrWhiteSpace(dto.RecipientName))
                HandleException("Не заполнено наименование получателя");

            if (string.IsNullOrWhiteSpace(dto.BIC))
                HandleException("Не заполнен БИК банка");

            if (string.IsNullOrWhiteSpace(dto.BankInfo))
                HandleException("Не заполнена информация о банке");

            if (string.IsNullOrWhiteSpace(dto.CorrespondentAccount))
                HandleException("Не заполнен корреспондентский счет");

            if (string.IsNullOrWhiteSpace(dto.PaidAccount))
                HandleException("Не заполнен расчетный счет");

            if (string.IsNullOrWhiteSpace(dto.FirmFullData))
                HandleException("Не заполнена полная информация о поставщике");

            if (string.IsNullOrWhiteSpace(dto.CounterpartyFullData))
                HandleException("Не заполнена полная информация о покупателе");

            if (string.IsNullOrWhiteSpace(dto.DirectorName))
                HandleException("Не заполнено имя руководителя");

            if (string.IsNullOrWhiteSpace(dto.AccountantName))
                HandleException("Не заполнено имя бухгалтера");

            if (dto.Items == null || !dto.Items.Any())
                HandleException("Не добавлено ни одной позиции в счет");

            if (dto.Items != null)
            {
                for (int i = 0; i < dto.Items.Count; i++)
                {
                    var item = dto.Items[i];

                    if (item.Number <= 0)
                        HandleException($"Некорректный номер позиции {i + 1}");

                    if (string.IsNullOrWhiteSpace(item.NomenclatureName))
                        HandleException($"Не заполнено наименование в позиции {i + 1}");

                    if (item.Quantity <= 0)
                        HandleException($"Количество должно быть больше нуля в позиции {i + 1} ({item.NomenclatureName})");

                    if (string.IsNullOrWhiteSpace(item.Unit))
                        HandleException($"Не заполнена единица измерения в позиции {i + 1} ({item.NomenclatureName})");

                    if (item.UnitPrice < 0)
                        HandleException($"Цена не может быть отрицательной в позиции {i + 1} ({item.NomenclatureName})");

                    if (item.TotalAmount < 0)
                        HandleException($"Сумма не может быть отрицательной в позиции {i + 1} ({item.NomenclatureName})");
                }
            }

            return lstErrors;
        }

        private InvoiceXmlStruct FillXmlDocument(InvoicePrintDto dto)
        {
            return new InvoiceXmlStruct
            {
                ServiceInfo = new ServiceInfo
                {
                    Guid = dto.DocumentId.ToString(),
                    DateTime = dto.DocumentDate.ToString("s"),
                    Application = dto.ApplicationName
                },
                InvoiceInfo = new InvoiceInfo
                {
                    Number = dto.InvoiceNumber,
                    Date = dto.InvoiceDate
                },
                BankDetails = new BankDetails
                {
                    BIC = dto.BIC,
                    CorrespondentAccount = dto.CorrespondentAccount,
                    PaidAccount = dto.PaidAccount,
                    BankInfo = dto.BankInfo,
                    INN = dto.INN,
                    KPP = dto.KPP,
                    RecipientName = dto.RecipientName
                },
                Firm = new Organization { FullData = dto.FirmFullData },
                Counterparty = new Organization { FullData = dto.CounterpartyFullData },
                Totals = new TotalPriceInfo
                {
                    TotalAmount = dto.TotalAmount.ToString("N2"),
                    FormattedTotalAmount = dto.FormattedTotalAmount,
                    VATAmount = dto.VATAmount.ToString("N2"),
                    FormattedVATAmount = dto.FormattedVATAmount,
                    AggregateAmount = dto.AggregateAmount.ToString("N2"),
                    FormattedAggregateAmount = dto.FormattedAggregateAmount,
                    AmountInWords = dto.AmountInWords,
                    CountNomenclatureNames = dto.CountNomenclatureNames.ToString()
                },
                Signatories = new Signatories
                {
                    DirectorName = dto.DirectorName,
                    AccountantName = dto.AccountantName
                },
                Items = dto.Items.Select(i => new XmlStructs.InvoiceItem
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

        private string SerializeXmlDocument(InvoiceXmlStruct documentStruct)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using StringWriter stringWriter = new StringWriter();
            using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "urn:invoice:v1");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");

            XmlSerializer serializer = new XmlSerializer(typeof(InvoiceXmlStruct));
            serializer.Serialize(xmlWriter, documentStruct, namespaces);

            return stringWriter.ToString();
        }
    }
}
