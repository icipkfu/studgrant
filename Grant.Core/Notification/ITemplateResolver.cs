namespace Grant.Core.Notification
{
    /// <summary>
    /// Разруливатель шаблонов уведомлений
    /// </summary>
    public interface ITemplateResolver
    {
        /// <summary>
        /// Получить шаблон по типу
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        NotificationTemplate GetTemplate(NotificationType type);
    }
}