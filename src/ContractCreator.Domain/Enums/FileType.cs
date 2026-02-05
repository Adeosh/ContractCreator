using System.ComponentModel;

namespace ContractCreator.Domain.Enums
{
    public enum FileType : byte
    {
        /// <summary> Предприятие </summary>
        [Description("Предприятие")]
        Firm = 1,
        /// <summary> Контракт </summary>
        [Description("Контракт")]
        Contract = 2,
        /// <summary> Договор </summary>
        [Description("Договор")]
        Agreement = 3,
        /// <summary> Контрагент </summary>
        [Description("Контрагент")]
        Counterparty = 4
    }
}
