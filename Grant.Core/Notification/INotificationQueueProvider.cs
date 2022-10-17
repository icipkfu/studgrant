using System.Collections.Generic;
using Grant.Core.Entities;

namespace Grant.Core.Notification
{
    using System.Threading.Tasks;

    /// <summary>
    /// Провайдер очереди уведомлений
    /// </summary>
    public interface INotificationQueueProvider
    {
        /// <summary>
        /// Поставить уведомление в очередь
        /// </summary>
        /// <param name="student">Студент-получатель</param>
        /// <param name="grant">Грант-инициатор уведомления</param>
        /// <param name="type">тип уведомления</param>
        /// <param name="parameters">объект, свойства которого будут использованы как значения для замены в шаблоне</param>
        /// <returns></returns>
        Task Enqueue(Student student, Entities.Grant grant, NotificationType type, Dictionary<string, object> parameters);

        /// <summary>
        /// Проверить наличие сообщений в очереди
        /// </summary>
        Task<bool> CheckQueue();

        /// <summary>
        /// Разослать очередь
        /// </summary>
        Task SentQueue();
    }
}