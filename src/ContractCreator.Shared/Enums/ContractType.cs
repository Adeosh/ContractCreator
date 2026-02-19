using System.ComponentModel;

namespace ContractCreator.Shared.Enums
{
    public enum ContractType : byte
    {
        /// <summary> Контракт </summary>
        [Description("Контракт")]
        Contract = 1,
        /// <summary> Договор </summary>
        [Description("Договор")]
        Agreement = 2
    }
}
