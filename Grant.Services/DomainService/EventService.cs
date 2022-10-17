using System.Linq;
using System.Threading.Tasks;

namespace Grant.Services.DomainService
{
    using Core;
    using Core.Entities;
    using DataAccess;

    public class EventService : IEventService
    {
        /// <summary>
        /// Реестр событий 
        /// </summary>
        private readonly IRepository<GrantEvent> _eventRepository;
        
        /// <summary>
        /// Реестр событий
        /// </summary>
        /// <param name="repository"></param>
        public EventService(IRepository<GrantEvent> repository)
        {
            _eventRepository = repository;
        }

        /// <summary>
        /// Получить список всех событий
        /// </summary>
        /// <returns>Список событий</returns>
        public async Task<IQueryable<GrantEvent>> GetAllAsync()
        {
            return ( _eventRepository.GetAll().AsQueryable());
        }


        /// <summary>
        /// Создание нового события
        /// </summary>
        /// <param name="entity">Сущность "Событие"</param>
        /// <returns>Результат выполнения</returns>
        public Task<DataResult> CreateEvent(GrantEvent entity)
        {
            return _eventRepository.Create(entity);
        }
    }
}