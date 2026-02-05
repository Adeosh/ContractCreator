using System.ComponentModel;

namespace ContractCreator.Domain.Enums
{
    public enum ContractEnterpriseRole : byte
    {
        /// <summary> Заказчик </summary>
        [Description("Заказчик")]
        Customer = 1,
        /// <summary> Исполнитель </summary>
        [Description("Исполнитель")]
        Contractor = 2
    }
}
