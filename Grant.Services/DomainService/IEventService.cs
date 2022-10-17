namespace Grant.Services.DomainService
{
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;

    public interface IEventService
    {
        /// <summary>
        /// Получить список всех событий
        /// </summary>
        /// <returns>Список событий</returns>
        Task<IQueryable<GrantEvent>> GetAllAsync();

        /// <summary>
        /// Создание нового события
        /// </summary>
        /// <param name="entity">Сущность "Событие"</param>
        /// <returns>Результат выполнения</returns>
        Task<DataResult> CreateEvent(GrantEvent entity);
    }
}
