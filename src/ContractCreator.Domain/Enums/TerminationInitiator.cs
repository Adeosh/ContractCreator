using System.ComponentModel;

namespace ContractCreator.Domain.Enums
{
    public enum TerminationInitiator : byte
    {
        /// <summary> Заказчик </summary>
        [Description("Заказчик")]
        Customer = 1,
        /// <summary> Исполнитель </summary>
        [Description("Исполнитель")]
        Contractor = 2,
        /// <summary> Обе стороны </summary>
        [Description("Обе стороны")]
        Both = 3
    }
}
