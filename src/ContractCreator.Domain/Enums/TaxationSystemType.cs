using System.ComponentModel;

namespace ContractCreator.Domain.Enums
{
    public enum TaxationSystemType : byte
    {
        /// <summary> "Общая система налогообложения" </summary>
        [Description("Общая система налогообложения")]
        OSNO = 1,
        /// <summary> "Упрощенная система налогообложения" </summary>
        [Description("Упрощенная система налогообложения")]
        USN = 2,
        /// <summary> "Автоматизированная упрощенная система налогообложения" </summary>
        [Description("Автоматизированная упрощенная система налогообложения")]
        AUSN = 3,
        /// <summary> "Единый сельскохозяйственный налог" </summary>
        [Description("Единый сельскохозяйственный налог")]
        ESHN = 4,
        /// <summary> "Патентная система налогообложения" </summary>
        [Description("Патентная система налогообложения")]
        PSN = 5,
        /// <summary> "Налог на профессиональный доход" </summary>
        [Description("Налог на профессиональный доход")]
        NPD = 6
    }
}
