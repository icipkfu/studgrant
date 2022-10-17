namespace Grant.Core.Enum
{
    using System;

    [Flags]
    public enum GrantStatus
    {
         /// <summary>
        /// Черновик
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Регистрация участников
        /// </summary>
        Registration = 2,

        /// <summary>
        /// Выбор основных победителей
        /// </summary>
        WinnersSelection = 4,

        /// <summary>
        /// Выбор дополнительных победителей
        /// </summary>
        AddtitionalSelection = 8,

        /// <summary>
        /// Выдача гранта победителям
        /// </summary>
        Delivery = 16,

        /// <summary>
        /// Завершение конкурса
        /// </summary>
        Final = 32,

        /// <summary>
        /// Удален
        /// </summary>
        Deleted = 64,

       /// <summary>
        /// Регистрация участников завершена
        /// </summary>
        RegistrationFinished = 128,

        WinnersSelectionClosed = 256,

        AddtitionalSelectionClosed = 512,

        /// <summary>
        /// Отмена этапа выдачи грантов победителям
        /// </summary>
        DeliveryCancel = 1024,
        
        /// <summary>
        /// Отмена этапа завершения конкурса
        /// </summary>
        FinalCancel = 2048
    }
}
