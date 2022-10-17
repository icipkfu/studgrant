using System.Collections.Generic;
using Grant.Core;
using Grant.Core.Entities;

namespace Grant.WebApi.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grant.Services.DomainService;
    using Grant.Utils.Extensions;


        [Authorize]
    public class GrantController : BaseController
    {
        private IGrantService _service;

        private IGrantService service
        {
            get { return (_service ?? (_service = Container.Get<IGrantService>())); }
        }

        [HttpGet]
        [Route("api/grant")]
        public async Task<IQueryable<Core.Entities.Grant>> GetGrants()
        {
            return await service.GetAllAsync();
        }

        [HttpGet]
        [Route("api/grant/list")]
        public async Task<DataResult> GetList()
        {
            return await service.GetGrantList();
        }

        [ResponseType(typeof(Core.Entities.Grant))]
        public IQueryable<Core.Entities.Grant> GetGrants([FromUri]string name)
        {
            return service.GetAll().Where(x=> x.Name.ToUpper().Contains(name.ToUpper()));
        }

        [Route("api/grant/{id}")]
        [ResponseType(typeof(Core.Entities.Grant))]
        public async Task<IHttpActionResult> GetGrant(long id)
        {
            var grant = await service.Get(id); 

            if (grant == null)
            {
                return this.NotFound();
            }

            grant.AttachmentsLinks = Utils.GetFilesListFromRow(grant.AttachmentFiles, '|');

            return this.Ok(grant);
        }

        [HttpPut]
        [Route("api/grant/{id}")]
        public async Task<IHttpActionResult> PutGrant(long id, Core.Entities.Grant grant)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != grant.Id)
            {
                return this.BadRequest();
            }

            await service.Update(grant);

            return this.StatusCode(HttpStatusCode.NoContent);
        }


        [HttpPost]
        [Route("api/grant")]
        [ResponseType(typeof(Core.Entities.Grant))]
        public async Task<IHttpActionResult> PostGrant(Core.Entities.Grant grant)
        {
            if (string.IsNullOrEmpty(grant.Name) || !this.ModelState.IsValid )
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await service.Create(grant);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(grant);
        }

        [HttpDelete]
        [Route("api/grant/{id}")]
        public async Task<IHttpActionResult> DeleteGrant(string id)
        {
            var Id = id.ToLong();

            var result = await service.Delete(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/start/{id}")]
        public async Task<IHttpActionResult> Start(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.StartGrant(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/closeregistration/{id}")]
        public async Task<IHttpActionResult> CloseRegistration(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.CloseRegistration(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/openwinnersselection/{id}")]
        public async Task<IHttpActionResult> OpenWinnersSelection(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.OpenWinnersSelection(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/changeCanAddReport/{id}/{option}")]
        public async Task<IHttpActionResult> СhangeCanAddReport(long id, bool option)
        {
            var result = await service.СhangeCanAddReport(id, option);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }



        [HttpGet]
        [Route("api/grant/openadditionalselection/{id}")]
        public async Task<IHttpActionResult> OpenAdditionalSelection(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.OpenAdditionalWinnersSelection(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/closeadditionalselection/{id}")]
        public async Task<IHttpActionResult> CloseAdditionalSelection(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.CloseAdditionalWinnersSelection(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/closewinnersselection/{id}")]
        public async Task<IHttpActionResult> CloseWinnersSelection(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.CloseWinnersSelection(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/returndraft/{id}")]
        public async Task<IHttpActionResult> ReturnToDraft(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.ReturnToDraft(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/opendelivery/{id}")]
        public async Task<IHttpActionResult> OpenDelivery(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.OpenDelivery(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/canceldelivery/{id}")]
        public async Task<IHttpActionResult> CancelDelivery(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.CancelDelivery(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/openfinal/{id}")]
        public async Task<IHttpActionResult> OpenFinal(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.OpenFinal(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/grant/cancelfinal/{id}")]
        public async Task<IHttpActionResult> CancelFinal(string id)
        {
            long Id = id.ToLong();

            if (Id == 0)
            {
                return BadRequest("Нe задан ID");
            }

            var result = await service.CancelFinal(Id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }


        [HttpGet]
        [Route("api/grant/getadmins/{id}")]
        public async Task<IEnumerable<Student>> GetAdmins(long id)
        {
            return await service.GetAdmins(id);
        }


        [HttpGet]
        [Route("api/grant/getregchart/{id}")]
        public async Task<object> GetGrantRegChart(long id)
        {
            return await service.GetGrantRegChart(id);
        }
        
    }
}
