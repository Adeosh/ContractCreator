using System.ComponentModel;

namespace ContractCreator.Shared.Enums
{
    public enum FileType : byte
    {
        /// <summary> Не определено </summary>
        [Description("Не определено")]
        None = 0,
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
