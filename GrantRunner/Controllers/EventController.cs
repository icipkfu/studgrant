using System.Linq;

namespace Grant.WebApi.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Core;
    using Core.Entities;
    using Services.DomainService;

    [Authorize]
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;
        private IGrantEventService grantEventService;

        /// <summary>
        /// Конструктор
        /// </summary>
        public EventController()
        {
            _eventService = this.Container.GetInstance<IEventService>();
            grantEventService = this.Container.GetInstance<IGrantEventService>();
        }


        /// <summary>
        /// Получение списка событий
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/timeline/{id}")]
        public async Task<IQueryable<GrantEvent>> GetEvents([FromBody] EventFilter filter)
        {
            return await grantEventService.GetGrantEvents(filter, null);
        }
        
        /// <summary>
        /// Создание нового события
        /// </summary>
        /// <param name="newEvent"></param>
        /// <returns></returns>
        public async Task<DataResult> CreateEvent(GrantEvent newEvent)
        {
            return await _eventService.CreateEvent(newEvent);
        }
    }
}