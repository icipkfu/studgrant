using System.Collections.Generic;
using Grant.Core;
using Grant.Core.Enum;

namespace Grant.WebApi.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Grant.Core.Entities;
    using Grant.Services.DomainService;
    using Grant.Utils.Extensions;

    [Authorize]
    public class BankGilialController : BaseController
    {
        private IBankFilialService _service;

        private IBankFilialService service
        {
            get { return (_service ?? (_service = Container.Get<IBankFilialService>())); }
        }

       
        [HttpGet]
        [Route("api/bankfilial")]
        public async Task<IQueryable<BankFilial>> GetBankFilials()
        {
            var t= await service.GetAllAsync();
            return await service.GetAllAsync();
        }


        // POST api/Universities
        [HttpPost]
        [ResponseType(typeof(BankFilial))]
        [Route("api/bankfilial")]
        public async Task<IHttpActionResult> PostBankFilial(BankFilial bankFilial)
        {
            if (string.IsNullOrEmpty(bankFilial.FilialName) || !this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await service.Create(bankFilial);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(bankFilial);
        }

        // DELETE api/Universities/5
        [HttpDelete]
        [Route("api/bankfilial/{id}")]
        [ResponseType(typeof(BankFilial))]
        public async Task<IHttpActionResult> DeleteUniversity(long id)
        {
            var result = await service.Delete(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }
    }
}