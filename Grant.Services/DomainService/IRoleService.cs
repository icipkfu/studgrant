namespace Grant.Services.DomainService
{
    using Core.Enum;

    public interface IRoleService
    {
        /// <summary>
        /// Проверка наличия прав на выполнение определенного действия
        /// </summary>
        /// <param name="obj">Сущность, над которым выполняется действие</param>
        /// <param name="eventType">Тип события, которое хотят выполнить</param>
        void CheckAccess(object obj, EventType eventType);
    }
}
