namespace ContractCreator.Domain.Enums
{
    public enum ContractStageType : byte
    {
        /// <summary> Черновик </summary>
        Draft = 1,
        /// <summary> Согласование </summary>
        Agreement = 2,
        /// <summary> Подача заявок (только для контрактов) </summary>
        ApplicationSubmission = 3,
        /// <summary> Торги (только для контрактов) </summary>
        Tender = 4,
        /// <summary> Торги проиграны (только для контрактов) </summary>
        TenderLost = 5,
        /// <summary> Заключение </summary>
        Conclusion = 6,
        /// <summary> Заключен </summary>
        Concluded = 7,
        /// <summary> На исполнении </summary>
        Execution = 8,
        /// <summary> Исполнен </summary>
        Executed = 9,
        /// <summary> Завершен(Оплачен) </summary>
        Paid = 10,
        /// <summary> Расторжение (только для договоров) </summary>
        Termination = 11,
        /// <summary> Расторгнут (только для договоров) </summary>
        Terminated = 12
    }
}
