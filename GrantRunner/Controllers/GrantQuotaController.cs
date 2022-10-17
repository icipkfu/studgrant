using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Castle.Core.Internal;
using Grant.Core.Entities;
using Grant.Services.DomainService;
using Grant.Utils.Extensions;

namespace Grant.WebApi.Controllers
{

        [Authorize]
    public class GrantQuotaController : BaseController
    {
        private IGrantQuotaService _service;

        private IGrantQuotaService service
        {
            get { return (_service ?? (_service = Container.Get<IGrantQuotaService>())); }
        }

        [HttpGet]
        [Route("api/grantquota")]
        public async Task<IQueryable<GrantQuota>> GetGrantQuotas()
        {
            return await service.GetAllAsync();
        }

        [ResponseType(typeof(GrantQuota))]
        public IQueryable<GrantQuota> GetGrantQuotas([FromUri]string name)
        {
            return service.GetAll().Where(x => x.Name.ToUpper().Contains(name.ToUpper()));
        }

        [Route("api/grantquota/{id}")]
        [ResponseType(typeof(GrantQuota))]
        public async Task<IHttpActionResult> GetGrantQuota(long id)
        {
            var quota = await service.GetGrantQuotas(id);

            if (quota == null)
            {
                return this.NotFound();
            }

            return this.Ok(quota);
        }


        [HttpPut]
        [Route("api/grantquota/{id}")]
        public async Task<IHttpActionResult> PutGrantQuota(long id, GrantQuota[] quotas)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id <= 0)
            {
                return this.BadRequest();
            }

            await service.Update(id, quotas);

            return Ok(quotas);
        }


        [HttpPost]
        [Route("api/grantquota")]
        [ResponseType(typeof(GrantQuota))]
        public async Task<IHttpActionResult> PostQuota(GrantQuota quota)
        {
            if (string.IsNullOrEmpty(quota.Name) || !this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await service.Create(quota);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(quota);
        }

        [HttpDelete]
        [Route("api/grantquota/{id}")]
        public async Task<IHttpActionResult> DeleteQuota(string id)
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
        [ResponseType(typeof(GrantQuota))]
        [Route("api/grantquota/getuniversityquota/{id}/{univerId}")]
        public async Task<IHttpActionResult> GetUniversityQuota(long id, long univerId)
        {

            if (id == 0)
            {
                return null;
            }

            var result = await service.GetUniversityQuota(id,univerId);

            if (!result.WinnerReport.IsNullOrEmpty())
            {
                result.WinnerReportFile = Utils.GetFilesListFromRow(result.WinnerReport, ',').FirstOrDefault();
            }

            if (!result.AdditionalWinnerReport.IsNullOrEmpty())
            {
                result.AdditionalWinnerReportFile = Utils.GetFilesListFromRow(result.AdditionalWinnerReport, ',').FirstOrDefault();
            }

            return Ok(result);
        }

        [HttpGet]
        [ResponseType(typeof(GrantQuota))]
        [Route("api/grantquota/getuniversityfullquota/{id}/{univerId}")]
        public async Task<IHttpActionResult> GetUniversityFullQuota(long id, long univerId)
        {
            if (id == 0)
            {
                return null;
            }

            var result = await service.GetUniversityFullQuota(id, univerId);

            return Ok(result);
        }


        [HttpGet]
        [Route("api/addwinnerreport/{id}/{univerId}/{report}")]
        [ResponseType(typeof(GrantQuota))]
        public async Task<IHttpActionResult> AddWinnerReport(long id, long univerId, string report)
        {
            var result = await service.AddWinnerReport(id, univerId, report);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var quota = (GrantQuota) result.Data;
            quota.WinnerReportFile = Utils.GetFilesListFromRow(quota.WinnerReport, ',').FirstOrDefault();

            return this.Ok(result.Data);
        }

        [HttpGet]
        [Route("api/grantquota/deletewinnerreport/{id}/{univerId}")]
        [ResponseType(typeof(GrantQuota))]
        public async Task<IHttpActionResult> DeleteWinnerReport(long id, long univerId)
        {
            var result = await service.DeleteWinnerReport(id, univerId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok(result.Data);
        }

        [HttpGet]
        [Route("api/addadditionalwinnerreport/{id}/{univerId}/{report}")]
        [ResponseType(typeof(GrantQuota))]
        public async Task<IHttpActionResult> AddAdditionalWinnerReport(long id, long univerId, string report)
        {
            var result = await service.AddAdditionalWinnerReport(id, univerId, report);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var quota = (GrantQuota)result.Data;
            quota.AdditionalWinnerReportFile = Utils.GetFilesListFromRow(quota.AdditionalWinnerReport, ',').FirstOrDefault();

            return this.Ok(result.Data);
        }

        [HttpGet]
        [Route("api/grantquota/deleteadditionalwinnerreport/{id}/{univerId}")]
        [ResponseType(typeof(GrantQuota))]
        public async Task<IHttpActionResult> DeleteAdditionalWinnerReport(long id, long univerId)
        {
            var result = await service.DeleteAdditionalWinnerReport(id, univerId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok(result.Data);
        }
    }
}
