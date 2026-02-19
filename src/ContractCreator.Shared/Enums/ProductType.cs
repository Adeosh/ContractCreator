using System.ComponentModel;

namespace ContractCreator.Shared.Enums
{
    public enum ProductType : byte
    {
        /// <summary> Товар </summary>
        [Description("Товар")]
        Good = 1,
        /// <summary> Услуга </summary>
        [Description("Услуга")]
        Service = 2
    }
}
