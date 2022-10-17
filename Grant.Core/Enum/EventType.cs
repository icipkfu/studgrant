namespace Grant.Core.Enum
{
    /// <summary>
    /// Тип события
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Создан грант
        /// </summary>
        GrantCreated = 0,

        /// <summary>
        /// Удален грант
        /// </summary>
        GrantDeleted = 1,
        
        /// <summary>
        /// Открыта регистрация в конкурсе 
        /// </summary>
        GrantOpenRegistration = 2,

        /// <summary>
        /// Закрыта регистрация в конкурсе 
        /// </summary>
        GrantCloseRegistration = 3,

        /// <summary>
        /// Пользователь стал участником конкурса 
        /// </summary>
        GrantUserRegister = 4,

        /// <summary>
        /// Пользователь отказался от участия
        /// </summary>
        GrantUserCancel = 5,
        

        /// <summary>
        /// Открыт выбор основных победителей конкурса
        /// </summary>
        GrantOpenWinnersSelection = 6,


        /// <summary>
        /// Закрыт выбор основных победителей конкурса 
        /// </summary>
        GrantCloseWinnersSelection = 7,


        /// <summary>
        /// Открыт выбор дополнительных победителей конкурса
        /// </summary>
        GrantOpenAdditionalWinnerSelection = 8,


        /// <summary>
        /// Закрыт выбор дополнительных победителей конкурса 
        /// </summary>
        GrantCloseAdditionalWinnerSelection = 9,
        

        /// <summary>
        /// Открыта выдача гранта победителям
        /// </summary>
        GrantOpenDelivery = 10,

        /// <summary>
        /// Закрыта выдача гранта победителям
        /// </summary>
        GrantCloseDelivery = 11,

        /// <summary>
        /// Выбран основной победитель конкурса
        /// </summary>
        GrantWinnerSelected = 12,


        /// <summary>
        /// Отменен основной победитель конкурса
        /// </summary>
        GrantWinnerCanceled = 13,


        /// <summary>
        /// Выбран дополнительный победитель конкурса
        /// </summary>
        GrantAdditionalWinnerSelected = 14,


        /// <summary>
        /// Отменен дополнительный победитель конкурса
        /// </summary>
        GrantAdditionalWinnerCanceled = 15,
        
        /// <summary>
        /// Изменен грант
        /// </summary>
        GrantChanged = 16,

        /// <summary>
        /// Грант переведен в черновик
        /// </summary>
        GrantMovedToDraft = 17,

        /// <summary>
        /// Запущен этап завершения конкурса
        /// </summary>
        GrantFinishOpen = 18,

        /// <summary>
        /// Этап завершения конкурса отменен
        /// </summary>
        GrantFinishCanceled = 19,

        /// <summary>
        /// Изменение квот
        /// </summary>
        GrantQuotaChanged = 20,

        /// <summary>
        /// Загружен отчет о победителях
        /// </summary>
        WinnerReportLoaded = 21,

        /// <summary>
        /// Загружен отчет о дополнительных победителях
        /// </summary>
        AdditionalWinnerReportLoaded = 22,

        /// <summary>
        /// Удален отчет о победителях
        /// </summary>
        WinnerReportDeleted = 23,

        /// <summary>
        /// Удален отчет о дополнительных победителях
        /// </summary>
        AdditionalWinnerReportDeleted = 24,

        /// <summary>
        /// Модератор валидировал данные
        /// </summary>
        ModeratorValidateData = 25,

        /// <summary>
        /// Полный доступ
        /// </summary>
        FullAccess = 26,


        /// <summary>
        /// Прикреплен отчет о победителях
        /// </summary>
        AttachWinnersReport = 27,

        /// <summary>
        /// Остановлено редактирвоание данных
        /// </summary>
        UserDataReadonly = 28,

        /// <summary>
        /// Остановлено редактирвоание данных
        /// </summary>
        UserDataEditable = 29,

        /// <summary>
        /// Открыта возможность загуржать отчет о победителях
        /// </summary>
        GrantCanAddReport = 30,

        /// <summary>
        /// Закрыта возможность загуржать отчет о победителях
        /// </summary>
        GrantAddReportNotAvailable = 31,

        CuratorEditedAchievement = 32
    }
}