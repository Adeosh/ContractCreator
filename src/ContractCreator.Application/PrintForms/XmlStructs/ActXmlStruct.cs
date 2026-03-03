using System.Xml.Serialization;

namespace ContractCreator.Application.PrintForms.XmlStructs
{
    [XmlRoot("Act", Namespace = "urn:act:v1")]
    public class ActXmlStruct
    {
        [XmlElement("ServiceInfo")] public ServiceInfo? ServiceInfo { get; set; }
        [XmlElement("ActInfo")] public ActInfo? ActInfo { get; set; }
        [XmlElement("Contractor")] public Organization? Contractor { get; set; }
        [XmlElement("Customer")] public Organization? Customer { get; set; }
        [XmlArray("Items")][XmlArrayItem("Item")] public ActItem[]? Items { get; set; }
        [XmlElement("Totals")] public ActTotalPriceInfo? Totals { get; set; }
        [XmlElement("Signatories")] public ActSignatories? Signatories { get; set; }
    }

    public class ActInfo
    {
        [XmlElement("Number")] public string Number { get; set; } = string.Empty;
        [XmlElement("Date")] public string Date { get; set; } = string.Empty;
        [XmlElement("ContractNumber")] public string ContractNumber { get; set; } = string.Empty;
        [XmlElement("ContractDate")] public string ContractDate { get; set; } = string.Empty;
        [XmlElement("FirmName")] public string FirmName { get; set; } = string.Empty;
        [XmlElement("CounterpartyName")] public string CounterpartyName { get; set; } = string.Empty;
    }

    public class ActItem
    {
        [XmlElement("Number")] public int Number { get; set; }
        [XmlElement("Nomenclature")] public string Nomenclature { get; set; } = string.Empty;
        [XmlElement("Quantity")] public int Quantity { get; set; }
        [XmlElement("Unit")] public string Unit { get; set; } = string.Empty;
        [XmlElement("UnitPrice")] public string UnitPrice { get; set; } = string.Empty;
        [XmlElement("TotalAmount")] public string TotalAmount { get; set; } = string.Empty;
    }

    public class ActTotalPriceInfo
    {
        [XmlElement("TotalAmount")] public string TotalAmount { get; set; } = string.Empty;
        [XmlElement("FormattedTotalAmount")] public string FormattedTotalAmount { get; set; } = string.Empty;
        [XmlElement("VATAmount")] public string VATAmount { get; set; } = string.Empty;
        [XmlElement("FormattedVATAmount")] public string FormattedVATAmount { get; set; } = string.Empty;
        [XmlElement("AggregateAmount")] public string AggregateAmount { get; set; } = string.Empty;
        [XmlElement("FormattedAggregateAmount")] public string FormattedAggregateAmount { get; set; } = string.Empty;
        [XmlElement("AmountInWords")] public string AmountInWords { get; set; } = string.Empty;
    }

    public class ActSignatories
    {
        [XmlElement("ContractorDirectorName")] public string ContractorDirectorName { get; set; } = string.Empty;
        [XmlElement("CustomerSignatoryName")] public string CustomerSignatoryName { get; set; } = string.Empty;
    }
}
