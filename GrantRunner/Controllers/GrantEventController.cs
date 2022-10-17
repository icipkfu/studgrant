using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Grant.Core.Entities;
using Grant.Services.DomainService;
using Grant.Utils.Extensions;

namespace Grant.WebApi.Controllers
{

        [Authorize]
    public class GrantEventController : BaseController
    {
        private IGrantEventService _service;

        private IGrantEventService service
        {
            get { return (_service ?? (_service = Container.Get<IGrantEventService>())); }
        }

        [HttpPut]
        [Route("api/grantevent")]
        public async Task<IQueryable<Core.Entities.GrantEvent>> GetEvents([FromBody] EventFilter filter)
        {
            return await service.GetGrantEvents(filter, null);
        }

        [HttpPut]
        [Route("api/grantevent/{id}")]
        public async Task<IQueryable<Core.Entities.GrantEvent>> GetEvents(long id, [FromBody] EventFilter filter)
        {
            var result =  await service.GetGrantEvents(id, filter);
            return result;
        }
    }
}
