using System.Xml.Serialization;

namespace ContractCreator.Application.PrintForms.XmlStructs
{
    [XmlRoot("Invoice", Namespace = "urn:invoice:v1")]
    public class InvoiceXmlStruct
    {
        [XmlElement("ServiceInfo")] public ServiceInfo? ServiceInfo { get; set; }
        [XmlElement("InvoiceInfo")] public InvoiceInfo? InvoiceInfo { get; set; }
        [XmlElement("BankDetails")] public BankDetails? BankDetails { get; set; }
        [XmlElement("Firm")] public Organization? Firm { get; set; }
        [XmlElement("Counterparty")] public Organization? Counterparty { get; set; }
        [XmlArray("Items")][XmlArrayItem("Item")] public InvoiceItem[]? Items { get; set; }
        [XmlElement("Totals")] public TotalPriceInfo? Totals { get; set; }
        [XmlElement("Signatories")] public Signatories? Signatories { get; set; }
    }

    public class ServiceInfo
    {
        [XmlElement("Guid")] public string Guid { get; set; } = string.Empty;
        [XmlElement("DateTime")] public string DateTime { get; set; } = string.Empty;
        [XmlElement("Application")] public string Application { get; set; } = string.Empty;
    }

    public class InvoiceInfo
    {
        [XmlElement("Number")] public string Number { get; set; } = string.Empty;
        [XmlElement("Date")] public string Date { get; set; } = string.Empty;
    }

    public class BankDetails
    {
        [XmlElement("BIC")] public string BIC { get; set; } = string.Empty;
        [XmlElement("CorrespondentAccount")] public string CorrespondentAccount { get; set; } = string.Empty;
        [XmlElement("PaidAccount")] public string PaidAccount { get; set; } = string.Empty;
        [XmlElement("BankInfo")] public string BankInfo { get; set; } = string.Empty;
        [XmlElement("INN")] public string INN { get; set; } = string.Empty;
        [XmlElement("KPP")] public string KPP { get; set; } = string.Empty;
        [XmlElement("RecipientName")] public string RecipientName { get; set; } = string.Empty;
    }

    public class Organization
    {
        [XmlElement("FullData")] public string FullData { get; set; } = string.Empty;
    }

    public class InvoiceItem
    {
        [XmlElement("Number")] public int Number { get; set; }
        [XmlElement("Nomenclature")] public string Nomenclature { get; set; } = string.Empty;
        [XmlElement("Quantity")] public int Quantity { get; set; }
        [XmlElement("Unit")] public string Unit { get; set; } = string.Empty;
        [XmlElement("UnitPrice")] public string UnitPrice { get; set; } = string.Empty;
        [XmlElement("TotalAmount")] public string TotalAmount { get; set; } = string.Empty;
    }

    public class TotalPriceInfo
    {
        [XmlElement("TotalAmount")] public string TotalAmount { get; set; } = string.Empty;
        [XmlElement("FormattedTotalAmount")] public string FormattedTotalAmount { get; set; } = string.Empty;
        [XmlElement("VATAmount")] public string VATAmount { get; set; } = string.Empty;
        [XmlElement("FormattedVATAmount")] public string FormattedVATAmount { get; set; } = string.Empty;
        [XmlElement("AggregateAmount")] public string AggregateAmount { get; set; } = string.Empty;
        [XmlElement("FormattedAggregateAmount")] public string FormattedAggregateAmount { get; set; } = string.Empty;
        [XmlElement("AmountInWords")] public string AmountInWords { get; set; } = string.Empty;
        [XmlElement("CountNomenclatureNames")] public string CountNomenclatureNames { get; set; } = string.Empty;
    }

    public class Signatories
    {
        [XmlElement("DirectorName")] public string DirectorName { get; set; } = string.Empty;
        [XmlElement("AccountantName")] public string AccountantName { get; set; } = string.Empty;
    }
}
