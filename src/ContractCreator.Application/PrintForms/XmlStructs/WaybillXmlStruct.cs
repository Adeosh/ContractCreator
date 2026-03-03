using System.Xml.Serialization;

namespace ContractCreator.Application.PrintForms.XmlStructs
{
    [XmlRoot("Waybill", Namespace = "urn:waybill:v1")]
    public class WaybillXmlStruct
    {
        [XmlElement("ServiceInfo")] public ServiceInfo? ServiceInfo { get; set; }
        [XmlElement("WaybillInfo")] public WaybillInfo? WaybillInfo { get; set; }
        [XmlElement("Parties")] public PartiesInfo? Parties { get; set; }
        [XmlArray("Items")][XmlArrayItem("Item")] public WaybillItem[]? Items { get; set; }
        [XmlElement("Totals")] public WaybillTotalPriceInfo? Totals { get; set; }
        [XmlElement("Signatories")] public WaybillSignatories? Signatories { get; set; }
    }

    public class WaybillInfo
    {
        [XmlElement("Number")] public string Number { get; set; } = string.Empty;
        [XmlElement("Date")] public string Date { get; set; } = string.Empty;
        [XmlElement("ContractInfo")] public string ContractInfo { get; set; } = string.Empty;
    }

    public class PartiesInfo
    {
        [XmlElement("Consignor")] public string Consignor { get; set; } = string.Empty;
        [XmlElement("Supplier")] public string Supplier { get; set; } = string.Empty;
        [XmlElement("Consignee")] public string Consignee { get; set; } = string.Empty;
        [XmlElement("Payer")] public string Payer { get; set; } = string.Empty;
    }

    public class WaybillItem
    {
        [XmlElement("Number")] public int Number { get; set; }
        [XmlElement("Nomenclature")] public string Nomenclature { get; set; } = string.Empty;
        [XmlElement("Unit")] public string Unit { get; set; } = string.Empty;
        [XmlElement("Quantity")] public int Quantity { get; set; }
        [XmlElement("UnitPrice")] public string UnitPrice { get; set; } = string.Empty;
        [XmlElement("TotalAmount")] public string TotalAmount { get; set; } = string.Empty;
    }

    public class WaybillTotalPriceInfo
    {
        [XmlElement("TotalAmount")] public string TotalAmount { get; set; } = string.Empty;
        [XmlElement("FormattedTotalAmount")] public string FormattedTotalAmount { get; set; } = string.Empty;
        [XmlElement("VATAmount")] public string VATAmount { get; set; } = string.Empty;
        [XmlElement("FormattedVATAmount")] public string FormattedVATAmount { get; set; } = string.Empty;
        [XmlElement("AggregateAmount")] public string AggregateAmount { get; set; } = string.Empty;
        [XmlElement("FormattedAggregateAmount")] public string FormattedAggregateAmount { get; set; } = string.Empty;
        [XmlElement("AmountInWords")] public string AmountInWords { get; set; } = string.Empty;
        [XmlElement("TotalItemsCount")] public int TotalItemsCount { get; set; }
        [XmlElement("TotalQuantity")] public int TotalQuantity { get; set; }
    }

    public class WaybillSignatories
    {
        [XmlElement("DirectorName")] public string DirectorName { get; set; } = string.Empty;
        [XmlElement("ChiefAccountantName")] public string ChiefAccountantName { get; set; } = string.Empty;
        [XmlElement("StorekeeperName")] public string StorekeeperName { get; set; } = string.Empty;
        [XmlElement("CustomerSignatoryName")] public string CustomerSignatoryName { get; set; } = string.Empty;
    }
}
