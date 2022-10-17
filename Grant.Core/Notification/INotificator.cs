using System.Collections.Generic;

namespace Grant.Core.Notification
{
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public interface INotificator
    {
        /// <summary>
        /// отправить
        /// </summary>
        /// <param name="to">кому</param>
        /// <param name="type">тип уведомления</param>
        /// <param name="data">объект, свойства которого будут использованы как значения для замены в шаблоне</param>
        /// <returns></returns>
        Task<DataResult> NotifyUser(string to, NotificationType type, Dictionary<string, object> data);
    }
}