using System.ComponentModel;

namespace ContractCreator.Domain.Enums
{
    public enum LegalFormType : byte
    {
        /// <summary> Индивидуальный предприниматель(ИП) </summary>
        [Description("Индивидуальный предприниматель(ИП)")]
        IndividualPerson = 1,
        /// <summary> Общество с ограниченной ответственностью (ООО) </summary>
        [Description("Общество с ограниченной ответственностью (ООО)")]
        LimitedLiabilityCompany = 2,
        /// <summary> Акционерное общество (АО) </summary>
        [Description("Акционерное общество (АО)")]
        JointStockCompany = 3,
        /// <summary> Товарищество </summary>
        [Description("Товарищество")]
        Partnership = 4,
        /// <summary> Кооператив </summary>
        [Description("Кооператив")]
        Cooperative = 5,
        /// <summary> Государственное или муниципальное предприятие </summary>
        [Description("Государственное или муниципальное предприятие")]
        StateAndMunicipalEnterprises = 6,
        /// <summary> Самозанятый </summary>
        [Description("Самозанятый")]
        SelfEmployed = 7
    }
}
